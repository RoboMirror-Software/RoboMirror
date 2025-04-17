/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RoboMirror
{
	/// <summary>
	/// Wraps a Robocopy command-line process.
	/// This class augments the ConsoleProcess class by Robocopy-specific
	/// output parsing and an appropriate constructor for mirror task
	/// backup/restore operations and optionally embeds Robocopy into
	/// a VolumeShadowCopySession.
	/// This class is thread-safe.
	/// </summary>
	public sealed class RobocopyProcess : ConsoleProcess
	{
		#region Fields.

		// const after construction:
		string _sourceFolder;
		string _destinationFolder;
		string _otherArguments;
		bool _deleteExtraItems;

		private VolumeShadowCopySession _vscSession;

		private bool _outputIsParsed;

		private int _transfersCount;
		private int _deletionsCount;
		private int _errorsCount;

		#endregion

		#region Properties.

		/// <summary>
		/// Gets the number of transferred files and folders.
		/// </summary>
		public int TransfersCount
		{
			get
			{
				ParseOutput();
				return _transfersCount;
			}
		}

		/// <summary>
		/// Gets the number of deleted files and folders.
		/// </summary>
		public int DeletionsCount
		{
			get
			{
				ParseOutput();
				return _deletionsCount;
			}
		}

		/// <summary>
		/// Gets the number of files and folders which could not be
		/// copied.
		/// </summary>
		public int ErrorsCount
		{
			get
			{
				ParseOutput();
				return _errorsCount;
			}
		}

		#endregion


		/// <param name="task">Task to be backed up/restored.</param>
		/// <param name="reverse">
		/// Indicates whether source and target are to be swapped, i.e.
		/// whether this is a restore or backup operation.
		/// </param>
		public RobocopyProcess(MirrorTask task, bool reverse) : base()
		{
			if (task == null)
				throw new ArgumentNullException("task");

			if (!Directory.Exists(task.Source))
				throw new InvalidOperationException(string.Format("The source folder \"{0}\" does not exist.", task.Source));
			if (!Directory.Exists(task.Target))
				throw new InvalidOperationException(string.Format("The target folder \"{0}\" does not exist.", task.Target));

			// only use the bundled Robocopy version if the system does not ship with one
			string exePath = Path.Combine(Environment.SystemDirectory, "Robocopy.exe");
			if (!File.Exists(exePath))
			{
				exePath = Path.Combine(System.Windows.Forms.Application.StartupPath, @"Tools\Robocopy.exe");

				if (!File.Exists(exePath))
					throw new InvalidOperationException(string.Format("\"{0}\" does not exist.", exePath));
			}

			StartInfo.FileName = exePath;

			BuildArguments(task, reverse);
		}

		/// <summary>
		/// Builds the command-line arguments for Robocopy (StartInfo.Arguments and
		/// _sourceFolder, _destinationFolder, _otherArguments and _deleteExtraItems).
		/// </summary>
		private void BuildArguments(MirrorTask task, bool reverse)
		{
			_sourceFolder = (reverse ? task.Target : task.Source);
			_destinationFolder = (reverse ? task.Source : task.Target);
			_deleteExtraItems = task.DeleteExtraItems;

			var otherArgs = new StringBuilder();
			otherArgs.Append(Properties.Settings.Default.RobocopySwitches);

			// if supported, use restartable mode and fall back to backup mode
			if (UacHelper.IsRobocopyBackupModeSupported())
				otherArgs.Append(" /zb");

			if (!string.IsNullOrEmpty(task.ExtendedAttributes))
			{
				otherArgs.Append(" /copy:dat");
				otherArgs.Append(task.ExtendedAttributes);
			}

			if (task.DeleteExtraItems)
				otherArgs.Append(" /purge");

			if (!string.IsNullOrEmpty(task.ExcludedAttributes))
			{
				otherArgs.Append(" /xa:");
				otherArgs.Append(task.ExcludedAttributes);
			}

			if (task.ExcludedFiles.Count > 0)
			{
				otherArgs.Append(" /xf");
				foreach (string file in task.ExcludedFiles)
					AppendPathOrWildcard(otherArgs, _sourceFolder, file);
			}

			if (task.ExcludedFolders.Count > 0)
			{
				otherArgs.Append(" /xd");
				foreach (string folder in task.ExcludedFolders)
					AppendPathOrWildcard(otherArgs, _sourceFolder, folder);
			}

			_otherArguments = otherArgs.ToString();

			StartInfo.Arguments = string.Format("{0} {1} {2}", PathHelper.QuotePath(_sourceFolder),
				PathHelper.QuotePath(_destinationFolder), _otherArguments);
		}

		private static void AppendPathOrWildcard(StringBuilder arguments, string folder, string pathOrWildcard)
		{
			// paths begin with a directory separator character
			if (pathOrWildcard[0] == Path.DirectorySeparatorChar)
				pathOrWildcard = Path.Combine(folder, pathOrWildcard.Substring(1));

			arguments.Append(' ');

			// enforce enclosing double-quotes because if a volume shadow copy is used,
			// the source volume will be replaced by the mount point which may contain spaces
			arguments.Append(PathHelper.QuotePath(pathOrWildcard, force: true));
		}


		/// <summary>
		/// Kills the process if it is currently running and releases all
		/// used resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			// make sure the volume shadow copy is deleted
			// (disposing is thread-safe and repeatable)
			if (_vscSession != null)
				_vscSession.Dispose();
		}


		/// <summary>
		/// Parses the output if it has not been parsed yet.
		/// </summary>
		private void ParseOutput()
		{
			// make sure parsing is thread-safe
			lock (_syncObject)
			{
				if (_outputIsParsed)
					return;

				var lines = Output;

				// search for the dashed line marking the beginning of Robocopy's summary
				int summaryLineIndex = -1;
				for (int i = lines.Count - 1; i >= 0; i--)
				{
					if (lines[i].StartsWith("----------", StringComparison.Ordinal))
					{
						// jump to the directories line
						summaryLineIndex = i + 3;
						break;
					}
				}

				try
				{
					string foldersSummaryLine = lines[summaryLineIndex];
					string filesSummaryLine = lines[summaryLineIndex + 1];

					// split the lines
					int foldersSeparatorIndex = foldersSummaryLine.IndexOf(':');
					int filesSeparatorIndex = filesSummaryLine.IndexOf(':');

					var foldersStats = foldersSummaryLine.Substring(foldersSeparatorIndex + 1).Split(new char[] { ' ' },
						StringSplitOptions.RemoveEmptyEntries);
					var filesStats = filesSummaryLine.Substring(filesSeparatorIndex + 1).Split(new char[] { ' ' },
						StringSplitOptions.RemoveEmptyEntries);

					_transfersCount = int.Parse(filesStats[1]) +
						int.Parse(foldersStats[1]);
					_deletionsCount = (!_deleteExtraItems ? 0 :
						int.Parse(filesStats[5]) + int.Parse(foldersStats[5]));
					_errorsCount = int.Parse(filesStats[4]) +
						int.Parse(foldersStats[4]);

					_outputIsParsed = true;
				}
				catch (Exception e)
				{
					throw new NotSupportedException("The Robocopy output could not be parsed.", e);
				}
			}
		}


		/// <summary>
		/// Invoked asynchronously when the process has exited.
		/// </summary>
		protected override void OnExited(EventArgs e)
		{
			// fire the event
			base.OnExited(e);

			// delete the volume shadow copy
			// (disposing is thread-safe and repeatable)
			if (_vscSession != null)
				_vscSession.Dispose();
		}


		#region Embedding into a VolumeShadowCopySession.

		/// <summary>
		/// Starts the process asynchronously, embedded into a volume
		/// shadow copy session.
		/// This method is NOT thread-safe, call it only once per instance
		/// and do not call the Start() method afterwards.
		/// </summary>
		/// <param name="onError">
		/// Event handler to be invoked if either the shadow copy could not be
		/// created/mounted or if Robocopy could not be started.
		/// </param>
		public void StartInVscSession(EventHandler<TextEventArgs> onError)
		{
			if (onError == null)
				throw new ArgumentNullException("onError");

			if (HasStarted)
				throw new InvalidOperationException("The process has already been started.");

			string sourceVolume = GetSourceVolume();

			// create a session
			_vscSession = new VolumeShadowCopySession();
			_vscSession.Ready += VscSession_Ready;
			_vscSession.Aborted += onError;

			// create and mount the shadow copy
			_vscSession.Start(sourceVolume);
		}

		/// <summary>
		/// Invoked asynchronously when the volume shadow copy has been
		/// created and mounted.
		/// </summary>
		private void VscSession_Ready(object sender, EventArgs e)
		{
			string sourceVolume = GetSourceVolume();

			// replace all occurrences of the source volume in the command-line
			// arguments (excluding the destination folder) by the mount point
			StartInfo.Arguments = string.Format("{0} {1} {2}",
				PathHelper.QuotePath(_sourceFolder.Replace(sourceVolume, _vscSession.MountPoint)),
				PathHelper.QuotePath(_destinationFolder),
				_otherArguments.Replace(sourceVolume, _vscSession.MountPoint));

			// start Robocopy, making sure the shadow copy is deleted
			// immediately if Robocopy cannot be started
			try
			{
				Start();
			}
			catch (Exception exception)
			{
				// dispose of the session and fire the session's Aborted event,
				// which will invoke the onError delegate supplied to the
				// StartInVscSession() method
				// (disposing is thread-safe and repeatable)
				_vscSession.OnAborted(new TextEventArgs("Robocopy could not be started:\n\n" +
					exception.Message));
			}
		}

		private string GetSourceVolume()
		{
			string r = Path.GetPathRoot(_sourceFolder);
			// remove a trailing directory separator character
			if (r[r.Length - 1] == Path.DirectorySeparatorChar)
				r = r.Substring(0, r.Length - 1);
			return r;
		}

		#endregion
	}
}

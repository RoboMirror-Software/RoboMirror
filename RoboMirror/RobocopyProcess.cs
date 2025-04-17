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


		/// <summary>
		/// Creates a new RobocopyProcess.
		/// </summary>
		/// <param name="task">Task to be backed up/restored.</param>
		/// <param name="reverse">
		/// Indicates whether source and target are to be swapped, i.e.
		/// whether this is a restore or backup operation.
		/// </param>
		public RobocopyProcess(MirrorTask task, bool reverse) :
			base(CreateStartInfo(task, reverse))
		{ }

		#region Static ProcessStartInfo creation.

		/// <summary>
		/// Returns an appropriate process start info.
		/// </summary>
		/// <param name="task">Task to be backed up/restored.</param>
		/// <param name="reverse">
		/// Indicates whether source and target are to be swapped, i.e.
		/// whether this will be a restore or backup operation.
		/// </param>
		private static ProcessStartInfo CreateStartInfo(MirrorTask task, bool reverse)
		{
			if (task == null)
				throw new ArgumentNullException("task");

			if (!Directory.Exists(task.Source))
				throw new InvalidOperationException(string.Format("The source folder \"{0}\" does not exist.", task.Source));
			if (!Directory.Exists(task.Target))
				throw new InvalidOperationException(string.Format("The target folder \"{0}\" does not exist.", task.Target));

			var startInfo = new ProcessStartInfo();

			// only use the bundled Robocopy version if the system does not ship with one
			string path = Path.Combine(Environment.SystemDirectory, "Robocopy.exe");
			if (!File.Exists(path))
			{
				path = Path.Combine(System.Windows.Forms.Application.StartupPath, @"Tools\Robocopy.exe");

				if (!File.Exists(path))
					throw new InvalidOperationException(string.Format("\"{0}\" does not exist.", path));
			}

			startInfo.FileName = path;

			string source = (reverse ? task.Target : task.Source);
			string destination = (reverse ? task.Source : task.Target);

			var arguments = new System.Text.StringBuilder();
			arguments.AppendFormat("\"{0}\" \"{1}\" {2}", source, destination,
				Properties.Settings.Default.RobocopySwitches);

			// if supported, use restartable mode and fall back to backup mode
			if (UacHelper.IsRobocopyBackupModeSupported())
				arguments.Append(" /zb");

			if (!string.IsNullOrEmpty(task.ExtendedAttributes))
			{
				arguments.Append(" /copy:dat");
				arguments.Append(task.ExtendedAttributes);
			}

			if (task.DeleteExtraItems)
				arguments.Append(" /purge");

			if (!string.IsNullOrEmpty(task.ExcludedAttributes))
			{
				arguments.Append(" /xa:");
				arguments.Append(task.ExcludedAttributes);
			}

			if (task.ExcludedFiles.Count > 0)
			{
				arguments.Append(" /xf");
				foreach (string file in task.ExcludedFiles)
				{
					arguments.Append(" \"");

					// wildcards must not contain any path information
					if (file[0] == Path.DirectorySeparatorChar)
						arguments.Append(source);

					arguments.Append(file);
					arguments.Append('\"');
				}
			}

			if (task.ExcludedFolders.Count > 0)
			{
				arguments.Append(" /xd");
				foreach (string folder in task.ExcludedFolders)
				{
					arguments.Append(" \"");

					// wildcards must not contain any path information
					if (folder[0] == Path.DirectorySeparatorChar)
						arguments.Append(source);

					arguments.Append(folder);
					arguments.Append('\"');
				}
			}

			startInfo.Arguments = arguments.ToString();

			return startInfo;
		}

		#endregion

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
					_deletionsCount = int.Parse(filesStats[5]) +
						int.Parse(foldersStats[5]);
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

			string arguments = StartInfo.Arguments;

			// parse the source folder from the command-line arguments
			// it is the first argument and enclosed in quotes
			int sourceStart = 1;
			int sourceEnd = arguments.IndexOf('\"', sourceStart);
			string sourceFolder = arguments.Substring(sourceStart, sourceEnd - sourceStart);

			// the destination folder is the second argument and enclosed in quotes
			int destinationStart = sourceEnd + 3;
			int destinationEnd = arguments.IndexOf('\"', destinationStart);
			string destinationFolder = arguments.Substring(destinationStart,
				destinationEnd - destinationStart);

			string otherArguments = arguments.Substring(destinationEnd + 2);

			// parse the source volume from the source folder
			string sourceVolume = Path.GetPathRoot(sourceFolder);
			if (sourceVolume[sourceVolume.Length - 1] == Path.DirectorySeparatorChar)
				sourceVolume = sourceVolume.Remove(sourceVolume.Length - 1);

			// replace all occurrences of the source volume in the command-line
			// arguments (excluding the destination folder) by "VOLUME"
			// ("C:\bla.log" => "\"VOLUME\"\bla.log", i.e. this results in invalid
			// paths so there should be no problem replacing the token later by
			// the mount point)
			string token = "\"VOLUME\"";
			StartInfo.Arguments = string.Format("\"{0}\" \"{1}\" {2}",
				sourceFolder.Replace(sourceVolume, token),
				destinationFolder,
				otherArguments.Replace(sourceVolume, token));

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
			// replace the "VOLUME" token by the mount point
			StartInfo.Arguments =
				StartInfo.Arguments.Replace("\"VOLUME\"", _vscSession.MountPoint);

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

		#endregion
	}
}

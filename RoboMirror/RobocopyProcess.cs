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
	public enum RobocopySummaryColumn
	{
		Caption = 0,
		Total = 1,
		Copied = 2,
		Skipped = 3,
		Mismatch = 4,
		Failed = 5,
		Extras = 6
	}

	public enum RobocopySummaryRow
	{
		Dirs = 0,
		Files = 1,
		Bytes = 2,
		Times = 3
	}

	[Flags]
	public enum RobocopyExitCodes
	{
		Copies = 1,          // 1+ items copied
		ExtraItems = 2,      // 1+ extra items
		MismatchedItems = 4, // 1+ mismatched items
		CopyErrors = 8,      // 1+ items could not be copied
		FatalError = 16      // serious error (invalid command-line or insufficient permissions etc.)
	}

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
		// const after construction
		private string _switches;

		private VolumeShadowCopySession _vscSession;

		private int _outputSummaryLineIndex = -1;

		/// <summary>
		/// Gets the source folder. This may be the mirror task's target folder!
		/// </summary>
		public string SourceFolder { get; private set; }

		/// <summary>
		/// Gets the destination folder. This may be the mirror task's source folder!
		/// </summary>
		public string DestinationFolder { get; private set; }

		/// <summary>
		/// Gets a value indicating whether extra items are to be deleted.
		/// </summary>
		public bool PurgeExtraItems { get; private set; }


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
				throw new InvalidOperationException(string.Format("The source folder {0} does not exist.", PathHelper.Quote(task.Source)));
			if (!Directory.Exists(task.Target))
				throw new InvalidOperationException(string.Format("The target folder {0} does not exist.", PathHelper.Quote(task.Target)));

			// only use the bundled Robocopy version if the system does not ship with one
			string exePath = Path.Combine(Environment.SystemDirectory, "Robocopy.exe");
			if (!File.Exists(exePath))
			{
				exePath = Path.Combine(System.Windows.Forms.Application.StartupPath, @"Tools\Robocopy.exe");

				if (!File.Exists(exePath))
					throw new InvalidOperationException(string.Format("{0} does not exist.", PathHelper.Quote(exePath)));
			}

			StartInfo.FileName = exePath;

			SourceFolder = (reverse ? task.Target : task.Source);
			DestinationFolder = (reverse ? task.Source : task.Target);
			_switches = BuildSwitches(task, SourceFolder);
			PurgeExtraItems = task.DeleteExtraItems;

			StartInfo.Arguments = string.Format("{0} {1} {2}", PathHelper.QuoteForRobocopy(SourceFolder),
				PathHelper.QuoteForRobocopy(DestinationFolder), _switches);
		}

		/// <summary>
		/// Builds the command-line switches for Robocopy.
		/// </summary>
		private static string BuildSwitches(MirrorTask task, string sourceFolder)
		{
			string basicSwitches = string.IsNullOrEmpty(task.CustomRobocopySwitches)
				? Properties.Settings.Default.RobocopySwitches
				: task.CustomRobocopySwitches;

			// if supported, use Robocopy's backup mode to avoid access denied errors
			if (UacHelper.IsRobocopyBackupModeSupported())
			{
				// do the basic switches include restartable mode (/z)?
				int zIndex = basicSwitches.IndexOf("/z ", StringComparison.OrdinalIgnoreCase);
				if (zIndex < 0 && basicSwitches.EndsWith("/z", StringComparison.OrdinalIgnoreCase))
					zIndex = basicSwitches.Length - 2;

				// if so, change /z to /zb to enable restartable mode with backup mode fallback on access denied,
				// else add /b for normal backup mode
				if (zIndex >= 0)
					basicSwitches = basicSwitches.Substring(0, zIndex) + "/zb" + basicSwitches.Substring(zIndex + 2);
				else
					basicSwitches += " /b";
			}

			var switches = new StringBuilder();
			switches.Append(basicSwitches);

			if (!string.IsNullOrEmpty(task.ExtendedAttributes))
			{
				switches.Append(" /copy:dat");
				switches.Append(task.ExtendedAttributes);
			}

			if (task.DeleteExtraItems)
				switches.Append(" /purge");

			if (!task.OverwriteNewerFiles)
				switches.Append(" /xo"); // exclude older files in the source folder

			if (!string.IsNullOrEmpty(task.ExcludedAttributes))
			{
				switches.Append(" /xa:");
				switches.Append(task.ExcludedAttributes);
			}

			if (task.ExcludedFiles.Count > 0)
			{
				switches.Append(" /xf");
				foreach (string file in task.ExcludedFiles)
					AppendPathOrWildcard(switches, sourceFolder, file);
			}

			if (task.ExcludedFolders.Count > 0)
			{
				switches.Append(" /xd");
				foreach (string folder in task.ExcludedFolders)
					AppendPathOrWildcard(switches, sourceFolder, folder);
			}

			return switches.ToString();
		}

		private static void AppendPathOrWildcard(StringBuilder arguments, string folder, string pathOrWildcard)
		{
			// paths begin with a directory separator character
			if (pathOrWildcard[0] == Path.DirectorySeparatorChar)
				pathOrWildcard = Path.Combine(folder, pathOrWildcard.Substring(1));

			arguments.Append(' ');

			// enforce enclosing double-quotes because if a volume shadow copy is used,
			// the source volume will be replaced by the mount point which may contain spaces
			arguments.Append(PathHelper.QuoteForRobocopy(pathOrWildcard, force: true));
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
		/// Indicates whether any of the specified flags is set in Robocopy's exit code.
		/// Throws if the process has not exited yet.
		/// </summary>
		public bool IsAnyExitFlagSet(RobocopyExitCodes flags)
		{
			return (ExitCode & (int)flags) != 0;
		}

		/// <summary>
		/// Tries to parse a field of the Robocopy summary output.
		/// Returns "NaN" on error.
		/// </summary>
		public string GetSummary(RobocopySummaryRow row, RobocopySummaryColumn column)
		{
			string defaultValue = "NaN";

			int baseRowIndex = GetOutputSummaryLineIndex();
			if (baseRowIndex < 0)
				return defaultValue;

			int rowIndex = baseRowIndex + (int)row;
			int colIndex = (int)column;

			try
			{
				string rawValue = Output[rowIndex].Substring(colIndex * 10, 10).Trim();

				// try to parse it as integer; on success enforce culture-specific group separators
				int integer;
				if (int.TryParse(rawValue, out integer) && integer.ToString() == rawValue)
					return integer.ToString("n0");

				return rawValue;
			}
			catch
			{
				return defaultValue;
			}
		}

		private int GetOutputSummaryLineIndex()
		{
			if (_outputSummaryLineIndex >= 0)
				return _outputSummaryLineIndex;

			// make sure parsing is thread-safe
			lock (_syncObject)
			{
				if (_outputSummaryLineIndex >= 0)
					return _outputSummaryLineIndex;

				var lines = Output;

				// search for the last dashed line marking the beginning of Robocopy's summary
				for (int i = lines.Count - 1 - 7; i >= 0; --i)
				{
					if (lines[i].StartsWith("----------", StringComparison.Ordinal))
					{
						// jump to the directories line
						_outputSummaryLineIndex = i + 3;
						break;
					}
				}
			}

			return _outputSummaryLineIndex;
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
				PathHelper.QuoteForRobocopy(SourceFolder.Replace(sourceVolume, _vscSession.MountPoint)),
				PathHelper.QuoteForRobocopy(DestinationFolder),
				_switches.Replace(sourceVolume + Path.DirectorySeparatorChar, _vscSession.MountPoint + Path.DirectorySeparatorChar));

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
			string r = Path.GetPathRoot(SourceFolder);
			// remove a trailing directory separator character
			if (r[r.Length - 1] == Path.DirectorySeparatorChar)
				r = r.Substring(0, r.Length - 1);
			return r;
		}

		#endregion
	}
}

/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RoboMirror
{
	#region Robocopy enums

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

	#endregion

	#region ProgressEventArgs class

	/// <summary>Extends the EventArgs class by a read-only Percentage property.</summary>
	public class ProgressEventArgs : EventArgs
	{
		public double Percentage { get; private set; }

		public ProgressEventArgs(double percentage)
		{
			Percentage = percentage;
		}
	}

	#endregion


	/// <summary>
	/// Wraps a hidden Robocopy process.
	/// This class augments the ConsoleProcess class by Robocopy-specific
	/// output parsing and very rough progress estimation when combined
	/// with a prior simulation run.
	/// This class is thread-safe.
	/// </summary>
	public sealed class RobocopyProcess : ConsoleProcess
	{
		private readonly int _expectedNumOutputLines;
		private int _currentNumOutputLines = 0;
		private double _lastReportedProgressPercentage = double.NegativeInfinity;

		private int _outputSummaryLineIndex = -1;

		/// <summary>
		/// Gets the source folder. This may be the mirror task's source or target
		/// folder or the mount point of a volume shadow copy snapshot.
		/// </summary>
		public string SourceFolder { get; private set; }

		/// <summary>
		/// Gets the destination folder. This may be the mirror task's target or
		/// source folder.
		/// </summary>
		public string DestinationFolder { get; private set; }

		/// <summary>
		/// Gets a value indicating whether extra items are to be deleted.
		/// </summary>
		public bool PurgeExtraItems { get; private set; }


		/// <summary>
		/// Gets the index of the output line for the directories summary.
		/// </summary>
		private int OutputSummaryLineIndex
		{
			get
			{
				if (_outputSummaryLineIndex >= 0)
					return _outputSummaryLineIndex;

				lock (_syncObject)
				{
					if (_outputSummaryLineIndex >= 0)
						return _outputSummaryLineIndex;

					// make sure the process has exited
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

					return _outputSummaryLineIndex;
				}
			}
		}


		/// <summary>
		/// Fired when the very roughly estimated progress has changed, but only
		/// if expectedNumOutputLines passed to the constructor was &gt; 0.
		/// Invoked asynchronously, except if the event handler is a method
		/// of a Control instance, in which case it will be dispatched in the
		/// thread which created the control.
		/// Do NOT attach or detach event handlers after starting the process!
		/// </summary>
		public event EventHandler<ProgressEventArgs> ProgressChanged;


		/// <param name="task">Task to be backed up/restored.</param>
		/// <param name="expectedNumOutputLines">
		/// Expected total number of output lines for progress estimation, e.g.,
		/// obtained by a prior simulation run.
		/// Required for the ProgressChanged event to be fired.
		/// </param>
		public RobocopyProcess(MirrorTask task, string sourceFolder, string destinationFolder, int expectedNumOutputLines = -1)
		{
			if (task == null)
				throw new ArgumentNullException("task");
			if (string.IsNullOrEmpty(sourceFolder))
				throw new ArgumentNullException("sourceFolder");
			if (string.IsNullOrEmpty(destinationFolder))
				throw new ArgumentNullException("destinationFolder");

			if (!Directory.Exists(sourceFolder))
				throw new InvalidOperationException(string.Format("The source folder {0} does not exist.", PathHelper.Quote(sourceFolder)));
			if (!Directory.Exists(destinationFolder))
				throw new InvalidOperationException(string.Format("The destination folder {0} does not exist.", PathHelper.Quote(destinationFolder)));

			// only use the bundled Robocopy version if the system does not ship with one
			string exePath = Path.Combine(Environment.SystemDirectory, "Robocopy.exe");
			if (!File.Exists(exePath))
			{
				exePath = Path.Combine(System.Windows.Forms.Application.StartupPath, @"Tools\Robocopy.exe");

				if (!File.Exists(exePath))
					throw new InvalidOperationException(string.Format("{0} does not exist.", PathHelper.Quote(exePath)));
			}

#if DEBUG
			exePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DummyConsoleProcess.exe");
#endif

			SourceFolder = sourceFolder;
			DestinationFolder = destinationFolder;
			PurgeExtraItems = task.DeleteExtraItems;

			_expectedNumOutputLines = expectedNumOutputLines;

			StartInfo.FileName = exePath;
			StartInfo.Arguments = string.Format("{0} {1} {2}", PathHelper.QuoteForRobocopy(SourceFolder),
				PathHelper.QuoteForRobocopy(DestinationFolder), BuildSwitches(task, SourceFolder));
		}

		#region Building the command-line switches (static)

		/// <summary>Builds the command-line switches for Robocopy.</summary>
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

		#endregion


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
			const string defaultValue = "NaN";

			int baseRowIndex = OutputSummaryLineIndex;
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
			catch { return defaultValue; }
		}


		/// <summary>
		/// Invoked asynchronously when the process has written a line to stdout or stderr.
		/// Implements a very rough progress estimation system based on the current number
		/// of output lines and a total estimate.
		/// </summary>
		protected override void OnLineWritten(System.Diagnostics.DataReceivedEventArgs e)
		{
			const double MIN_PERCENTAGE_DELTA = 0.1;

			base.OnLineWritten(e);

			if (_expectedNumOutputLines <= 0 || ProgressChanged == null)
				return;

			double expectedPercentage;
			lock (_syncObject)
			{
				int numLines = ++_currentNumOutputLines;
				expectedPercentage = (100.0 * numLines) / _expectedNumOutputLines;

				if (expectedPercentage - _lastReportedProgressPercentage < MIN_PERCENTAGE_DELTA
					|| _lastReportedProgressPercentage >= 100) // no more progress reports if last percentage >= 100%
					return;

				expectedPercentage = Math.Min(100, expectedPercentage); // max 100%
				_lastReportedProgressPercentage = expectedPercentage;
			}

			SmartEventInvoker.FireEvent(ProgressChanged, this, new ProgressEventArgs(expectedPercentage));
		}
	}
}

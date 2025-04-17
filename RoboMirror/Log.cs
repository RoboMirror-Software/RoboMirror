/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Diagnostics;

namespace RoboMirror
{
	/// <summary>
	/// Provides an easy way of writing to the application event log.
	/// This class is thread-safe.
	/// </summary>
	public static class Log
	{
		/// <summary>
		/// Source of the events (displayed in a column in the event viewer).
		/// </summary>
		internal const string SOURCE = "RoboMirror";


		/// <summary>
		/// Writes an event to the application log.
		/// </summary>
		/// <param name="type">Type of the event.</param>
		/// <param name="text">Message of the event.</param>
		public static void Write(EventLogEntryType type, string text)
		{
			// there is a supposed limit of 32766 characters for the text
			// I get Win32Exceptions when inserting >= 31900 characters on Vista x64 SP1,
			// therefore let's use a decrementing limit until we succeed
			for (int limit = 32766; limit > 3; limit -= 1000)
			{
				try
				{
					if (text.Length > limit)
						text = text.Remove(limit - 3) + "...";

					EventLog.WriteEntry(SOURCE, text, type);

					break;
				}
				catch (System.ComponentModel.Win32Exception) { }
			}
		}


		/// <summary>
		/// Logs a Robocopy run.
		/// </summary>
		/// <param name="process">Terminated Robocopy process.</param>
		/// <param name="source">Path to the source folder.</param>
		/// <param name="target">Path to the target folder.</param>
		public static void LogRun(RobocopyProcess process, string source, string target)
		{
			if (process == null)
				throw new ArgumentNullException("process");
			if (string.IsNullOrEmpty(source))
				throw new ArgumentNullException("source");
			if (string.IsNullOrEmpty(target))
				throw new ArgumentNullException("target");

			int exitCode = process.ExitCode;

			if (exitCode == -1)
			{
				Write(EventLogEntryType.Warning,
					string.Format("Mirroring of \"{0}\" to \"{1}\" aborted:\n\n",
					source, target) + process.FullOutput);

				return;
			}

			if ((exitCode & 16) != 0)
			{
				Write(EventLogEntryType.Error,
					string.Format("A fatal error occurred while trying to mirror \"{0}\" to \"{1}\":\n\n",
					source, target) + process.FullOutput);

				return;
			}

			if ((exitCode & 8) != 0)
			{
				Write(EventLogEntryType.Error,
					string.Format("Some items could not be mirrored from \"{0}\" to \"{1}\":\n\n",
					source, target) + process.FullOutput);

				return;
			}

			if (exitCode == 0)
			{
				Write(EventLogEntryType.Information,
					string.Format("\"{0}\" and \"{1}\" are already in sync:\n\n",
					source, target) + process.FullOutput);

				return;
			}

			Write(EventLogEntryType.Information,
				string.Format("\"{0}\" successfully mirrored to \"{1}\":\n\n",
				source, target) + process.FullOutput);
		}
	}
}

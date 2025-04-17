/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace RoboMirror
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (!EnsureEventSourceExists())
				return;

			var args = Environment.GetCommandLineArgs();

			if (args.Length == 2)
			{
				// exit immediately if the -i switch has been specified
				// (this is only used to create the event source)
				if (args[1] == "-i")
					return;

				// the argument is most likely a GUID of a scheduled
				// task to be backed up
				ScheduledBackupExecutor executor;

				try
				{
					executor = new ScheduledBackupExecutor(args[1]);
				}
				catch (InvalidOperationException e)
				{
					Log.Write(EventLogEntryType.Error,
						"A scheduled backup task could not be loaded:\n\n" + e.Message);

					return;
				}

				Application.Run(executor);
			}
			else
			{
				MainForm form;

				try
				{
					form = new MainForm();
				}
				catch (InvalidOperationException e)
				{
					MessageBox.Show(e.Message, "RoboMirror cannot be started",
						MessageBoxButtons.OK, MessageBoxIcon.Error);

					return;
				}

				Application.Run(form);
			}
		}

		/// <summary>
		/// Invoked when an exception has not been caught.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			var exception = e.ExceptionObject as Exception;

			if (exception != null)
			{
				// make sure no exception can be thrown in this delicate handler
				try
				{
					Log.Write(EventLogEntryType.Error,
						"An unexpected exception has occurred:\n\n" + exception.ToString());
				}
				catch { }

				MessageBox.Show("Oops, an unexpected error has occurred.\nDetails may be found in the application event log.",
					"Unexpected RoboMirror error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Makes sure the event source for the application event log exists.
		/// This may involve launching a new admin process and a UAC popup.
		/// </summary>
		/// <returns>
		/// True if the event source exists, false if the user refused to
		/// grant us the admin rights.
		/// </returns>
		private static bool EnsureEventSourceExists()
		{
			// we need to make sure our event source exists
			// if it doesn't, we usually end up with a security exception
			// (due to the security log) and need to create it using admin
			// privileges (UAC popup)
			try
			{
				if (!EventLog.SourceExists(Log.SOURCE))
					EventLog.CreateEventSource(Log.SOURCE, null);
			}
			catch (System.Security.SecurityException)
			{
				// we need admin privileges
				// launch a new admin process using the -i switch
				// the process will create the event source and exit immediately
				var startInfo = new ProcessStartInfo();
				startInfo.FileName = Application.ExecutablePath;
				startInfo.Arguments = "-i";
				startInfo.Verb = "runas"; // this causes the process to require admin rights

				// loop until the user cancels or the admin process has exited
				while (true)
				{
					if (MessageBox.Show("RoboMirror needs admin privileges to create an event source for the application event log.\n" +
						"Press cancel to terminate RoboMirror.",
						"RoboMirror is going to cause a UAC popup", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) ==
						DialogResult.Cancel)
					{
						return false;
					}

					try
					{
						using (var adminProcess = Process.Start(startInfo))
						{
							adminProcess.WaitForExit();
						}

						break;
					}
					catch (System.ComponentModel.Win32Exception)
					{
						// the user refused to grant us the admin rights
					}
				}
			}

			return true;
		}
	}
}

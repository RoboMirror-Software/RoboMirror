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
		/// <summary>The main entry point for the application.</summary>
		[STAThread]
		static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			Application.ThreadException += OnThreadException;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var args = Environment.GetCommandLineArgs();

			if (args.Length == 2)
			{
				// the argument is most likely a GUID of a task scheduled for backup
				GUI.ScheduledBackupExecutor form;

				try
				{
					form = new GUI.ScheduledBackupExecutor(args[1]);
				}
				catch (InvalidOperationException e)
				{
					MessageBox.Show("A scheduled backup task could not be initiated:\n\n" + e.Message,
						"RoboMirror", MessageBoxButtons.OK, MessageBoxIcon.Error);

					return;
				}

				Application.Run(form);
			}
			else
			{
				GUI.MainForm form;

				try
				{
					form = new GUI.MainForm();
				}
				catch (FileLockedException)
				{
					MessageBox.Show("Another RoboMirror instance is currently running.", "RoboMirror cannot be started",
						MessageBoxButtons.OK, MessageBoxIcon.Error);

					return;
				}

				Application.Run(form);
			}
		}

		/// <summary>
		/// Invoked when an exception in a UI thread has not been caught.
		/// </summary>
		private static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			ShowUnhandledExceptionMessageBox(e.Exception);
			Application.Exit();
		}

		/// <summary>
		/// Invoked when an exception in a non-UI thread has not been caught.
		/// </summary>
		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ShowUnhandledExceptionMessageBox(e.ExceptionObject);
			// application will be terminated
		}

		private static void ShowUnhandledExceptionMessageBox(object e)
		{
			try
			{
				string msg = "Oops, an unexpected error has occurred:\n\n" + e.ToString();
				MessageBox.Show(msg, "RoboMirror", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch {}
		}
	}
}

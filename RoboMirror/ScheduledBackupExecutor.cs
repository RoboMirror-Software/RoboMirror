/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RoboMirror
{
	/// <summary>
	/// Executes a scheduled backup in the background while still
	/// providing a minimal GUI (the task tray icon).
	/// </summary>
	public sealed class ScheduledBackupExecutor : ApplicationContext
	{
		private MirrorOperation _operation;


		/// <summary>
		/// Creates a new ScheduledBackupExecutor.
		/// </summary>
		/// <param name="guid">GUID of the task to be backed up.</param>
		public ScheduledBackupExecutor(string guid)
		{
			MirrorTask task = null;

			using (var taskManager = new TaskManager(true))
			{
				task = taskManager.LoadTask(guid);
			}

			if (task == null)
				throw new InvalidOperationException("The task does not exist in the XML file.");

			_operation = new MirrorOperation(task, false);

			_operation.Succeeded += new EventHandler(Operation_Succeeded);
			_operation.Finished += new EventHandler(Operation_Finished);

			_operation.Start(false);
		}

		/// <summary>
		/// Makes sure the operation is disposed of before exiting the application.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (_operation != null)
			{
				_operation.Dispose();
				_operation = null;
			}
		}


		/// <summary>
		/// Invoked when the backup has been completed successfully.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Operation_Succeeded(object sender, EventArgs e)
		{
			try
			{
				using (var taskManager = new TaskManager(false))
				{
					taskManager.SaveTask(_operation.Task);
				}
			}
			catch (InvalidOperationException exception)
			{
				Log.Write(EventLogEntryType.Warning,
					"The last backup timestamp could not be updated:\n\n" + exception.Message);
			}
		}

		/// <summary>
		/// Invoked when the operation has finished.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Operation_Finished(object sender, EventArgs e)
		{
			// exit the application
			ExitThread();
		}
	}
}

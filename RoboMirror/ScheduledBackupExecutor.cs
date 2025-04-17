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
			_operation.Finished += Operation_Finished;

			_operation.Start(false);
		}

		/// <summary>
		/// Makes sure the operation is disposed of before exiting the application.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && _operation != null)
			{
				_operation.Dispose();
				_operation = null;
			}
		}


		/// <summary>
		/// Invoked when the operation has finished.
		/// </summary>
		private void Operation_Finished(object sender, FinishedEventArgs e)
		{
			if (e.Success)
			{
				try
				{
					using (var taskManager = new TaskManager(false))
					{
						// reload the task
						var newTask = taskManager.LoadTask(_operation.Task.Guid);
						newTask.LastOperation = _operation.Task.LastOperation;
						taskManager.SaveTask(newTask);
					}
				}
				catch (Exception exception)
				{
					try
					{
						Log.WriteEntry(_operation.Task, EventLogEntryType.Warning,
							"The last successful operation time stamp could not be updated: " + exception.Message, null);
					}
					catch
					{
						MessageBox.Show("The last successful operation time stamp could not be updated:\n\n" + exception.Message,
							"RoboMirror", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
			}

			// exit the application
			ExitThread();
		}
	}
}

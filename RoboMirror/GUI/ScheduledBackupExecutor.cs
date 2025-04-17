/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Windows.Forms;

namespace RoboMirror.GUI
{
	/// <summary>
	/// Executes a scheduled backup in the background while still
	/// providing a minimal GUI (the task tray icon).
	/// The form itself will be hidden and is only used for proper process
	/// lifetime management (incl. Windows logoff/shutdown/restart).
	/// </summary>
	public sealed class ScheduledBackupExecutor : Form
	{
		private MirrorOperation _operation;

		/// <param name="guid">GUID of the task to be backed up.</param>
		public ScheduledBackupExecutor(string guid)
		{
			MirrorTask task = null;
			using (var taskManager = new TaskManager(true))
				task = taskManager.LoadTask(guid);

			if (task == null)
				throw new InvalidOperationException("The task does not exist in the XML file.");

			_operation = new MirrorOperation(this, task, false);
			_operation.Finished += OnOperationFinished;

			ShowInTaskbar = false;
			WindowState = FormWindowState.Minimized;
		}

		protected override void OnLoad(EventArgs e)
		{
			_operation.Start(simulateFirst: false);
			base.OnLoad(e);
		}

		protected override void OnClosed(EventArgs e)
		{
			_operation.Abort();
			base.OnClosed(e);
		}

		private void OnOperationFinished(object sender, FinishedEventArgs e)
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
						Log.WriteEntry(_operation.Task.Guid, System.Diagnostics.EventLogEntryType.Warning,
							"The last successful operation time stamp could not be updated: " + exception.Message, null);
					}
					catch
					{
						MessageBox.Show("The last successful operation time stamp could not be updated:\n\n" + exception.Message,
							"RoboMirror", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
			}

			Close();
		}
	}
}

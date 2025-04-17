/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;

namespace RoboMirror
{
	/// <summary>
	/// Allows scheduling of a backup task on Windows.
	/// </summary>
	public partial class ScheduleTaskDialog : BaseDialog
	{
		private MirrorTask _mirrorTask;
		private Task _task;

		
		/// <summary>
		/// Creates a new dialog.
		/// </summary>
		/// <param name="mirrorTask"></param>
		public ScheduleTaskDialog(MirrorTask mirrorTask)
		{
			if (mirrorTask == null)
				throw new ArgumentNullException("mirrorTask");

			_mirrorTask = mirrorTask;

			InitializeComponent();

			// search for an existing task
			_task = TaskManager.GetScheduledTask(_mirrorTask);
			if (_task != null)
			{
				checkBox1.Checked = _task.Enabled;

				if (_task.Definition.Triggers.Count == 1)
				{
					var trigger = _task.Definition.Triggers[0];

					if (trigger is DailyTrigger)
						intervalComboBox.SelectedIndex = 0;
					else if (trigger is WeeklyTrigger)
						intervalComboBox.SelectedIndex = 1;
					else if (trigger is MonthlyDOWTrigger)
						intervalComboBox.SelectedIndex = 2;
					else
						throw new NotSupportedException("The existing scheduled task's trigger is not supported.");

					timeMaskedTextBox.Text = trigger.StartBoundary.ToString("t");
				}
			}
			else
			{
				intervalComboBox.SelectedIndex = 1;
				timeMaskedTextBox.Text = DateTime.Now.ToString("t");
			}
		}


		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			intervalComboBox.Enabled = timeMaskedTextBox.Enabled = checkBox1.Checked;
			HasChanged = true;
		}

		private void control_Changed(object sender, EventArgs e)
		{
			HasChanged = true;
		}


		protected override bool ApplyChanges()
		{
			using (var service = new TaskService())
			{
				if (!checkBox1.Checked)
				{
					if (_task != null)
						service.RootFolder.DeleteTask(_task.Name);

					return true;
				}

				TimeSpan time;
				if (!TimeSpan.TryParse(timeMaskedTextBox.Text, out time))
				{
					MessageBox.Show("Please enter a valid time.", "Invalid time",
						MessageBoxButtons.OK, MessageBoxIcon.Error);

					timeMaskedTextBox.Focus();
					return false;
				}

				var definition = service.NewTask();

				definition.RegistrationInfo.Description = string.Format("Mirrors \"{0}\" to \"{1}\".",
					_mirrorTask.Source, _mirrorTask.Target);

				definition.Actions.Add(new ExecAction(Application.ExecutablePath,
					_mirrorTask.Guid, Application.StartupPath));

				if (intervalComboBox.SelectedIndex == 0)
					definition.Triggers.Add(new DailyTrigger());
				else if (intervalComboBox.SelectedIndex == 1)
					definition.Triggers.Add(new WeeklyTrigger());
				else
					definition.Triggers.Add(new MonthlyDOWTrigger());

				var sb = definition.Triggers[0].StartBoundary;
				definition.Triggers[0].StartBoundary = sb.Date + time;

				definition.Principal.LogonType = TaskLogonType.InteractiveToken;

				// set some advanced settings under Vista+
				if (service.HighestSupportedVersion.Major >= 2)
				{
					definition.Settings.AllowHardTerminate = false;
					definition.Settings.StopIfGoingOnBatteries = false;
					definition.Settings.StartWhenAvailable = true;
				}

				if (_task != null)
					service.RootFolder.DeleteTask(_task.Name);

				service.RootFolder.RegisterTaskDefinition(string.Format("RoboMirror ({0})", _mirrorTask.Guid),
					definition, TaskCreation.Create, null, null, TaskLogonType.InteractiveToken, null);

				return true;
			}
		}
	}
}

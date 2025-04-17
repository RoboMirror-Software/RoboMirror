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
		private ScheduledTasksManager _manager;

		public ScheduleTaskDialog(MirrorTask mirrorTask)
		{
			if (mirrorTask == null)
				throw new ArgumentNullException("mirrorTask");

			_mirrorTask = mirrorTask;
			_manager = new ScheduledTasksManager();

			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// search for an existing task
			var scheduledTask = _manager.Get(_mirrorTask);
			if (scheduledTask != null)
			{
				checkBox1.Checked = scheduledTask.Enabled;

				if (scheduledTask.Definition.Triggers.Count != 1)
					throw new NotSupportedException("The existing scheduled task's multiple triggers are not supported.");

				var trigger = scheduledTask.Definition.Triggers[0];

				if (trigger is DailyTrigger)
					intervalComboBox.SelectedIndex = 0;
				else if (trigger is WeeklyTrigger)
					intervalComboBox.SelectedIndex = 1;
				else if (trigger is MonthlyDOWTrigger)
					intervalComboBox.SelectedIndex = 2;
				else
					throw new NotSupportedException("The existing scheduled task's trigger is not supported.");

				dateTimePicker1.Value = trigger.StartBoundary;
			}
			else
			{
				intervalComboBox.SelectedIndex = 1;
				dateTimePicker1.Value = DateTime.Now;
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			_manager.Dispose();
			base.OnFormClosed(e);
		}


		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			intervalComboBox.Enabled = dateTimePicker1.Enabled = checkBox1.Checked;
			HasChanged = true;
		}

		private void control_Changed(object sender, EventArgs e)
		{
			HasChanged = true;
		}


		protected override bool ApplyChanges()
		{
			try
			{
				if (!checkBox1.Checked)
				{
					_manager.Delete(_mirrorTask);
					return true;
				}

				Trigger trigger;

				if (intervalComboBox.SelectedIndex == 0)
					trigger = new DailyTrigger();
				else if (intervalComboBox.SelectedIndex == 1)
					trigger = new WeeklyTrigger();
				else
					trigger = new MonthlyDOWTrigger();

				trigger.StartBoundary = dateTimePicker1.Value;

				_manager.Save(_mirrorTask, trigger);

				return true;
			}
			catch (Exception e)
			{
				MessageBox.Show(this, "The changes could not be saved.\n\nDetails:\n" + e.Message, "Scheduled backup task",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}
	}
}

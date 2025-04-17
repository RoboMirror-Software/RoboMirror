/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RoboMirror.Properties;
using Microsoft.Win32.TaskScheduler;

namespace RoboMirror.GUI
{
	/// <summary>
	/// Main form of the application.
	/// </summary>
	public partial class MainForm : BaseForm
	{
		private TaskManager _taskManager;
		private List<MirrorOperation> _activeOperations = new List<MirrorOperation>();


		/// <summary>
		/// Gets the currently selected task or null if none is selected.
		/// </summary>
		private MirrorTask SelectedTask
		{
			get
			{
				return (listView1.SelectedIndices.Count == 0 ? null :
					(MirrorTask)listView1.SelectedItems[0].Tag);
			}
		}


		/// <exception cref="FileLockedException">The TaskManager could not get exclusive write access to its XML file.</exception>
		public MainForm()
		{
			InitializeComponent();

			backupButton.Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold);

			_taskManager = new TaskManager(false);

			var tasks = _taskManager.LoadTasks();
			foreach (MirrorTask task in tasks)
				AddListViewItem(task);

			// select the first item
			if (listView1.Items.Count > 0)
				listView1.SelectedIndices.Add(0);
		}


		private void pictureBox1_Click(object sender, EventArgs e)
		{
			label1.Text = string.Format("RoboMirror v{0}\nCopyright (c) Martin Kinkelin",
				Application.ProductVersion);

			try { System.Diagnostics.Process.Start("http://robomirror.sourceforge.net/"); }
			catch { }
		}


		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			editButton.Enabled = removeButton.Enabled = historyButton.Enabled = scheduleButton.Enabled =
				(listView1.SelectedIndices.Count > 0);

			backupButton.Enabled = restoreButton.Enabled =
				(listView1.SelectedIndices.Count > 0 && GetAssociatedActiveOperation(SelectedTask) == null);
		}

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			// simulate a click on the edit button when an item is double-clicked
			editButton.PerformClick();
		}

		private void listView1_KeyDown(object sender, KeyEventArgs e)
		{
			// simulate a click on the remove button when del or backspace is pressed
			if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
				removeButton.PerformClick();
		}


		private void addButton_Click(object sender, EventArgs e)
		{
			MirrorTask task = new MirrorTask();

			using (TaskDialog dialog = new TaskDialog(task))
			{
				if (dialog.ShowDialog(this) != DialogResult.Yes)
					return;
			}

			if (!TrySaveTask(task))
				return;

			AddListViewItem(task);

			// select the newly added item
			listView1.SelectedIndices.Clear();
			listView1.SelectedIndices.Add(listView1.Items.Count - 1);
		}

		private bool TrySaveTask(MirrorTask task)
		{
			try
			{
				_taskManager.SaveTask(task);
				return true;
			}
			catch (Exception e)
			{
				MessageBox.Show(this, "The mirror task could not be saved.\n\n" + e.Message,
					"I/O error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		private void editButton_Click(object sender, EventArgs e)
		{
			MirrorTask task = SelectedTask;
			if (task == null)
				return;

			using (TaskDialog dialog = new TaskDialog(task))
			{
				if (dialog.ShowDialog(this) != DialogResult.Yes)
					return;
			}

			if (!TrySaveTask(task))
				return;

			UpdateListViewItem(task);
		}

		private void removeButton_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count == 0)
				return;

			if (MessageBox.Show(this, "Are you sure you want to remove the selected task?", "Confirmation",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			try
			{
				_taskManager.DeleteTask(SelectedTask);
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, "The mirror task could not be deleted.\n\n" + exception.Message,
					"I/O error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			listView1.Items.RemoveAt(listView1.SelectedIndices[0]);
		}


		private void historyButton_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count == 0)
				return;

			using (var dialog = new TaskHistoryForm(SelectedTask))
			{
				dialog.ShowDialog(this);
			}
		}


		private void scheduleButton_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count == 0)
				return;

			using (var dialog = new ScheduleTaskDialog(SelectedTask))
			{
				dialog.ShowDialog(this);
			}
		}


		private void backupButton_Click(object sender, EventArgs e)
		{
			StartOperation(reverse: false, simulateFirst: Settings.Default.SimulateFirst);
		}

		private void restoreButton_Click(object sender, EventArgs e)
		{
			StartOperation(reverse: true, simulateFirst: Settings.Default.SimulateFirst);
		}


		private MirrorOperation GetAssociatedActiveOperation(MirrorTask task)
		{
			foreach (var op in _activeOperations)
			{
				if (op.Task == task)
					return op;
			}

			return null;
		}

		private void StartOperation(bool reverse, bool simulateFirst)
		{
			var task = SelectedTask;
			if (task == null)
				return;

			if (GetAssociatedActiveOperation(task) != null)
				return;

			var operation = new MirrorOperation(this, task, reverse);

			operation.Finished += Operation_Finished;

			_activeOperations.Add(operation);

			backupButton.Enabled = restoreButton.Enabled = false;

			operation.Start(simulateFirst);
		}

		private void Operation_Finished(object sender, FinishedEventArgs e)
		{
			var operation = (MirrorOperation)sender;

			if (e.Success)
			{
				UpdateListViewItem(operation.Task);
				_taskManager.SaveTask(operation.Task);
			}

			_activeOperations.Remove(operation);

			// enable the backup and restore buttons if the associated task
			// is currently selected
			if (SelectedTask == operation.Task)
				backupButton.Enabled = restoreButton.Enabled = true;
		}


		/// <summary>
		/// Creates an appropriate ListViewItem and adds it to the list view.
		/// </summary>
		private void AddListViewItem(MirrorTask task)
		{
			var item = new ListViewItem();
			item.Tag = task;
			item.ImageIndex = 0;
			item.SubItems.Add(task.Source);
			item.SubItems.Add(task.Target);
			item.SubItems.Add(task.LastOperation.HasValue ? task.LastOperation.Value.ToString("g") : "Never");

			listView1.Items.Add(item);
		}

		/// <summary>
		/// Updates the corresponding item in the list view after a task
		/// has been modified.
		/// </summary>
		private void UpdateListViewItem(MirrorTask task)
		{
			foreach (ListViewItem item in listView1.Items)
			{
				if (item.Tag == task)
				{
					item.SubItems[1].Text = task.Source;
					item.SubItems[2].Text = task.Target;
					item.SubItems[3].Text = (task.LastOperation.HasValue ?
						task.LastOperation.Value.ToString("g") : "Never");

					break;
				}
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);

			if (e.Cancel)
				return;

			// * Task Manager:
			// beginning with Windows 8, the task manager ends tasks just like the user would
			// (i.e., sending WM_SYSCOMMAND before WM_CLOSE - this is why e.CloseReason is
			// UserClosing instead of TaskManagerClosing!), but uses a rather short time-out
			// after which the thread seems to be killed
			// so we cannot distinguish between task manager kills and normal user closings
			// anymore
			// so on a task manager kill on Win8+ and if there are active operations, our app
			// will shortly display a confirmation message box before being terminated brutally

			// * Windows logoff/shutdown/restart:
			// although a logoff may be cancelled, it makes no sense as external Robocopy
			// processes are killed anyway
			// so simply do not prompt and then abort in OnFormClosed()

			if (_activeOperations.Count > 0 && e.CloseReason == CloseReason.UserClosing)
			{
				if (MessageBox.Show(this, "Are you sure you want to abort all active operations?",
					"RoboMirror", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) !=
					DialogResult.Yes)
				{
					e.Cancel = true;
					return;
				}
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			// copy _activeOperations to a new array
			// reason: aborting an operation will finish it, and Operation_Finished()
			// removes the operation from the _activeOperations list
			foreach (var operation in _activeOperations.ToArray())
				operation.Abort();

			_taskManager.Dispose();

			base.OnFormClosed(e);
		}
	}
}

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

namespace RoboMirror
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

			AddListViewItem(task);

			// select the newly added item
			listView1.SelectedIndices.Clear();
			listView1.SelectedIndices.Add(listView1.Items.Count - 1);

			_taskManager.SaveTask(task);
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

			UpdateListViewItem(task);

			_taskManager.SaveTask(task);
		}

		private void removeButton_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count == 0)
				return;

			if (MessageBox.Show("Are you sure you want to remove the selected task?", "Confirmation",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			_taskManager.DeleteTask(SelectedTask);

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
			StartOperation(false, Settings.Default.SimulateFirst);
		}

		private void restoreButton_Click(object sender, EventArgs e)
		{
			StartOperation(true, true);
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

			var operation = new MirrorOperation(task, reverse);

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
			ListViewItem item = new ListViewItem();
			item.Tag = task;
			item.ImageIndex = 0;
			item.SubItems.Add(task.Source);
			item.SubItems.Add(task.Target);
			item.SubItems.Add(task.LastBackup.HasValue ? task.LastBackup.Value.ToString("g") : "Never");

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
					item.SubItems[3].Text = (task.LastBackup.HasValue ?
						task.LastBackup.Value.ToString("g") : "Never");

					break;
				}
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
 			base.OnFormClosing(e);

			if (e.Cancel)
				return;

			foreach (var operation in _activeOperations)
			{
				if (!operation.Abort())
				{
					e.Cancel = true;
					return;
				}
			}

			_taskManager.Dispose();
		}
	}
}

/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using Microsoft.Win32.TaskScheduler;
using RoboMirror.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RoboMirror.GUI
{
	public partial class MainForm : BaseForm
	{
		private TaskManager _taskManager;


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


		/// <exception cref="FileLockedException">
		/// Could not get exclusive write access to the Tasks.xml file (most likely because another GUI instance is running).
		/// </exception>
		public MainForm()
		{
			InitializeComponent();

			HideQueuePanel();

			backupButton.Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold);

			_taskManager = new TaskManager(readOnly: false);

			var tasks = _taskManager.LoadTasks();
			Log.LoadLastSuccessTimeStamps(tasks);

			foreach (var task in tasks)
				AddListViewItem(task);

			// select the first item
			if (listView1.Items.Count > 0)
				listView1.SelectedIndices.Add(0);
		}


		private void HideQueuePanel()
		{
			SuspendLayout();

			queuePanel.Visible = false;
			Height -= queuePanel.Height + 14;
			// let the queue panel (below the form) be shifted vertically with the form
			queuePanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			// let the main panel's size increase with the form
			mainPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

			ResumeLayout();
		}

		private void ShowQueuePanel()
		{
			if (queuePanel.Visible)
				return;

			SuspendLayout();

			// fix the current height of the main panel
			mainPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

			// set the current height of the form (with hidden queue panel) as new minimum height
			var minSize = this.MinimumSize;
			minSize.Height = this.Height;
			this.MinimumSize = minSize;

			// increase the form's height to show the query panel and let its size increase with the form
			Height += queuePanel.Height + 14;
			queuePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			queuePanel.Visible = true;

			ResumeLayout();
		}


		private void pictureBox1_Click(object sender, EventArgs e)
		{
			label1.Text = string.Format("RoboMirror v{0}\nCopyright (c) Martin Kinkelin",
				Application.ProductVersion.TrimEnd('0', '.'));

			try { System.Diagnostics.Process.Start("http://robomirror.sourceforge.net/"); }
			catch { }
		}


		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			editButton.Enabled = removeButton.Enabled = historyButton.Enabled = scheduleButton.Enabled =
				backupButton.Enabled = restoreButton.Enabled =
					(listView1.SelectedIndices.Count > 0);
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
			var task = new MirrorTask();

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

			using (var dialog = new TaskDialog(task))
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
				dialog.ShowDialog(this);
		}


		private void scheduleButton_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count == 0)
				return;

			using (var dialog = new ScheduleTaskDialog(SelectedTask))
				dialog.ShowDialog(this);
		}


		private void backupButton_Click(object sender, EventArgs e)
		{
			StartOperation(reverse: false);
		}

		private void restoreButton_Click(object sender, EventArgs e)
		{
			StartOperation(reverse: true);
		}

		private void StartOperation(bool reverse)
		{
			var task = SelectedTask;
			if (task == null)
				return;

			var operation = new MirrorOperation(this, task, reverse);
			operation.Finished += (s, e) => { UpdateListViewItem(operation.Task); };

			mirrorOperationsQueueControl.Push(operation);

			ShowQueuePanel();
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
			var item = listView1.Items.Cast<ListViewItem>().FirstOrDefault(i => i.Tag == task);
			if (item == null)
				return;

			item.SubItems[1].Text = task.Source;
			item.SubItems[2].Text = task.Target;
			item.SubItems[3].Text = (task.LastOperation.HasValue ?
				task.LastOperation.Value.ToString("g") : "Never");
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

			if (!mirrorOperationsQueueControl.IsEmpty && e.CloseReason == CloseReason.UserClosing)
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
			mirrorOperationsQueueControl.AbortAll();

			_taskManager.Dispose();

			base.OnFormClosed(e);
		}


		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool InitiateSystemShutdownEx(
			string lpMachineName,
			string lpMessage,
			int dwTimeout,
			[MarshalAs(UnmanagedType.Bool)] bool bForceAppsClosed,
			[MarshalAs(UnmanagedType.Bool)] bool bRebootAfterShutdown,
			int dwReason);

		private void mirrorOperationsQueueControl_AllFinished(object sender, EventArgs e)
		{
			if (!shutdownWhenDoneCheckBox.Checked)
				return;

			const string message = "All RoboMirror operations have finished - shutting down as requested.";
			const int timeoutSeconds = 60;

			if (TokenPrivilegesAdjuster.Enable("SeShutdownPrivilege"))
				InitiateSystemShutdownEx(null, message, timeoutSeconds, false, false, 0);
		}
	}
}

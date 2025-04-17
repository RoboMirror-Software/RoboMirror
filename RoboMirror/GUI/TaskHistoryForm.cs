/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace RoboMirror.GUI
{
	/// <summary>
	/// Displays the history of a mirror task, based on its event log entries.
	/// </summary>
	public partial class TaskHistoryForm : BaseForm
	{
		/// <summary>
		/// Creates a new history form.
		/// </summary>
		public TaskHistoryForm(MirrorTask task)
		{
			if (task == null)
				throw new ArgumentNullException("task");

			InitializeComponent();

			var entries = Log.LoadEntries(task.Guid);

			foreach (var entry in entries)
			{
				var item = new ListViewItem();

				item.Tag = entry;

				if (entry.Type == EventLogEntryType.Information)
					item.ImageIndex = 0;
				else if (entry.Type == EventLogEntryType.Warning)
					item.ImageIndex = 1;
				else
					item.ImageIndex = 2;

				item.SubItems.Add(entry.TimeStamp.ToString("g"));
				item.SubItems.Add(entry.Message);

				listView1.Items.Add(item);
			}
		}

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count != 1)
				return;

			var entry = (Log.LogEntry)listView1.SelectedItems[0].Tag;
			if (string.IsNullOrEmpty(entry.Data))
				return;

			using (var dialog = new LogForm("Robocopy log", entry.Data))
			{
				dialog.ShowDialog(this);
			}
		}

		private void listView1_Resize(object sender, EventArgs e)
		{
			listView1.Columns[2].Width = listView1.Width - 140;
		}
	}
}

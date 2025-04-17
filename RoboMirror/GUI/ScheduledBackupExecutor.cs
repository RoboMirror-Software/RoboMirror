/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Linq;
using System.Windows.Forms;

namespace RoboMirror.GUI
{
	/// <summary>
	/// Executes a scheduled backup in the background while still
	/// providing a minimal GUI (a task tray icon).
	/// The form itself will be hidden and is only used for proper process
	/// lifetime management (incl. Windows logoff/shutdown/restart).
	/// </summary>
	public partial class ScheduledBackupExecutor : Form
	{
		#region Status class
		private class Status : IMirrorOperationStatus
		{
			// number of milliseconds after which a balloon tip will fade out automatically
			private const int BALLOON_TIMEOUT = 20000;

			private readonly ScheduledBackupExecutor _executor;

			public bool IsAbortingSupported { set { _executor.abortToolStripMenuItem.Enabled = value; } }

			public double Percentage
			{
				set
				{
					string percentageSuffix = (value >= 100 ? string.Empty
						: string.Format(" ({0}%)", value.ToString("f1")));

					_executor.destinationToolStripMenuItem.Text = string.Format("To: {0}{1}",
						_executor._operation.DestinationFolder, percentageSuffix);

					_executor.notifyIcon.Text = "RoboMirroring..." + percentageSuffix;
				}
			}

			public Status(ScheduledBackupExecutor executor)
			{
				_executor = executor;
			}

			public void OnEnterNewStage(string stage, string text)
			{
				_executor.notifyIcon.ShowBalloonTip(BALLOON_TIMEOUT, stage, text, ToolTipIcon.Info);
			}
		}
		#endregion

		private MirrorOperation _operation;

		/// <param name="guid">GUID of the task to be backed up.</param>
		public ScheduledBackupExecutor(string guid)
		{
			InitializeComponent();

			notifyIcon.Icon = Properties.Resources.data_copy_Icon;

			MirrorTask task = null;
			using (var taskManager = new TaskManager(readOnly: true))
				task = taskManager.LoadTask(guid);

			if (task == null)
				throw new InvalidOperationException("The task does not exist in the XML file.");

			_operation = new MirrorOperation(this, task, reverse: false);
			_operation.Finished += OnOperationFinished;
		}

		protected override void OnLoad(EventArgs e)
		{
			destinationToolStripMenuItem.Text = string.Format("To: {0}", _operation.DestinationFolder);
			notifyIcon.Visible = true;

			_operation.Start(new Status(this), simulateFirst: false);

			base.OnLoad(e);
		}

		protected override void OnClosed(EventArgs e)
		{
			_operation.Abort();
			base.OnClosed(e);
		}

		private void OnOperationFinished(object sender, FinishedEventArgs e)
		{
			Close();
		}

		private void abortToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_operation.Abort();
		}
	}
}

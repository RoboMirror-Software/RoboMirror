/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Windows.Forms;

namespace RoboMirror
{
	/// <summary>
	/// Encapsulates a backup or restore operation.
	/// It provides some minimal GUI through a task tray icon and manages
	/// output logging and error handling.
	/// It is designed as Control only because it offers a neat synchronization
	/// mechanism in combination with the SmartEventInvoker: the local event
	/// handlers are invoked in the thread which created the
	/// MirrorOperation instance. This allows for a single shared GUI thread.
	/// </summary>
	public sealed class MirrorOperation : Control
	{
		// number of milliseconds after which a balloon tip will fade out
		// automatically
		private static int BALLOON_TIMEOUT = 10000;

		// number of milliseconds after which a completed operation finishes
		// this time may be used by the user to view the log
		private static int OPERATION_DELAY = 60000;


		#region Fields.

		// current/last Robocopy process
		private RobocopyProcess _process;

		// for a very rough progress estimation based on the number of output
		// lines, used when a simulation is performed first:
		private int _expectedOutputLinesCount, _currentOutputLinesCount;

		// are we performing a backup or a restore operation?
		private bool _reverse;
		// real source and destination folders
		private string _source, _destination;

		// tray icon
		private NotifyIcon _icon;

		// timer used to finish the operation after a delay
		private Timer _timer;

		#endregion


		/// <summary>
		/// Gets the task associated with the operation.
		/// </summary>
		public MirrorTask Task { get; private set; }


		/// <summary>
		/// Fired either if the folders have already been in sync or when the
		/// destination folder has been synchronized successfully.
		/// If a backup has been performed, the LastBackup property of the
		/// associated task has been updated.
		/// </summary>
		public event EventHandler Succeeded;

		/// <summary>
		/// Fired when the operation has finished and the application may be
		/// closed.
		/// </summary>
		public event EventHandler Finished;


		/// <summary>
		/// Creates a new MirrorOperation.
		/// </summary>
		/// <param name="task">Associated task.</param>
		/// <param name="reverse">
		/// Indicates whether source and target are to be swapped, i.e.
		/// whether this will be a restore or backup operation.
		/// </param>
		public MirrorOperation(MirrorTask task, bool reverse)
			: base()
		{
			if (task == null)
				throw new ArgumentNullException("task");

			Task = task;

			_reverse = reverse;

			_source = (_reverse ? task.Target : task.Source);
			_destination = (_reverse ? task.Source : task.Target);

			_icon = new NotifyIcon();
			_icon.Icon = Properties.Resources.data_copy_Icon;
			_icon.Text = string.Format("RoboMirroring...");

			_icon.ContextMenuStrip = new ContextMenuStrip();

			_icon.ContextMenuStrip.Items.Add("Destination: " + _destination);
			_icon.ContextMenuStrip.Items[0].Enabled = false;

			_icon.ContextMenuStrip.Items.Add("Abort", Properties.Resources.delete,
				AbortToolStripItem_Clicked);

			EnableAborting(false);

			// create the control, otherwise it cannot be used as synchronization
			// mechanism (it is based on the control's handle)
			base.CreateControl();
		}

		/// <summary>
		/// Disposes of the operation.
		/// </summary>
		/// <param name="disposing">
		/// False if the destructor is calling the method, true if Dispose() is
		/// the caller.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if (_process != null)
			{
				_process.Dispose();
				_process = null;
			}

			if (disposing && _icon != null)
			{
				if (_timer != null)
					_timer.Dispose();

				_icon.ContextMenuStrip.Dispose();
				_icon.Dispose();

				_icon = null;
			}

			base.Dispose(disposing);
		}


		/// <summary>
		/// Starts the operation.
		/// </summary>
		/// <param name="simulateFirst">
		/// Indicates whether a "preview run" is to be performed before the
		/// actual operation. This is used to identify the pending changes to
		/// prompt the user for confirmation and enable some rough progress
		/// estimation for the actual operation.
		/// </param>
		public void Start(bool simulateFirst)
		{
			if (_process != null)
				throw new InvalidOperationException("The operation has already been started.");

			_icon.Visible = true;

			try
			{
				_process = new RobocopyProcess(Task, _reverse);

				if (simulateFirst)
				{
					_process.WrappedProcess.StartInfo.Arguments += " /l";

					_process.Started += new EventHandler(SimulationProcess_Started);
					_process.Exited += new EventHandler(SimulationProcess_Exited);

					_process.Start();
				}
				else
					StartRealProcess();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Robocopy cannot be started",
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				OnFinished(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Starts the real (non-simulation) Robocopy process.
		/// This method takes care of possibly embedding the process in a
		/// volume shadow copy session.
		/// </summary>
		private void StartRealProcess()
		{
			_process.Started += new EventHandler(RealProcess_Started);
			_process.Exited += new EventHandler(RealProcess_Exited);

			// keep track of the progress if a simulation has been performed first
			if (_expectedOutputLinesCount > 0)
			{
				_process.LineWritten +=
					new EventHandler<System.Diagnostics.DataReceivedEventArgs>(RealProcess_LineWritten);
			}

			if (Task.UseVolumeShadowCopy && !_reverse)
			{
				_icon.ShowBalloonTip(BALLOON_TIMEOUT, "Preparing...",
					"The volume shadow copy is being created.", ToolTipIcon.Info);

				_process.StartInVscSession(VscSession_Aborted);
			}
			else
				_process.Start();
		}


		#region Process event handlers.

		/// <summary>
		/// Invoked when a simulation process has been started.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SimulationProcess_Started(object sender, EventArgs e)
		{
			_icon.ShowBalloonTip(BALLOON_TIMEOUT, "Analyzing...",
				"Pending changes are being identified.", ToolTipIcon.Info);

			EnableAborting(true);
		}

		/// <summary>
		/// Invoked when a simulation process has exited.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SimulationProcess_Exited(object sender, EventArgs e)
		{
			EnableAborting(false);

			int exitCode = CheckExitCode();

			// finish if the user has aborted or if Robocopy could not be
			// started normally
			if (exitCode == -1 || (exitCode & 16) != 0)
			{
				OnFinished(EventArgs.Empty);
				return;
			}

			if (exitCode == 0)
			{
				OnAlreadyInSync();
				return;
			}

			// prompt the user to commit the pending changes
			using (var dialog = new SimulationResultDialog(_process, _destination))
			{
				if (dialog.ShowDialog() != DialogResult.OK)
				{
					OnFinished(EventArgs.Empty);
					return;
				}
			}

			_expectedOutputLinesCount = _process.Output.Count;
			_process.Dispose();

			try
			{
				_process = new RobocopyProcess(Task, _reverse);

				StartRealProcess();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message, "Robocopy cannot be started",
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				OnFinished(EventArgs.Empty);
			}
		}


		/// <summary>
		/// Invoked when a real (non-simulation) process has been started.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RealProcess_Started(object sender, EventArgs e)
		{
			_icon.ShowBalloonTip(BALLOON_TIMEOUT, "Mirroring...",
				string.Format("to \"{0}\"", _destination), ToolTipIcon.Info);

			EnableAborting(true);
		}

		/// <summary>
		/// Invoked when a real (non-simulation) process has written a
		/// line to stdout or stderr.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RealProcess_LineWritten(object sender, System.Diagnostics.DataReceivedEventArgs e)
		{
			_currentOutputLinesCount++;

			double expectedPercentage = (100.0 * _currentOutputLinesCount) / _expectedOutputLinesCount;

			if (expectedPercentage < 100)
			{
				_icon.Text = string.Format("RoboMirroring... ({0}%)",
					expectedPercentage.ToString("F1"));
			}
			else
				_icon.Text = "RoboMirroring...";
		}

		/// <summary>
		/// Invoked when a real (non-simulation) process has exited.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RealProcess_Exited(object sender, EventArgs e)
		{
			EnableAborting(false);

			_icon.Text = "RoboMirrored";

			Log.LogRun(_process, _source, _destination);

			int exitCode = CheckExitCode();

			// could the process be started normally?
			if (exitCode == -1 || (exitCode & 16) == 0)
			{
				// show a summary balloon tip

				if (exitCode == 0)
				{
					OnAlreadyInSync();
				}
				else if (exitCode == -1)
				{
					OnSynchronized(ToolTipIcon.Warning, "Mirroring aborted",
						"Click here to view the log.");
				}
				else if ((exitCode & 8) != 0)
				{
					OnSynchronized(ToolTipIcon.Error, "Mirroring incomplete",
						"Some items could not be copied. Click here to view the log.");
				}
				else
				{
					OnSynchronized(ToolTipIcon.Info, "Mirroring complete",
						"Click here to view the log.");
				}
			}
			else
				OnFinished(EventArgs.Empty);
		}

		#endregion


		/// <summary>
		/// Invoked if the volume shadow copy could not be created/mounted
		/// or if the embedded Robocopy process could not be started.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VscSession_Aborted(object sender, TextEventArgs e)
		{
			MessageBox.Show(e.Text, "Volume shadow copy cannot be created",
				MessageBoxButtons.OK, MessageBoxIcon.Error);

			OnFinished(EventArgs.Empty);
		}


		/// <summary>
		/// Enables or disables the abort context menu item.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableAborting(bool enable)
		{
			_icon.ContextMenuStrip.Items[1].Enabled = enable;
		}

		/// <summary>
		/// Prompts the user to abort the current operation.
		/// </summary>
		/// <returns>True if the operation has been aborted.</returns>
		public bool Abort()
		{
			// is the operation not running?
			if (_process == null || _process.HasExited)
				return false;

			// ask the user for confirmation and check if the process has
			// exited in the meantime
			if (MessageBox.Show(string.Format("Are you sure you want to abort mirroring to \"{0}\"?", _destination),
				"Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) !=
				DialogResult.Yes || _process.HasExited)
			{
				return false;
			}

			_process.Kill();

			return true;
		}


		/// <summary>
		/// Checks the exit code of a Robocopy process and displays a message box if
		/// it could not be started normally.
		/// </summary>
		/// <returns>The exit code.</returns>
		private int CheckExitCode()
		{
			int exitCode = _process.ExitCode;

			if (exitCode != -1 && (exitCode & 16) != 0)
			{
				if (MessageBox.Show("Robocopy has terminated unexpectedly.\nWould you like to view the log?",
					"Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
				{
					using (var form = new LogForm("Robocopy log", _process.FullOutput))
					{
						form.ShowDialog();
					}
				}
			}

			return exitCode;
		}


		/// <summary>
		/// Invoked if the folders have already been in sync.
		/// </summary>
		private void OnAlreadyInSync()
		{
			_icon.ShowBalloonTip(BALLOON_TIMEOUT, "Mirroring complete",
				"The folders are already in sync.", ToolTipIcon.Info);

			OnCompleted(true);
		}

		/// <summary>
		/// Invoked when the destination folder has been synchronized,
		/// at least partially.
		/// </summary>
		/// <param name="icon">Icon of the balloon tip.</param>
		/// <param name="title">Title of the balloon tip.</param>
		/// <param name="text">Text of the balloon tip.</param>
		private void OnSynchronized(ToolTipIcon icon, string title, string text)
		{
			_icon.ShowBalloonTip(BALLOON_TIMEOUT, title, text, icon);

			_icon.BalloonTipClicked += new EventHandler(ViewLogToolStripItem_Clicked);

			OnCompleted(icon == ToolTipIcon.Info);
		}

		/// <summary>
		/// Invoked either if the folders have already been in sync or when
		/// the destination folder has been synchronized, at least partially.
		/// </summary>
		/// <param name="success">Indicates if the operation was successful.</param>
		private void OnCompleted(bool success)
		{
			_icon.Text = "RoboMirrored";

			_icon.ContextMenuStrip.Items.RemoveAt(1);
			_icon.ContextMenuStrip.Items.Add("View log", Properties.Resources.text,
				ViewLogToolStripItem_Clicked);
			_icon.ContextMenuStrip.Items.Add("Close", Properties.Resources.delete,
				CloseToolStripItem_Clicked);

			// use a timer to finish the operation
			if (OPERATION_DELAY > 0)
			{
				_timer = new Timer();
				_timer.Interval = OPERATION_DELAY;
				_timer.Tick += new EventHandler(Timer_Tick);
				_timer.Start();
			}

			if (success)
				OnSucceeded(EventArgs.Empty);
		}

		/// <summary>
		/// Invoked when the operation has been completed successfully.
		/// It is not yet finished though because the user is given some
		/// time to view the log.
		/// </summary>
		/// <param name="e"></param>
		private void OnSucceeded(EventArgs e)
		{
			// update the last backup timestamp
			if (!_reverse)
				Task.LastBackup = DateTime.Now;

			if (Succeeded != null)
				Succeeded(this, e);
		}

		/// <summary>
		/// Fires the Finished event after disposing of the operation.
		/// </summary>
		/// <param name="e"></param>
		private void OnFinished(EventArgs e)
		{
			Dispose();

			if (Finished != null)
				Finished(this, e);
		}


		/// <summary>
		/// Invoked when the user clicks on the abort context menu item.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AbortToolStripItem_Clicked(object sender, EventArgs e)
		{
			Abort();
		}

		/// <summary>
		/// Invoked when the user clicks on the view log context menu item.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewLogToolStripItem_Clicked(object sender, EventArgs e)
		{
			using (var form = new LogForm("Robocopy log", _process.FullOutput))
			{
				_icon.Visible = false;
				form.ShowDialog();
			}

			OnFinished(EventArgs.Empty);
		}

		/// <summary>
		/// Invoked when the user clicks on the close context menu item.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CloseToolStripItem_Clicked(object sender, EventArgs e)
		{
			OnFinished(EventArgs.Empty);
		}


		/// <summary>
		/// Invoked when the summary balloon tip has faded out automatically.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Timer_Tick(object sender, EventArgs e)
		{
			OnFinished(EventArgs.Empty);
		}
	}
}

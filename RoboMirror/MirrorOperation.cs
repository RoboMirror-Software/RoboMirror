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
	#region FinishedEventArgs class.

	/// <summary>
	/// EventArgs derivate for the MirrorOperation's Finished event.
	/// </summary>
	public class FinishedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets a value indicating whether the operation was successful,
		/// i.e. whether both folders have already been in sync or the
		/// destination folder has been synchronized successfully.
		/// </summary>
		public bool Success { get; private set; }


		/// <summary>
		/// Creates a new FinishedEventArgs.
		/// </summary>
		public FinishedEventArgs(bool success)
		{
			Success = success;
		}
	}

	#endregion


	/// <summary>
	/// Encapsulates a backup or restore operation.
	/// It provides some minimal GUI through a task tray icon and manages
	/// output logging and error handling.
	/// It is designed as Control only because it offers a neat synchronization
	/// mechanism in combination with the SmartEventInvoker: the local event
	/// handlers are invoked in the thread which created the MirrorOperation
	/// instance. This allows for a single shared GUI thread.
	/// </summary>
	public sealed class MirrorOperation : Control
	{
		// number of milliseconds after which a balloon tip will fade out
		// automatically
		private const int BALLOON_TIMEOUT = 10000;

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

		#endregion


		/// <summary>
		/// Gets the task associated with the operation.
		/// </summary>
		public MirrorTask Task { get; private set; }


		/// <summary>
		/// Fired when the operation has finished.
		/// If a backup has been completed successfully, the LastBackup
		/// property of the associated task has been updated.
		/// </summary>
		public event EventHandler<FinishedEventArgs> Finished;


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
			if (disposing)
			{
				if (_process != null)
				{
					_process.Dispose();
					_process = null;
				}

				if (_icon != null)
				{
					_icon.ContextMenuStrip.Dispose();
					_icon.Dispose();
					_icon = null;
				}
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

					_process.Started += SimulationProcess_Started;
					_process.Exited += SimulationProcess_Exited;

					_process.Start();
				}
				else
					StartRealProcess();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "RoboMirror", MessageBoxButtons.OK, MessageBoxIcon.Error);

				OnFinished(new FinishedEventArgs(false));
			}
		}

		/// <summary>
		/// Starts the real (non-simulation) Robocopy process.
		/// This method takes care of possibly embedding the process in a
		/// volume shadow copy session.
		/// </summary>
		private void StartRealProcess()
		{
			_process.Started += RealProcess_Started;
			_process.Exited += RealProcess_Exited;

			// keep track of the progress if a simulation has been performed first
			if (_expectedOutputLinesCount > 0)
			{
				_process.LineWritten += RealProcess_LineWritten;
			}

			if (Task.UseVolumeShadowCopy)
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
		private void SimulationProcess_Started(object sender, EventArgs e)
		{
			_icon.ShowBalloonTip(BALLOON_TIMEOUT, "Analyzing...",
				"Pending changes are being identified.", ToolTipIcon.Info);

			EnableAborting(true);
		}

		/// <summary>
		/// Invoked when a simulation process has exited.
		/// </summary>
		private void SimulationProcess_Exited(object sender, EventArgs e)
		{
			EnableAborting(false);

			int exitCode = CheckExitCode();

			// finish if the user has aborted or if Robocopy could not be
			// started normally
			if (exitCode == -1 || (exitCode & 16) != 0)
			{
				OnFinished(new FinishedEventArgs(false));
				return;
			}

			// prompt the user to commit the pending changes
			using (var dialog = new SimulationResultDialog(_process, _destination))
			{
				if (dialog.ShowDialog() != DialogResult.OK)
				{
					OnFinished(new FinishedEventArgs(false));
					return;
				}
			}

			_expectedOutputLinesCount = _process.Output.Count;
			_process.Dispose();
			_process = null;

			try
			{
				_process = new RobocopyProcess(Task, _reverse);

				StartRealProcess();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message, "RoboMirror", MessageBoxButtons.OK, MessageBoxIcon.Error);

				OnFinished(new FinishedEventArgs(false));
			}
		}


		/// <summary>
		/// Invoked when a real (non-simulation) process has been started.
		/// </summary>
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
		private void RealProcess_Exited(object sender, EventArgs e)
		{
			EnableAborting(false);

			_icon.Text = "RoboMirrored";

			Log.LogRun(_process, Task, _reverse);

			int exitCode = CheckExitCode();

			bool success = (exitCode != -1 && (exitCode & 16) == 0 && (exitCode & 8) == 0);

			OnFinished(new FinishedEventArgs(success));
		}

		#endregion


		/// <summary>
		/// Invoked if the volume shadow copy could not be created/mounted
		/// or if the embedded Robocopy process could not be started.
		/// </summary>
		private void VscSession_Aborted(object sender, TextEventArgs e)
		{
			MessageBox.Show(e.Text, "RoboMirror", MessageBoxButtons.OK, MessageBoxIcon.Error);

			OnFinished(new FinishedEventArgs(false));
		}

		/// <summary>
		/// Invoked when the user clicks on the abort context menu item.
		/// </summary>
		private void AbortToolStripItem_Clicked(object sender, EventArgs e)
		{
			Abort();
		}


		/// <summary>
		/// Enables or disables the abort context menu item.
		/// </summary>
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
				"RoboMirror", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) !=
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

			if (exitCode != -1)
			{
				if ((exitCode & 16) != 0)
				{
					if (MessageBox.Show("A fatal Robocopy error has occurred.\nWould you like to view the log?",
						"RoboMirror", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
					{
						using (var form = new LogForm("Robocopy log", _process.FullOutput))
						{
							form.ShowDialog();
						}
					}
				}
				else if ((exitCode & 8) != 0)
				{
					if (MessageBox.Show("Some files could not be copied.\nWould you like to view the log?",
						"RoboMirror", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
					{
						using (var form = new LogForm("Robocopy log", _process.FullOutput))
						{
							form.ShowDialog();
						}
					}
				}
			}

			return exitCode;
		}


		/// <summary>
		/// Fires the Finished event after disposing of the operation.
		/// </summary>
		private void OnFinished(FinishedEventArgs e)
		{
			if (e.Success && !_reverse)
				Task.LastBackup = DateTime.Now;

			Dispose();

			if (Finished != null)
				Finished(this, e);
		}
	}
}

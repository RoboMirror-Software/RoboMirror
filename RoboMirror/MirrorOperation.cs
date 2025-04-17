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

		// are we performing a backup or a restore operation?
		private bool _reverse;

		// current/last Robocopy process
		private RobocopyProcess _process;

		// for a very rough progress estimation based on the number of output
		// lines, used when a simulation is performed first:
		private int _expectedOutputLinesCount, _currentOutputLinesCount;

		// tray icon
		private NotifyIcon _icon;


		/// <summary>
		/// Gets the task associated with the operation.
		/// </summary>
		public MirrorTask Task { get; private set; }

		private string DestinationFolder { get { return _reverse ? Task.Source : Task.Target; } }


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

			_icon = new NotifyIcon();
			_icon.Icon = Properties.Resources.data_copy_Icon;
			_icon.Text = string.Format("RoboMirroring...");

			_icon.ContextMenuStrip = new ContextMenuStrip();

			_icon.ContextMenuStrip.Items.Add("Destination: " + DestinationFolder);
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
					_process.StartInfo.Arguments += " /l";

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
				_process.LineWritten += RealProcess_LineWritten;

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

			bool aborted = (_process.ExitCode == -1);

			// alert if Robocopy could not be started normally
			bool fatalError = (!aborted && _process.IsAnyExitFlagSet(RobocopyExitCodes.FatalError));
			if (fatalError)
				Alert("A fatal Robocopy error has occurred.", MessageBoxIcon.Error);

			if (aborted || fatalError)
			{
				OnFinished(new FinishedEventArgs(false));
				return;
			}

			// prompt the user to commit the pending changes
			using (var dialog = new SimulationResultDialog(_process))
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
				string.Format("to {0}", PathHelper.Quote(DestinationFolder)), ToolTipIcon.Info);

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

			_icon.Text = (expectedPercentage < 100
				? string.Format("RoboMirroring... ({0}%)", expectedPercentage.ToString("F1"))
				: "RoboMirroring...");
		}

		/// <summary>
		/// Invoked when a real (non-simulation) process has exited.
		/// </summary>
		private void RealProcess_Exited(object sender, EventArgs e)
		{
			EnableAborting(false);

			_icon.Text = "RoboMirrored";

			try
			{
				Log.LogRun(_process, Task);
			}
			catch (Exception exception)
			{
				MessageBox.Show("The mirror operation could not be logged.\n\n" + exception.Message,
					"RoboMirror", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			bool success = CheckExitCode();

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
			if (MessageBox.Show(string.Format("Are you sure you want to abort mirroring to {0}?", PathHelper.Quote(DestinationFolder)),
				"RoboMirror", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) !=
				DialogResult.Yes || _process.HasExited)
			{
				return false;
			}

			_process.Kill();

			return true;
		}


		/// <summary>
		/// Checks the exit code of the Robocopy process and informs the user
		/// if the operation was not successful.
		/// </summary>
		/// <returns>True if the operation was successful.</returns>
		private bool CheckExitCode()
		{
			if (_process.ExitCode == -1) // aborted?
				return false;

			if (_process.IsAnyExitFlagSet(RobocopyExitCodes.FatalError))
			{
				Alert("A fatal Robocopy error has occurred.", MessageBoxIcon.Error);
				return false;
			}

			if (_process.IsAnyExitFlagSet(RobocopyExitCodes.CopyErrors))
			{
				Alert("Some items could not be mirrored.", MessageBoxIcon.Error);
				return false;
			}

			if (_process.IsAnyExitFlagSet(RobocopyExitCodes.MismatchedItems))
			{
				Alert("There were some file <-> folder mismatches.", MessageBoxIcon.Warning);
				return false;
			}

			return true;
		}

		private void Alert(string text, MessageBoxIcon icon)
		{
			if (MessageBox.Show(text + "\nWould you like to view the log?", "RoboMirror", MessageBoxButtons.YesNo, icon) == DialogResult.Yes)
			{
				using (var form = new LogForm("Robocopy log", _process.FullOutput))
				{
					form.ShowDialog();
				}
			}
		}


		/// <summary>
		/// Fires the Finished event after disposing of the operation.
		/// </summary>
		private void OnFinished(FinishedEventArgs e)
		{
			if (e.Success)
				Task.LastOperation = DateTime.Now;

			Dispose();

			if (Finished != null)
				Finished(this, e);
		}
	}
}

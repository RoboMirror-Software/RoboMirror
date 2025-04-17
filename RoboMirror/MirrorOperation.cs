/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace RoboMirror
{
	#region FinishedEventArgs class
	/// <summary>
	/// EventArgs derivate for the MirrorOperation's Finished event.
	/// </summary>
	public class FinishedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets a value indicating whether the operation was successful,
		/// i.e., whether both folders have already been in sync or the
		/// destination folder has been synchronized successfully.
		/// </summary>
		public bool Success { get; private set; }

		public FinishedEventArgs(bool success)
		{
			Success = success;
		}
	}
	#endregion

	#region IMirrorOperationStatus interface
	public interface IMirrorOperationStatus
	{
		bool IsAbortingSupported { set; }
		double Percentage { set; }

		void OnEnterNewStage(string stage, string text);
	}
	#endregion

	/// <summary>
	/// Encapsulates a backup or restore operation.
	/// It provides some minimal GUI through a task tray icon and manages
	/// output logging and error handling.
	/// </summary>
	public sealed class MirrorOperation : IDisposable, ISynchronizeInvoke
	{
		// all ISynchronizeInvoke calls will be forwarded to a control acting as
		// synchronization object
		// ISynchronizeInvoke offers a neat synchronization mechanism in
		// combination with the SmartEventInvoker: all event handlers defined
		// as non-static methods of an ISynchronizeInvoke implementation are
		// invoked in the associated thread (either directly or via dispatching)
		private readonly ISynchronizeInvoke _control;

		// are we performing a backup or a restore operation?
		private readonly bool _reverse;

		private IMirrorOperationStatus _status;

		private VolumeShadowCopySession _vscSession;

		// current/last Robocopy process
		private RobocopyProcess _process;
		int _simulationProcessOutputLinesCount = -1;


		public MirrorTask Task { get; private set; }

		public string SourceFolder { get; private set; }
		public string DestinationFolder { get; private set; }

		public bool HasStarted { get; private set; }
		public bool IsFinished { get; private set; }


		/// <summary>
		/// Fired when the operation has finished.
		/// If the operation has been completed successfully, the LastOperation
		/// property of the associated task has been updated.
		/// </summary>
		public event EventHandler<FinishedEventArgs> Finished;


		/// <param name="control">Synchronization object.</param>
		/// <param name="reverse">
		/// Indicates whether source and target folders are to be swapped,
		/// i.e., whether this is a restore or backup operation.
		/// </param>
		public MirrorOperation(ISynchronizeInvoke control, MirrorTask task, bool reverse)
		{
			if (control == null)
				throw new ArgumentNullException("control");
			if (task == null)
				throw new ArgumentNullException("task");

			_control = control;
			Task = task;
			_reverse = reverse;

			UpdateFolders();
		}

		private void UpdateFolders()
		{
			SourceFolder = (!_reverse ? Task.Source : Task.Target);
			DestinationFolder = (!_reverse ? Task.Target : Task.Source);
		}


		/// <param name="simulateFirst">
		/// Indicates whether a simulation run is to be performed before the
		/// actual operation. This is used to identify the pending changes,
		/// prompt the user for confirmation and enable some rough progress
		/// estimation for the actual operation.
		/// </param>
		public void Start(IMirrorOperationStatus status, bool simulateFirst)
		{
			if (status == null)
				throw new ArgumentNullException("status");

			if (IsFinished)
				throw new InvalidOperationException("The operation has already finished.");
			if (HasStarted)
				throw new InvalidOperationException("The operation has already been started.");

			_status = status;
			_status.IsAbortingSupported = false;

			UpdateFolders();
			HasStarted = true;

			if (simulateFirst)
				StartSimulationProcess();
			else
				LaunchActualOperation();
		}


		public void Dispose()
		{
			if (_process != null)
			{
				_process.Dispose(); // incl. killing it if currently running + waiting for it to exit and the Exited event handler to complete
				_process = null;
			}

			if (_vscSession != null)
			{
				_vscSession.Dispose();
				_vscSession = null;
			}
		}

		/// <summary>
		/// Finishes the operation by disposing of it and firing the Finished event.
		/// </summary>
		private void Finish(bool success)
		{
			if (IsFinished || !HasStarted)
				return;

			IsFinished = true;

			Dispose();

			if (Finished != null)
				Finished(this, new FinishedEventArgs(success));
		}


		#region Simulation process

		private void StartSimulationProcess()
		{
			_process = new RobocopyProcess(Task, SourceFolder, DestinationFolder);
			_process.StartInfo.Arguments += " /l";
			_process.Exited += SimulationProcess_Exited;

			if (!TryStartRobocopy(_process))
				return;

			_status.OnEnterNewStage("Analyzing...", "Pending changes are being identified...");
			_status.IsAbortingSupported = true;
		}

		private void SimulationProcess_Exited(object sender, EventArgs e)
		{
			_status.IsAbortingSupported = false;

			bool aborted = (_process.ExitCode == -1 || IsFinished);

			// alert if Robocopy could not be started normally
			bool fatalError = (!aborted && _process.IsAnyExitFlagSet(RobocopyExitCodes.FatalError));
			if (fatalError)
				Alert("A fatal Robocopy error has occurred.", MessageBoxIcon.Error);

			if (aborted || fatalError)
			{
				Finish(false);
				return;
			}

			// prompt the user to commit the pending changes
			using (var dialog = new GUI.SimulationResultDialog(_process))
			{
				if (dialog.ShowDialog() != DialogResult.OK)
				{
					Finish(false);
					return;
				}
			}

			_simulationProcessOutputLinesCount = _process.Output.Count;
			_process.Dispose();
			_process = null;

			LaunchActualOperation();
		}

		#endregion

		private void LaunchActualOperation()
		{
			if (Task.UseVolumeShadowCopy)
				StartInVscSession();
			else
				StartProcess(SourceFolder);
		}

		#region Volume shadow copy

		private void StartInVscSession()
		{
			string sourceVolume = PathHelper.RemoveTrailingSeparator(Path.GetPathRoot(SourceFolder));

			_vscSession = new VolumeShadowCopySession();
			_vscSession.Error += VscSession_Error;
			_vscSession.Ready += VscSession_Ready;

			_status.OnEnterNewStage("Preparing...", string.Format("Creating shadow copy of volume {0} ...", PathHelper.Quote(sourceVolume)));

			// create and mount the shadow copy
			_vscSession.Start(SourceFolder);

			_status.IsAbortingSupported = true;
		}

		/// <summary>
		/// Invoked if the volume shadow copy could not be created/mounted.
		/// </summary>
		private void VscSession_Error(object sender, TextEventArgs e)
		{
			_status.IsAbortingSupported = false;

			MessageBox.Show(e.Text, "RoboMirror", MessageBoxButtons.OK, MessageBoxIcon.Error);

			Finish(false);
		}

		/// <summary>
		/// Invoked when the volume shadow copy has been created and mounted.
		/// </summary>
		private void VscSession_Ready(object sender, EventArgs e)
		{
			_status.IsAbortingSupported = false;

			if (!IsFinished)
				StartProcess(_vscSession.MountPoint);
		}

		#endregion

		#region Actual (non-simulation) process

		/// <summary>Starts the actual (non-simulation) Robocopy process.</summary>
		private void StartProcess(string sourceFolder)
		{
			_process = new RobocopyProcess(Task, sourceFolder, DestinationFolder, _simulationProcessOutputLinesCount);
			_process.Exited += Process_Exited;
			if (_simulationProcessOutputLinesCount > 0)
				_process.ProgressChanged += Process_ProgressChanged;

			if (!TryStartRobocopy(_process))
				return;

			_status.OnEnterNewStage("Mirroring...", string.Format("to {0}", PathHelper.Quote(DestinationFolder)));
			_status.IsAbortingSupported = true;
		}

		private void Process_ProgressChanged(object sender, ProgressEventArgs e)
		{
			_status.Percentage = e.Percentage;
		}

		private void Process_Exited(object sender, EventArgs e)
		{
			_status.IsAbortingSupported = false;

			bool success = CheckExitCode();

			try
			{
				var entry = Log.LogRun(Task.Guid, _process, SourceFolder, DestinationFolder,
					updateLastSuccessTimeStamp: success);

				if (success)
					Task.LastOperation = entry.TimeStamp;
			}
			catch (Exception exception)
			{
				MessageBox.Show("The mirror operation could not be logged.\n\n" + exception.Message,
					"RoboMirror", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			Finish(success);
		}

		#endregion


		/// <summary>Aborts the operation if it is currently running.</summary>
		public void Abort()
		{
			if (HasStarted && !IsFinished)
				Finish(false);
		}


		/// <summary>
		/// Tries to start the specified process and returns true if successful.
		/// If the process could not be started, a message box is displayed
		/// and then the operation is finished.
		/// </summary>
		private bool TryStartRobocopy(RobocopyProcess process)
		{
			try
			{
				process.Start();
				return true;
			}
			catch (Exception e)
			{
				MessageBox.Show("Robocopy could not be started:\n\n" + e.Message,
					"RoboMirror", MessageBoxButtons.OK, MessageBoxIcon.Error);

				Finish(false);
				return false;
			}
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
				using (var form = new GUI.LogForm("Robocopy log", _process.FullOutput))
					form.ShowDialog();
			}
		}


		#region ISynchronizeInvoke (forwarding to _control only)

		bool ISynchronizeInvoke.InvokeRequired { get { return _control.InvokeRequired; } }

		IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
		{
			return _control.BeginInvoke(method, args);
		}

		object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
		{
			return _control.EndInvoke(result);
		}

		object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
		{
			return _control.Invoke(method, args);
		}

		#endregion
	}
}

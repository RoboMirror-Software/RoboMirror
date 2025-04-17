/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace RoboMirror
{
	/// <summary>
	/// Wraps a hidden command-line process writing to stdout and stderr.
	/// It manages the process output and provides some neat events:
	/// event handlers defined in UI controls will be invoked in the
	/// control's thread.
	/// This class is thread-safe.
	/// </summary>
	public class ConsoleProcess : IDisposable
	{
		#region Fields.

		protected readonly object _syncObject = new object();

		private bool _isDisposed;

		private List<string> _output = new List<string>();
		private string _fullOutput;

		#endregion

		#region Properties.

		/// <summary>
		/// Gets the actual process.
		/// </summary>
		public Process WrappedProcess { get; private set; }

		/// <summary>
		/// Gets the start info for the process.
		/// </summary>
		public ProcessStartInfo StartInfo
		{
			get { return WrappedProcess.StartInfo; }
		}

		/// <summary>
		/// Gets a value indicating whether the process has been started.
		/// This does not mean it is currently running, because it may
		/// have already exited.
		/// </summary>
		public bool HasStarted
		{
			get
			{
				IntPtr handle = IntPtr.Zero;

				try
				{
					handle = WrappedProcess.Handle;
				}
				catch (InvalidOperationException)
				{
					// the process has not been started yet
				}

				return (handle != IntPtr.Zero);
			}

		}

		/// <summary>
		/// Gets a value indicating whether the process has exited.
		/// </summary>
		public bool HasExited
		{
			get
			{
				try
				{
					return WrappedProcess.HasExited;
				}
				catch (InvalidOperationException)
				{
					// the process has not been started yet
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the exit code of the process.
		/// If the process has not exited yet, an exception is thrown.
		/// </summary>
		public int ExitCode
		{
			get
			{
				try
				{
					return WrappedProcess.ExitCode;
				}
				catch (InvalidOperationException)
				{
					throw new InvalidOperationException("The process has not exited yet.");
				}
			}
		}


		/// <summary>
		/// Gets the lines written to stdout and stderr.
		/// To save synchronization across threads, the output can
		/// only be accessed after the process has exited, otherwise
		/// an exception is thrown.
		/// Do not alter the list!
		/// </summary>
		public List<string> Output
		{
			get
			{
				if (!HasExited)
					throw new InvalidOperationException("The process has not exited yet.");

				return _output;
			}
		}

		/// <summary>
		/// Gets the full text written to stdout and stderr.
		/// The full output can only be accessed after the process has
		/// exited, otherwise an exception is thrown.
		/// </summary>
		public string FullOutput
		{
			get
			{
				// make sure that accessing the property is thread-safe
				lock (_syncObject)
				{
					// use the cache if possible
					if (_fullOutput != null)
						return _fullOutput;

					// check if the process has exited
					var lines = Output;

					var output = new StringBuilder(32768);

					foreach (string line in lines)
					{
						output.AppendLine(line);
					}

					_fullOutput = output.ToString();

					return _fullOutput;
				}
			}
		}

		#endregion

		#region Events.

		/// <summary>
		/// Fired when the process has been started.
		/// Invoked in the thread which started the process, except if the
		/// event handler is a method of a Control instance, in which case
		/// it will be invoked in the thread which created the Control.
		/// </summary>
		public event EventHandler Started;

		/// <summary>
		/// Fired when the process has written a line to stdout or stderr.
		/// Invoked asynchronously, except if the event handler is a method
		/// of a Control instance, in which case it will be invoked in the
		/// thread which created the control.
		/// </summary>
		public event EventHandler<DataReceivedEventArgs> LineWritten;

		/// <summary>
		/// Fired when the process has exited.
		/// Invoked asynchronously, except if the event handler is a method
		/// of a Control instance, in which case it will be invoked in the
		/// thread which created the control.
		/// </summary>
		public event EventHandler Exited;

		#endregion


		/// <summary>
		/// Creates a new ConsoleProcess.
		/// </summary>
		public ConsoleProcess() :
			this(null)
		{
		}

		/// <summary>
		/// Creates a new ConsoleProcess and sets the specified start info.
		/// </summary>
		/// <param name="startInfo">
		/// Optional start info for the new process.
		/// Some properties regarding stdout and stderr will be overwritten.
		/// </param>
		public ConsoleProcess(ProcessStartInfo startInfo)
		{
			WrappedProcess = new Process();

			if (startInfo != null)
				WrappedProcess.StartInfo = startInfo;

			SetupProcess();
		}

		/// <summary>
		/// Kills the process if it is currently running and releases all
		/// used resources.
		/// </summary>
		public virtual void Dispose()
		{
			lock (_syncObject)
			{
				if (!_isDisposed)
				{
					try
					{
						WrappedProcess.Kill();
					}
					catch (System.ComponentModel.Win32Exception)
					{
						// the process might just be terminating
					}
					catch (InvalidOperationException)
					{
						// the process has either not been started or already exited
					}

					WrappedProcess.Close();

					_isDisposed = true;
				}
			}
		}


		/// <summary>
		/// Sets the process up so that it is compatible with this class.
		/// </summary>
		private void SetupProcess()
		{
			// hide the process from the user
			StartInfo.UseShellExecute = false;
			StartInfo.CreateNoWindow = true;

			// redirect both output streams
			StartInfo.RedirectStandardOutput = StartInfo.RedirectStandardError = true;

			// set both streams' encoding to the OEM code page, usually used in the console
			StartInfo.StandardOutputEncoding = StartInfo.StandardErrorEncoding =
				Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.TextInfo.OEMCodePage);

			WrappedProcess.OutputDataReceived += new DataReceivedEventHandler(Process_DataReceived);
			WrappedProcess.ErrorDataReceived += new DataReceivedEventHandler(Process_DataReceived);
			WrappedProcess.Exited += new EventHandler(Process_Exited);

			// enable the exited event
			WrappedProcess.EnableRaisingEvents = true;
		}


		/// <summary>
		/// Starts the process asynchronously.
		/// Trying to restart a running or exited process results in an
		/// exception being thrown.
		/// </summary>
		public void Start()
		{
			lock (_syncObject)
			{
				if (HasStarted)
					throw new InvalidOperationException("The process has already been started.");

				WrappedProcess.Start();

				WrappedProcess.BeginOutputReadLine();
				WrappedProcess.BeginErrorReadLine();
			}

			OnStarted(EventArgs.Empty);
		}

		/// <summary>
		/// Kills the process asynchronously if it is currently running.
		/// </summary>
		public void Kill()
		{
			lock (_syncObject)
			{
				try
				{
					WrappedProcess.Kill();
				}
				catch (System.ComponentModel.Win32Exception)
				{
					// the process might just be terminating
				}
				catch (InvalidOperationException)
				{
					// the process has either not been started or already exited
				}
			}
		}


		/// <summary>
		/// Invoked asynchronously when the process has written a line to stdout or stderr.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Process_DataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null)
				return;

			lock (_syncObject)
			{
				_output.Add(e.Data);
			}

			OnLineWritten(e);
		}

		/// <summary>
		/// Invoked asynchronously when the process has exited.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Process_Exited(object sender, EventArgs e)
		{
			OnExited(e);
		}


		/// <summary>
		/// Invoked when the process has started.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnStarted(EventArgs e)
		{
			SmartEventInvoker.FireEvent(Started, this, e);
		}

		/// <summary>
		/// Invoked asynchronously when the process has written a line to stdout or stderr.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnLineWritten(DataReceivedEventArgs e)
		{
			SmartEventInvoker.FireEvent(LineWritten, this, e);
		}

		/// <summary>
		/// Invoked asynchronously when the process has exited.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnExited(EventArgs e)
		{
			SmartEventInvoker.FireEvent(Exited, this, e);
		}
	}
}

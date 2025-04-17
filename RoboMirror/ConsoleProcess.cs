/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace RoboMirror
{
	/// <summary>
	/// Wraps a hidden command-line process writing to stdout and stderr.
	/// It manages lifetime and output of the process and provides some
	/// neat events: event handlers defined in UI controls will be
	/// dispatched in the control's thread.
	/// This class is thread-safe.
	/// </summary>
	public class ConsoleProcess : IDisposable
	{
		private class ProcessExitingException : Exception
		{
			public ProcessExitingException() : base("Cannot WaitForExit() while the process is already exiting.") { }
		}

		protected readonly object _syncObject = new object();
		private readonly Process _process = new Process();
		private readonly EventWaitHandle _fullyExitedWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
		private bool _isDisposed;

		private int _pendingExitedEventHandlers = -1;
		private volatile bool _disposeOnFullyExited; // volatile so that we don't have to lock for up-to-date value

		protected readonly List<string> _output = new List<string>();
		private string _fullOutput;

		#region Properties

		/// <summary>Gets the start info for the process.</summary>
		public ProcessStartInfo StartInfo { get { return _process.StartInfo; } }

		/// <summary>
		/// Gets a value indicating whether the process has been started.
		/// This does not mean it is currently running, because it may
		/// have already exited.
		/// </summary>
		public bool HasStarted
		{
			get
			{
				if (HasExited)
					return true;

				try
				{
					var handle = _process.Handle;
					return true;
				}
				catch (InvalidOperationException)
				{
					// the process has not been started yet
					return false;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the process has exited.
		/// </summary>
		public bool HasExited
		{
			get
			{
				try { return _process.HasExited; }
				catch (InvalidOperationException)
				{
					// the process has not been started yet
					return false;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the process has exited and
		/// all Exited event handlers have finished.
		/// </summary>
		public bool HasFullyExited { get { return _fullyExitedWaitHandle.WaitOne(0); } }

		/// <summary>
		/// Gets the exit code of the process.
		/// </summary>
		/// <exception cref="InvalidOperationException">The process has not exited yet.</exception>
		public int ExitCode { get { return _process.ExitCode; } }

		/// <summary>
		/// Gets the lines written to stdout and stderr.
		/// To circumvent synchronization across threads, this property
		/// can only be accessed after the process has exited, otherwise
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
				if (_fullOutput != null)
					return _fullOutput;

				lock (_syncObject)
				{
					if (_fullOutput != null)
						return _fullOutput;

					if (!HasExited)
						throw new InvalidOperationException("The process has not exited yet.");

					var lines = _output;

					int fullLength = lines.Sum(line => line.Length + Environment.NewLine.Length);
					var output = new StringBuilder(fullLength);
					foreach (string line in lines)
						output.AppendLine(line);

					_fullOutput = output.ToString();

					return _fullOutput;
				}
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Fired when the process has written a line to stdout or stderr.
		/// Invoked asynchronously, except if the event handler is a method
		/// of a Control instance, in which case it will be dispatched in the
		/// thread which created the control.
		/// Do NOT attach or detach event handlers after starting the process!
		/// </summary>
		public event EventHandler<DataReceivedEventArgs> LineWritten;

		/// <summary>
		/// Fired when the process has exited.
		/// Invoked asynchronously, except if the event handler is a method
		/// of a Control instance, in which case it will be dispatched in the
		/// thread which created the control.
		/// Do NOT attach or detach event handlers after starting the process!
		/// </summary>
		public event EventHandler Exited;

		#endregion


		public ConsoleProcess()
		{
			StartInfo.UseShellExecute = false;
			StartInfo.CreateNoWindow = true; // hide the process from the user

			// redirect both output streams
			StartInfo.RedirectStandardOutput = StartInfo.RedirectStandardError = true;

			// set both streams' encoding to the OEM code page, usually used in the console
			StartInfo.StandardOutputEncoding = StartInfo.StandardErrorEncoding =
				Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.TextInfo.OEMCodePage);

			_process.OutputDataReceived += OnDataReceived;
			_process.ErrorDataReceived += OnDataReceived;
			_process.Exited += (s, e) => { OnExited(e); };

			// enable the exited event
			_process.EnableRaisingEvents = true;
		}


		/// <summary>
		/// Kills the process if it is currently running, waits for it
		/// to exit and the Exited event to be handled and then releases
		/// all used resources.
		/// If the call is made during the Exited event, disposing will
		/// be deferred until all Exited event handlers have finished.
		/// Disposing is safely repeatable.
		/// </summary>
		public virtual void Dispose()
		{
			if (_isDisposed)
				return;

			if (HasStarted)
			{
				try { WaitForExit(kill: true); }
				catch (ProcessExitingException)
				{
					// this is a fairly common case: Dispose() called by an Exited event handler
					_disposeOnFullyExited = true;

					return;
				}
			}

			lock (_syncObject)
			{
				if (_isDisposed)
					return;

				_fullyExitedWaitHandle.Dispose();
				_process.Dispose();

				_isDisposed = true;
			}
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

				_pendingExitedEventHandlers = GetNumDelegates(Exited);
				_process.Start();

				_process.BeginOutputReadLine();
				_process.BeginErrorReadLine();
			}
		}

		/// <summary>
		/// Kills the process if it is currently running and waits until it has exited and
		/// all Exited event handlers have finished.
		/// </summary>
		public void Kill()
		{
			if (!HasStarted)
				return;

			try { WaitForExit(kill: true); }
			// the process may be just exiting - return immediately
			catch (ProcessExitingException) { }
		}

		/// <summary>
		/// Waits until the process has exited and all Exited event handlers have finished.
		/// </summary>
		public void WaitForExit()
		{
			if (!HasStarted)
				throw new InvalidOperationException("The process has not been started yet.");

			WaitForExit(kill: false);
		}


		private void WaitForExit(bool kill = false)
		{
			// after _process.WaitForExit(), we cannot simply wait for _fullyExitedWaitHandle because
			// there may be an Exited event handler dispatched in this thread, resulting in a deadlock
			// (this thread would wait for _fullyExitedWaitHandle and hence never execute the dispatched
			// handler while all event handlers need to complete before _fullyExitedWaitHandle is signaled)

			// what we do is hijacking these event handlers from the Exited event and invoking them
			// here in this thread before waiting for _fullyExitedWaitHandle
			// for this to work, this call must be either before the process exits (OnExited()) or
			// after it has exited and all Exited event handlers have finished
			if (HasExited)
			{
				if (HasFullyExited)
					return;

				throw new ProcessExitingException();
			}

			EventHandler compatibleHandlers;
			lock (_syncObject)
			{
				compatibleHandlers = SmartEventInvoker.HijackCompatibleHandlers(ref Exited);

				if (kill)
					KillInternal();
			}

			_process.WaitForExit();

			int numHandlers = GetNumDelegates(compatibleHandlers);
			if (numHandlers > 0)
				compatibleHandlers(this, EventArgs.Empty);

			int numPending = OnExitedEventHandlersFinished(numHandlers);
			if (numPending > 0)
				_fullyExitedWaitHandle.WaitOne();
		}

		/// <summary>
		/// Kills the process asynchronously if it is currently running.
		/// </summary>
		private void KillInternal()
		{
			try
			{
				_process.Kill();
			}
			// the process may be just terminating
			catch (System.ComponentModel.Win32Exception) {}
			// the process may not have been started or already exited
			catch (InvalidOperationException) {}
		}

		private static int GetNumDelegates(MulticastDelegate d)
		{
			return (d == null ? 0 : d.GetInvocationList().Length);
		}

		private int _onExitedInvoked = 0;
		/// <summary>
		/// Invoked asynchronously when the process has exited.
		/// </summary>
		private void OnExited(EventArgs e)
		{
			// the Exited event is sometimes triggered a second time
			// I guess that has to do with disposing of the process in another thread
			// while the Exited event hasn't been handled completely yet
			if (Interlocked.Exchange(ref _onExitedInvoked, 1) == 1)
				return;

			// hijack all (remaining) event handlers from the Exited event
			EventHandler exitedEventHandlers;
			lock (_syncObject)
			{
				exitedEventHandlers = Exited;
				Exited = null;
			}

			// invoke/dispatch them
			var iasyncs = SmartEventInvoker.FireEvent(exitedEventHandlers, this, e);

			int numHandlers = GetNumDelegates(exitedEventHandlers); // incl. dispatched ones
			int numDispatched = (iasyncs == null ? 0 : iasyncs.Count);
			int numInvoked = numHandlers - numDispatched;

			OnExitedEventHandlersFinished(numInvoked);

			if (numDispatched > 0)
			{
				// in a new thread:
				var thread = new Thread(() =>
				{
					// wait for all dispatched handlers to complete
					SmartEventInvoker.Wait(iasyncs);
					OnExitedEventHandlersFinished(numDispatched);
				});
				thread.Start();
			}
		}

		/// <summary>
		/// Invoked (potentially asynchronously) when some Exited event handlers have finished
		/// and may trigger OnFullyExited().
		/// Returns the number of still pending Exited event handlers.
		/// </summary>
		private int OnExitedEventHandlersFinished(int numHandlers)
		{
			// decrement _pendingExitedEventHandlers atomically
			int numPending = Interlocked.Add(ref _pendingExitedEventHandlers, -numHandlers);

			// the first caller reaching 0 triggers OnFullyExited()
			if (numPending == 0 && !HasFullyExited)
				OnFullyExited(EventArgs.Empty);

			return numPending;
		}

		/// <summary>
		/// Invoked (potentially asynchronously) when the process has exited and all Exited
		/// event handlers have finished.
		/// </summary>
		private void OnFullyExited(EventArgs e)
		{
			_fullyExitedWaitHandle.Set();

			if (_disposeOnFullyExited)
				Dispose();
		}


		/// <summary>
		/// Invoked asynchronously when the process has written a line to stdout or stderr.
		/// </summary>
		private void OnDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null)
				return;

			lock (_syncObject)
				_output.Add(e.Data);

			OnLineWritten(e);
		}

		/// <summary>
		/// Invoked asynchronously when the process has written a line to stdout or stderr.
		/// </summary>
		protected virtual void OnLineWritten(DataReceivedEventArgs e)
		{
			SmartEventInvoker.FireEvent(LineWritten, this, e);
		}
	}
}

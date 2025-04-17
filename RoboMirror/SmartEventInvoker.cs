/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.Threading;

namespace RoboMirror
{
	public static class SmartEventInvoker
	{
		/// <summary>
		/// Fires the specified event.
		/// All event handlers are scanned for methods of Control instances.
		/// If the current thread is not the one which created the control,
		/// the control's method is dispatched in the control's thread.
		/// </summary>
		/// <param name="ev">Event to be fired. May be null.</param>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">EventArgs instance for the event handlers.</param>
		/// <returns>
		/// One IAsyncResult object for each event handler dispatched in
		/// another thread, or null if no handlers have been dispatched.
		/// </returns>
		public static List<IAsyncResult> FireEvent(MulticastDelegate ev, object sender, EventArgs e)
		{
			if (ev == null)
				return null;

			var args = new object[] { sender, e };
			var delegates = ev.GetInvocationList();
			var dispatchedDelegates = new List<Delegate>(delegates.Length);

			foreach (var d in delegates)
			{
				var control = d.Target as System.ComponentModel.ISynchronizeInvoke;
				if (control != null && control.InvokeRequired)
					dispatchedDelegates.Add(d);
				else
				{
					// invoke the delegate normally
					d.DynamicInvoke(args);
				}
			}

			if (dispatchedDelegates.Count == 0)
				return null;

			// now that all normally invoked delegates have completed without throwing an
			// exception, we dispatch the Control methods in the control threads
			var iasyncs = new List<IAsyncResult>(dispatchedDelegates.Count);
			foreach (var d in dispatchedDelegates)
			{
				var control = (System.ComponentModel.ISynchronizeInvoke)d.Target;

				// by using BeginInvoke(), we lose dynamic binding,
				// but event handlers are usually not virtual
				System.Diagnostics.Debug.Assert(!d.Method.IsVirtual);
				var iasync = control.BeginInvoke(d, args);
				iasyncs.Add(iasync);
			}

			return iasyncs;
		}

		/// <summary>
		/// Waits for all asynchronous operations (e.g., dispatched event handlers) to complete.
		/// </summary>
		public static void Wait(List<IAsyncResult> iasyncs)
		{
			if (iasyncs == null || iasyncs.Count == 0)
				return;

			var waitHandles = new WaitHandle[iasyncs.Count];
			for (int i = 0; i < iasyncs.Count; ++i)
				waitHandles[i] = iasyncs[i].AsyncWaitHandle;

			WaitHandle.WaitAll(waitHandles);

			foreach (var waitHandle in waitHandles)
				waitHandle.Dispose();
		}


		/// <summary>
		/// Hijacks event handlers of the specified event which can be run in the calling thread.
		/// </summary>
		/// <param name="ev">
		/// Event containing all event handlers.
		/// After the call, the original event will only contain event handlers which need to be
		/// run in other threads, or be null if there are no incompatible handlers.
		/// </param>
		/// <returns>
		/// A new multicast delegate containing all event handlers which can be run in the calling thread.
		/// Null if there are no compatible handlers.
		/// </returns>
		public static EventHandler HijackCompatibleHandlers(ref EventHandler ev)
		{
			if (ev == null)
				return null;

			var delegates = ev.GetInvocationList();
			var myDelegates = new List<Delegate>(delegates.Length);    // can be run in this thread
			var otherDelegates = new List<Delegate>(delegates.Length); // need to be run in another thread

			foreach (var d in delegates)
			{
				var control = d.Target as System.ComponentModel.ISynchronizeInvoke;
				if (control != null && control.InvokeRequired)
					otherDelegates.Add(d);
				else
					myDelegates.Add(d);
			}

			ev = (EventHandler)Delegate.Combine(otherDelegates.ToArray());

			return (EventHandler)Delegate.Combine(myDelegates.ToArray());
		}
	}
}

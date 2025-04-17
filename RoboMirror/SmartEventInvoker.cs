/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;

namespace RoboMirror
{
	public static class SmartEventInvoker
	{
		/// <summary>
		/// Fires the specified event.
		/// All event handlers are scanned for methods of Control instances.
		/// If the current thread is not the one which created the control,
		/// the control's method is invoked asynchronously in the control's
		/// thread.
		/// </summary>
		/// <param name="ev">Event to be fired. May be null.</param>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">EventArgs instance for the event handlers.</param>
		public static void FireEvent(MulticastDelegate ev, object sender, EventArgs e)
		{
			if (ev == null)
				return;

			var delegates = ev.GetInvocationList();

			var args = new object[] { sender, e };

			foreach (var d in delegates)
			{
				var control = d.Target as System.Windows.Forms.Control;

				if (control != null && control.InvokeRequired)
				{
					// invoke the Control method asynchronously in the control's
					// thread
					// we lose dynamic binding this way, but event handlers
					// usually do not get overridden
					control.BeginInvoke(d, args);
				}
				else
				{
					// invoke the delegate normally
					d.DynamicInvoke(args);
				}
			}
		}
	}
}

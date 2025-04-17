/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.IO;

namespace RoboMirror
{
	#region TextEventArgs class.

	/// <summary>
	/// Augments the EventArgs class by a Text property.
	/// </summary>
	public class TextEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the associated text.
		/// </summary>
		public string Text { get; private set; }

		/// <summary>
		/// Creates a new TextEventArgs with the specified text.
		/// </summary>
		public TextEventArgs(string text)
		{
			Text = (text == null ? string.Empty : text);
		}
	}

	#endregion


	/// <summary>
	/// Provides a session during which a persistent volume shadow copy
	/// may be accessed.
	/// </summary>
	public sealed class VolumeShadowCopySession : IDisposable
	{
		private static string _vshadowPath;

		private ConsoleProcess _process;

		// synchronize disposing because it may involve starting another
		// process, therefore it must be disposed only once, even
		// across multiple threads
		private readonly object _syncObject = new object();


		/// <summary>
		/// Gets or sets the ID of the shadow copy snapshot.
		/// </summary>
		private string SnapshotID { get; set; }

		/// <summary>
		/// Gets the path to the mount point of the shadow copy snapshot.
		/// </summary>
		public string MountPoint { get; private set; }


		/// <summary>
		/// Fired when the volume shadow copy has been created and mounted.
		/// Invoked asynchronously, except if the event handler is a method
		/// of a Control instance, in which case it will be invoked in the
		/// thread which created the control.
		/// </summary>
		public event EventHandler Ready;

		/// <summary>
		/// Fired when the volume shadow copy could not be created or mounted.
		/// Probably invoked asynchronously, except if the event handler is a
		/// method of a Control instance, in which case it will be invoked in
		/// the thread which created the control.
		/// </summary>
		public event EventHandler<TextEventArgs> Aborted;


		/// <summary>
		/// Creates a new volume shadow copy session.
		/// </summary>
		public VolumeShadowCopySession()
		{
			// make sure the path to the vshadow.exe is set
			if (string.IsNullOrEmpty(_vshadowPath))
			{
				_vshadowPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "Tools");

				// select either the 64 bit or 32 bit version
				bool x64 = Directory.Exists(Environment.SystemDirectory + @"\..\SysWOW64");
				_vshadowPath = Path.Combine(_vshadowPath,
					(x64 ? "vshadow64.exe" : "vshadow32.exe"));

				if (!File.Exists(_vshadowPath))
					throw new InvalidOperationException(string.Format("\"{0}\" does not exist.", _vshadowPath));
			}
		}

		/// <summary>
		/// Makes sure the persistent shadow copy is deleted.
		/// Disposing is thread-safe and repeatable.
		/// </summary>
		public void Dispose()
		{
			lock (_syncObject)
			{
				if (_process != null)
				{
					_process.Dispose();
					_process = null;
				}

				if (SnapshotID != null)
				{
					// try to delete the shadow copy synchronously
					// (deleting is fast anyway)
					using (var process = new ConsoleProcess())
					{
						process.StartInfo.FileName = _vshadowPath;
						process.StartInfo.Arguments = string.Format("-ds={0}", SnapshotID);

						process.Start();
						process.WrappedProcess.WaitForExit();
					}

					SnapshotID = null;
				}

				if (!string.IsNullOrEmpty(MountPoint))
				{
					Directory.Delete(MountPoint);
					MountPoint = null;
				}
			}
		}


		/// <summary>
		/// Starts the session, i.e. creates a persistent shadow copy of the specified
		/// volume and mounts it in a temporary directory.
		/// </summary>
		public void Start(string volume)
		{
			if (string.IsNullOrEmpty(volume))
				throw new ArgumentNullException("volume");

			if (SnapshotID != null)
				throw new InvalidOperationException("The shadow copy session is already active.");

			_process = new ConsoleProcess();

			_process.StartInfo.FileName = _vshadowPath;
			_process.StartInfo.Arguments = string.Format("-p -nw \"{0}\"", volume);

			_process.Exited += CreationProcess_Exited;

			try
			{
				_process.Start();
			}
			catch (Exception e)
			{
				OnAborted(new TextEventArgs("The volume shadow copy could not be created:\n\n" +
					e.Message));
			}
		}

		/// <summary>
		/// Invoked asynchronously when the creation process has exited.
		/// </summary>
		private void CreationProcess_Exited(object sender, EventArgs e)
		{
			if (_process.ExitCode == 0)
			{
				try
				{
					MountShadowCopy();
				}
				catch (Exception exception)
				{
					OnAborted(new TextEventArgs("The volume shadow copy could not be mounted:\n\n" +
						exception.Message));
				}
			}
			else
			{
				OnAborted(new TextEventArgs("The volume shadow copy could not be created:\n\n" +
					_process.FullOutput));
			}
		}


		/// <summary>
		/// Parses the vshadow output after shadow copy creation and
		/// mounts the snapshot in an appropriate temporary directory.
		/// </summary>
		private void MountShadowCopy()
		{
			var lines = _process.Output;

			// parse the snapshot ID
			for (int i = lines.Count - 1; i >= 0; i--)
			{
				if (lines[i].StartsWith("* SNAPSHOT ID = ", StringComparison.Ordinal))
				{
					SnapshotID = lines[i].Substring(16, 38);
					break;
				}
			}

			if (SnapshotID == null)
			{
				throw new NotSupportedException("The vshadow output could not be parsed:\n\n" +
					_process.FullOutput);
			}

			// create a new directory in the temp folder and use it as
			// mount point
			string tempPath = Path.GetTempPath();
			string path;

			do
			{
				path = Path.Combine(tempPath, Guid.NewGuid().ToString());
			} while (Directory.Exists(path));

			Directory.CreateDirectory(path);

			MountPoint = path;

			// start the mount process
			_process.Dispose();
			_process = null;

			_process = new ConsoleProcess();

			_process.StartInfo.FileName = _vshadowPath;
			_process.StartInfo.Arguments = string.Format("-el={0},\"{1}\"", SnapshotID, path);

			_process.Exited += MountProcess_Exited;

			_process.Start();
		}

		/// <summary>
		/// Invoked asynchronously when the mount process has exited.
		/// </summary>
		private void MountProcess_Exited(object sender, EventArgs e)
		{
			if (_process.ExitCode != 0)
			{
				OnAborted(new TextEventArgs("The volume shadow copy could not be mounted:\n\n" +
					_process.FullOutput));
			}

			_process.Dispose();
			_process = null;

			OnReady(EventArgs.Empty);
		}


		/// <summary>
		/// Fires the Ready event.
		/// </summary>
		private void OnReady(EventArgs e)
		{
			SmartEventInvoker.FireEvent(Ready, this, e);
		}

		/// <summary>
		/// Disposes of the session and fires the Aborted event.
		/// </summary>
		internal void OnAborted(TextEventArgs e)
		{
			Dispose();

			SmartEventInvoker.FireEvent(Aborted, this, e);
		}
	}
}

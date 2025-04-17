/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.IO;
using System.Threading;
using Alphaleonis.Win32.Vss;

namespace RoboMirror
{
	#region TextEventArgs class

	/// <summary>Extends the EventArgs class by a read-only Text property.</summary>
	public class TextEventArgs : EventArgs
	{
		public string Text { get; private set; }

		public TextEventArgs(string text)
		{
			Text = (text == null ? string.Empty : text);
		}
	}

	#endregion

	/// <summary>
	/// Provides an asynchronous session during which a new,
	/// non-persistent volume shadow copy snapshot may be accessed.
	/// This class is thread-safe.
	/// </summary>
	public sealed class VolumeShadowCopySession : IDisposable
	{
		private readonly object _syncObject = new object();
		private VscThread _thread;
		private volatile string _mountPoint; // set and reset in a dedicated _thread


		/// <summary>
		/// Gets the path to the source folder of the shadow copy snapshot.
		/// Will only be set after the session has been started.
		/// </summary>
		public string SourceFolder { get; private set; }

		/// <summary>
		/// Gets the path to the mount point of the shadow copy snapshot.
		/// Will only be set right before firing the Ready event.
		/// </summary>
		public string MountPoint { get { return _mountPoint; } }


		/// <summary>
		/// Fired when the volume shadow copy snapshot has been created and
		/// mounted.
		/// Invoked asynchronously, except if the event handler is a method
		/// of a Control instance, in which case it will be dispatched in the
		/// thread which created the control.
		/// Do NOT attach or detach event handlers after starting the session!
		/// </summary>
		public event EventHandler Ready;

		/// <summary>
		/// Fired when the volume shadow copy snapshot could not be created
		/// or mounted.
		/// The session will already be disposed of.
		/// Invoked asynchronously, except if the event handler is a method
		/// of a Control instance, in which case it will be dispatched in the
		/// thread which created the control.
		/// Do NOT attach or detach event handlers after starting the session!
		/// </summary>
		public event EventHandler<TextEventArgs> Error;


		public void Start(string sourceFolder)
		{
			if (!PathHelper.IsValidAbsolutePath(sourceFolder) || !Directory.Exists(sourceFolder))
				throw new ArgumentException("sourceFolder must be a valid absolute path to an existing folder.", "sourceFolder");

			lock (_syncObject)
			{
				if (_thread != null)
					throw new InvalidOperationException("The volume shadow copy session has already been started.");

				SourceFolder = sourceFolder;
				_thread = new VscThread(this);
			}
		}

		/// <summary>
		/// Initiates disposing of the volume shadow copy snapshot.
		/// Disposing is safely repeatable.
		/// </summary>
		public void Dispose()
		{
			lock (_syncObject)
			{
				if (_thread != null)
					_thread.Dispose();
			}
		}


		private void OnReady()
		{
			SmartEventInvoker.FireEvent(Ready, this, EventArgs.Empty);
		}

		private void OnError(string text)
		{
			SmartEventInvoker.FireEvent(Error, this, new TextEventArgs(text));
		}



		#region VscThread class

		/// <summary>
		/// Represents a new thread which
		/// * creates and mounts the volume shadow copy snapshot,
		/// * fires the session's Ready event,
		/// * waits for a Dispose() request and
		/// * unmounts and deletes the snapshot.
		/// </summary>
		private sealed class VscThread : IDisposable
		{
			private readonly VolumeShadowCopySession _session;
			private readonly object _disposeEventSyncObject = new object();
			private ManualResetEvent _disposeEvent = new ManualResetEvent(false);

			// exclusively accessed by the new thread only:
			private IVssBackupComponents _backup;
			private Guid _snapshotSetID = Guid.Empty;
			private Guid _volumeSnapshotID = Guid.Empty;

			/// <summary>Creates a new thread and starts it.</summary>
			public VscThread(VolumeShadowCopySession session)
			{
				_session = session;

				var thread = new Thread(this.Main);
				thread.Start();
			}

			/// <summary>
			/// Initiates disposing.
			/// Actual disposing will be deferred to either one of a few points during
			/// the creation of the snapshot (in order to allow aborting the operation
			/// in a controlled way), or, once the snapshot is mounted, after all Ready
			/// event handlers have finished.
			/// This method is thread-safe and repeatable.
			/// </summary>
			public void Dispose()
			{
				// Cleanup() closes _disposeEvent and sets it to null
				lock (_disposeEventSyncObject)
				{
					if (_disposeEvent != null)
						_disposeEvent.Set();
				}
			}

			private delegate void Action();
			private void Main()
			{
				string sourceFolder = PathHelper.AppendSeparator(_session.SourceFolder);                         // C:\,  C:\Folder\, \\Server\Share\, \\Server\Share\Folder\
				string volume = PathHelper.RemoveTrailingSeparator(Path.GetPathRoot(sourceFolder));              // C:,   C:,         \\Server\Share,  \\Server\Share
				string pathFromRoot = PathHelper.RemoveTrailingSeparator(sourceFolder.Substring(volume.Length)); // "",   \Folder,    "",              \Folder
				if (pathFromRoot.Length == 0)
					pathFromRoot = null;                                                                         // null, \Folder,    null,            \Folder

				try
				{
					// helper function throwing an ObjectDisposedException if _disposeEvent is currently signaled
					Action CheckForDisposeRequest = () => { if (_disposeEvent.WaitOne(0)) throw new ObjectDisposedException("VscThread"); };

					CheckForDisposeRequest();

					_backup = VssUtils.LoadImplementation().CreateVssBackupComponents();
					_backup.InitializeForBackup(null);
					_backup.SetContext(VssSnapshotContext.Backup);
					_backup.SetBackupState(false, false, VssBackupType.Copy, false);
					CheckForDisposeRequest();

					_backup.GatherWriterMetadata();
					CheckForDisposeRequest();

					_snapshotSetID = _backup.StartSnapshotSet();
					_volumeSnapshotID = _backup.AddToSnapshotSet(PathHelper.AppendSeparator(volume));
					CheckForDisposeRequest();

					_backup.PrepareForBackup();
					VerifyWriterStatus();
					CheckForDisposeRequest();

					// create the snapshot
					_backup.DoSnapshotSet();
					VerifyWriterStatus();

					// mount the source folder as file share
					// (non-persistent shadow copies cannot be mounted locally on a drive or folder)
					string shareName = _backup.ExposeSnapshot(_volumeSnapshotID, pathFromRoot,
						VssVolumeSnapshotAttributes.ExposedRemotely, null);
					_session._mountPoint = @"\\localhost\" + shareName;

					CheckForDisposeRequest();
				}
				catch (ObjectDisposedException) // by CheckForDisposeRequest()
				{
					Cleanup();
					return;
				}
				catch (Exception e)
				{
					Cleanup();

					string msg = (e is UnauthorizedAccessException
						? "You lack the required privileges to create a volume shadow copy.\nPlease restart RoboMirror as administrator."
						: "The volume shadow copy could not be created:\n\n" + e.Message);
					_session.OnError(msg);

					return;
				}

				// fire the Ready event
				try { _session.OnReady(); }
				// on unhandled exception by Ready event handler: Cleanup() before rethrowing
				catch { Cleanup(); throw; }

				// wait for Dispose() signal
				_disposeEvent.WaitOne();

				Cleanup();
			}

			private void VerifyWriterStatus()
			{
				_backup.GatherWriterStatus();

				var sb = new System.Text.StringBuilder();
				foreach (var status in _backup.WriterStatus)
				{
					if (status.Failure != VssError.Success)
					{
						sb.AppendFormat("VSS writer {0} reports error {1}{2}", status.Name, status.Failure.ToString(),
							string.IsNullOrEmpty(status.ApplicationErrorMessage) ? string.Empty : ": " + status.ApplicationErrorMessage);
						sb.AppendLine();
					}
				}

				if (sb.Length > 0)
					throw new Exception(sb.ToString());
			}

			private void Cleanup()
			{
				if (_session._mountPoint != null)
				{
					_backup.BackupComplete();
					VerifyWriterStatus();

					_backup.UnexposeSnapshot(_volumeSnapshotID);
					_session._mountPoint = null;
				}

				if (_snapshotSetID != Guid.Empty)
					_backup.DeleteSnapshotSet(_snapshotSetID, forceDelete: true);

				if (_backup != null)
					_backup.Dispose();

				lock (_disposeEventSyncObject)
				{
					_disposeEvent.Close();
					_disposeEvent = null;
				}
			}
		}

		#endregion
	}
}

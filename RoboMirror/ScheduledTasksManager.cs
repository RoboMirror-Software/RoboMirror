/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RoboMirror
{
	public sealed class ScheduledTasksManager : IDisposable
	{
		private static readonly string FOLDER = "RoboMirror";

		private TaskService _service;
		private TaskFolder _folder;
		private bool _v1Mode;

		public ScheduledTasksManager()
		{
			_service = new TaskService();
			_v1Mode = (_service.HighestSupportedVersion < new Version(1, 2));

			var root = _service.RootFolder;
			if (_v1Mode)
			{
				// no support for subfolders
				_folder = root;
			}
			else
			{
				// a normal user isn't allowed to create tasks in the root tasks folder
				_folder = (root.SubFolders.Exists(FOLDER)
					? root.SubFolders[FOLDER]
					: root.CreateFolder(FOLDER));
			}
		}

		public void Dispose()
		{
			if (_service != null)
			{
				_service.Dispose();
				_service = null;
				_folder = null;
			}
		}


		/// <summary>
		/// Gets the scheduled task associated with the specified mirror task.
		/// Returns null if it doesn't exist.
		/// </summary>
		public Task Get(MirrorTask mirrorTask)
		{
			if (mirrorTask == null)
				throw new ArgumentNullException("mirrorTask");

			string name = GetName(mirrorTask);
			var tasks = _folder.Tasks;

			return (tasks.Exists(name) ? tasks[name] : null);
		}

		/// <summary>
		/// Creates/overwrites the scheduled task for the specified mirror task.
		/// </summary>
		public Task Save(MirrorTask mirrorTask, Trigger trigger)
		{
			if (mirrorTask == null)
				throw new ArgumentNullException("mirrorTask");
			if (trigger == null)
				throw new ArgumentNullException("trigger");

			var definition = _service.NewTask();

			definition.RegistrationInfo.Description = string.Format("Mirrors \"{0}\" to \"{1}\".",
				mirrorTask.Source, mirrorTask.Target);

			definition.Actions.Add(new ExecAction(Application.ExecutablePath,
				mirrorTask.Guid, Application.StartupPath));

			definition.Triggers.Add(trigger);

			definition.Principal.LogonType = TaskLogonType.InteractiveToken;

			// set some advanced settings under Vista+
			if (!_v1Mode)
			{
				definition.Settings.AllowHardTerminate = false;
				definition.Settings.StopIfGoingOnBatteries = false;
				definition.Settings.StartWhenAvailable = true;

				if (UacHelper.IsInAdminRole())
					definition.Principal.RunLevel = TaskRunLevel.Highest;
			}

			Delete(mirrorTask);

			return _folder.RegisterTaskDefinition(GetName(mirrorTask), definition,
				TaskCreation.Create, null, null, TaskLogonType.InteractiveToken, null);
		}

		/// <summary>
		/// Deletes the scheduled task associated with the specified mirror task
		/// if it exists.
		/// </summary>
		public void Delete(MirrorTask mirrorTask)
		{
			if (mirrorTask == null)
				throw new ArgumentNullException("mirrorTask");

			_folder.DeleteTask(GetName(mirrorTask), exceptionOnNotExists: false);
		}


		/// <summary>
		/// Gets the name for the scheduled task (to be) associated with the specified mirror task.
		/// </summary>
		private static string GetName(MirrorTask mirrorTask)
		{
			return string.Format("RoboMirror ({0})", mirrorTask.Guid);
		}
	}
}

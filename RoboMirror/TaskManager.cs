/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Microsoft.Win32.TaskScheduler;

namespace RoboMirror
{
	/// <summary>
	/// Manages loading tasks from and saving tasks to an XML file.
	/// </summary>
	public sealed class TaskManager : IDisposable
	{
		// path to the XML file, relative to the user's AppData folder
		private static readonly string PATH = Path.Combine("RoboMirror", "Tasks.xml");


		private FileStream _file;
		private XmlDocument _document;


		/// <summary>
		/// Creates a new TaskManager.
		/// Other threads cannot open the XML file for writing as long as the
		/// instance is not disposed of. This ensures a consistent file, even
		/// across multiple RoboMirror instances (e.g., GUI + scheduled task),
		/// and that there can only be one running GUI instance at a time.
		/// </summary>
		/// <param name="readOnly">
		/// Indicates whether the XML file should be opened in read-only mode,
		/// allowing to load tasks even if another accessing thread exists.
		/// </param>
		public TaskManager(bool readOnly)
		{
			string path = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PATH);

			try
			{
				string folder = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));

				if (!Directory.Exists(folder))
					Directory.CreateDirectory(folder);

				_file = File.Open(path, FileMode.OpenOrCreate,
					(readOnly ? FileAccess.Read : FileAccess.ReadWrite),
					(readOnly ? FileShare.ReadWrite : FileShare.Read));
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(
					string.Format("\"{0}\" could not be opened, probably due to another running RoboMirror instance.",
					path), e);
			}

			_document = new XmlDocument();

			if (_file.Length > 0)
			{
				try
				{
					_document.Load(_file);
				}
				catch (Exception e)
				{
					throw new InvalidOperationException(string.Format("\"{0}\" is corrupt.", path), e);
				}
			}
			else
			{
				// create the document element
				_document.AppendChild(_document.CreateElement("tasks"));
			}
		}

		/// <summary>
		/// Unlocks the XML file.
		/// </summary>
		public void Dispose()
		{
			_file.Close();
		}


		/// <summary>
		/// Loads all tasks from the XML file.
		/// </summary>
		/// <returns></returns>
		public List<MirrorTask> LoadTasks()
		{
			var tasks = new List<MirrorTask>();

			var taskNodes = _document.DocumentElement.SelectNodes("task");

			foreach (XmlNode node in taskNodes)
			{
				tasks.Add(MirrorTask.Deserialize(node));
			}

			return tasks;
		}

		/// <summary>
		/// Loads the task with the specified GUID from the XML file or returns
		/// null if it does not exist.
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public MirrorTask LoadTask(string guid)
		{
			if (string.IsNullOrEmpty(guid))
				return null;

			var node = GetTaskNode(guid);

			return (node == null ? null : MirrorTask.Deserialize(node));
		}


		/// <summary>
		/// Saves the specified task to the XML file.
		/// </summary>
		/// <param name="task"></param>
		public void SaveTask(MirrorTask task)
		{
			if (task == null)
				throw new ArgumentNullException("task");

			var node = GetTaskNode(task.Guid);
			if (node != null)
			{
				// the task already exists in the file, reset its XML node by removing
				// all attributes and children
				node.RemoveAll();
			}
			else
			{
				node = _document.CreateElement("task");
				_document.DocumentElement.AppendChild(node);
			}

			task.Serialize(node);

			SaveDocument();
		}


		/// <summary>
		/// Deletes the specified task from the XML file, including the
		/// first associated scheduled task if it exists.
		/// </summary>
		/// <param name="task"></param>
		public void DeleteTask(MirrorTask task)
		{
			if (task == null)
				throw new ArgumentNullException("task");

			var scheduledTask = GetScheduledTask(task);
			if (scheduledTask != null)
			{
				using (var service = new TaskService())
				{
					service.RootFolder.DeleteTask(scheduledTask.Name);
				}
			}

			var node = GetTaskNode(task.Guid);
			if (node == null)
				return;

			node.ParentNode.RemoveChild(node);

			SaveDocument();
		}


		/// <summary>
		/// Returns the specified task's node in the XML document or null
		/// if there is no matching node.
		/// </summary>
		/// <param name="guid">GUID of the task.</param>
		/// <returns></returns>
		private XmlNode GetTaskNode(string guid)
		{
			return _document.DocumentElement.SelectSingleNode(
				string.Format("task[@guid=\"{0}\"]", guid));
		}

		/// <summary>
		/// Saves the current XML document to disk.
		/// </summary>
		private void SaveDocument()
		{
			if (!_file.CanWrite)
				throw new InvalidOperationException("A read-only task manager cannot modify the XML file.");

			// reset the stream
			_file.Position = 0;
			_file.SetLength(0);

			// save to the stream
			_document.Save(_file);

			// save to disk
			_file.Flush();
		}


		/// <summary>
		/// Gets the first scheduled task associated with the specified mirror task.
		/// Returns null if there is no associated scheduled task.
		/// </summary>
		/// <param name="mirrorTask"></param>
		/// <returns></returns>
		public static Task GetScheduledTask(MirrorTask mirrorTask)
		{
			if (mirrorTask == null)
				throw new ArgumentNullException("mirrorTask");

			using (var service = new TaskService())
			{
				foreach (var task in service.RootFolder.Tasks)
				{
					if (task.Definition.Actions.Count != 1)
						continue;

					var execAction = task.Definition.Actions[0] as ExecAction;
					if (execAction != null &&
						string.Compare(execAction.Path, System.Windows.Forms.Application.ExecutablePath, StringComparison.OrdinalIgnoreCase) == 0 &&
						execAction.Arguments == mirrorTask.Guid)
					{
						return task;
					}
				}
			}

			return null;
		}
	}
}

/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Win32.TaskScheduler;

namespace RoboMirror
{
	/// <summary>
	/// Manages loading tasks from and saving tasks to an XML file.
	/// </summary>
	public sealed class TaskManager : XmlFileManager
	{
		// path to the XML file, relative to the user's AppData folder
		private static readonly string PATH = Path.Combine("RoboMirror", "Tasks.xml");

		/// <summary>
		/// Creates a new TaskManager.
		/// Other threads cannot open the XML file for writing as long as the
		/// instance is not disposed of. This ensures a consistent file, even
		/// across multiple RoboMirror instances (e.g., GUI + scheduled task),
		/// and that there can only be one running GUI instance at a time.
		/// </summary>
		/// <exception cref="FileLockedException"></exception>
		public TaskManager(bool readOnly)
			: base(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PATH), "tasks", readOnly)
		{ }

		/// <summary>
		/// Loads all tasks from the XML file.
		/// </summary>
		public List<MirrorTask> LoadTasks()
		{
			var tasks = new List<MirrorTask>();

			var taskNodes = Document.DocumentElement.SelectNodes("task");

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
				node = Document.CreateElement("task");
				Document.DocumentElement.AppendChild(node);
			}

			task.Serialize(node);

			Save();
		}


		/// <summary>
		/// Deletes the specified task from the XML file, including the
		/// associated scheduled task if it exists.
		/// </summary>
		public void DeleteTask(MirrorTask task)
		{
			if (task == null)
				throw new ArgumentNullException("task");

			try
			{
				using (var scheduledTasksManager = new ScheduledTasksManager())
				{
					scheduledTasksManager.Delete(task);
				}
			}
			catch { }

			var node = GetTaskNode(task.Guid);
			if (node == null)
				return;

			node.ParentNode.RemoveChild(node);

			Save();
		}


		/// <summary>
		/// Returns the specified task's node in the XML document or null
		/// if there is no matching node.
		/// </summary>
		/// <param name="guid">GUID of the task.</param>
		private XmlNode GetTaskNode(string guid)
		{
			return Document.DocumentElement.SelectSingleNode(
				string.Format("task[@guid=\"{0}\"]", guid));
		}
	}
}

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

namespace RoboMirror
{
	/// <summary>
	/// Associates a source folder with a target folder, including the
	/// mirroring parameters and the last backup timestamp.
	/// </summary>
	public sealed class MirrorTask
	{
		#region Properties.

		/// <summary>
		/// Gets the globally unique ID of this task.
		/// </summary>
		public string Guid { get; private set; }


		/// <summary>
		/// Gets or sets the path to the source folder to be mirrored.
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a temporary volume shadow copy
		/// of the source volume is to be created and used during mirroring.
		/// This way locked files can be read and mirrored.
		/// </summary>
		public bool UseVolumeShadowCopy { get; set; }

		/// <summary>
		/// Gets the list of files to be excluded from mirroring.
		/// Paths are relative to the source folder and must begin with a
		/// directory separator char; wildcards must not contain any path
		/// information and therefore do not begin with a directory separator char.
		/// </summary>
		public List<string> ExcludedFiles { get; private set; }

		/// <summary>
		/// Gets the list of subfolders to be excluded from mirroring.
		/// Paths are relative to the source folder and must begin with a
		/// directory separator char; wildcards must not contain any path
		/// information and therefore do not begin with a directory separator char.
		/// </summary>
		public List<string> ExcludedFolders { get; private set; }

		/// <summary>
		/// Gets or sets a string encoding the attributes of files to be excluded
		/// (RASHCNETO).
		/// </summary>
		public string ExcludedAttributes { get; set; }


		/// <summary>
		/// Gets or sets the path to the mirror target folder.
		/// </summary>
		public string Target { get; set; }

		/// <summary>
		/// Gets or sets a string encoding the extended security attributes to be
		/// copied (SOU).
		/// </summary>
		public string ExtendedAttributes { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether extra files and folders in
		/// the target folder are to be deleted.
		/// </summary>
		public bool DeleteExtraItems { get; set; }


		/// <summary>
		/// Gets or sets the date and time of the last successful backup operation.
		/// </summary>
		public DateTime? LastBackup { get; set; }

		#endregion


		/// <summary>
		/// Creates a new MirrorTask.
		/// </summary>
		public MirrorTask()
		{
			Guid = System.Guid.NewGuid().ToString();
			ExcludedFiles = new List<string>();
			ExcludedFolders = new List<string>();
		}


		/// <summary>
		/// Converts the task to an XML representation at the specified XML node.
		/// </summary>
		public void Serialize(XmlNode taskNode)
		{
			if (taskNode == null)
				throw new ArgumentNullException("taskNode");

			var document = taskNode.OwnerDocument;

			var attribute = document.CreateAttribute("guid");
			attribute.Value = Guid;
			taskNode.Attributes.Append(attribute);

			var node = document.CreateElement("source");
			node.InnerText = Source;
			taskNode.AppendChild(node);

			node = document.CreateElement("useVolumeShadowCopy");
			node.InnerText = UseVolumeShadowCopy.ToString();
			taskNode.AppendChild(node);

			if (ExcludedFiles.Count > 0)
			{
				var exclusionsNode = document.CreateElement("exclusions");
				taskNode.AppendChild(exclusionsNode);

				foreach (string exclusion in ExcludedFiles)
				{
					if (string.IsNullOrEmpty(exclusion))
						continue;

					node = document.CreateElement("file");
					node.InnerText = exclusion;
					exclusionsNode.AppendChild(node);
				}
			}

			if (ExcludedFolders.Count > 0)
			{
				var exclusionsNode = (XmlElement)taskNode.SelectSingleNode("exclusions");
				if (exclusionsNode == null)
				{
					exclusionsNode = document.CreateElement("exclusions");
					taskNode.AppendChild(exclusionsNode);
				}

				foreach (string exclusion in ExcludedFolders)
				{
					if (string.IsNullOrEmpty(exclusion))
						continue;

					node = document.CreateElement("folder");
					node.InnerText = exclusion;
					exclusionsNode.AppendChild(node);
				}
			}

			node = document.CreateElement("excludedAttributes");
			node.InnerText = (ExcludedAttributes == null ? string.Empty : ExcludedAttributes);
			taskNode.AppendChild(node);

			node = document.CreateElement("target");
			node.InnerText = Target;
			taskNode.AppendChild(node);

			node = document.CreateElement("extendedAttributes");
			node.InnerText = (ExtendedAttributes == null ? string.Empty : ExtendedAttributes);
			taskNode.AppendChild(node);

			node = document.CreateElement("deleteExtraItems");
			node.InnerText = DeleteExtraItems.ToString();
			taskNode.AppendChild(node);

			if (LastBackup.HasValue)
			{
				node = document.CreateElement("lastBackup");
				node.InnerText = LastBackup.Value.ToUniversalTime().ToString("u",
					System.Globalization.CultureInfo.InvariantCulture);
				taskNode.AppendChild(node);
			}
		}

		/// <summary>
		/// Recreates a task from the specified XML node.
		/// </summary>
		public static MirrorTask Deserialize(XmlNode taskNode)
		{
			if (taskNode == null)
				throw new ArgumentNullException("taskNode");

			var task = new MirrorTask();

			task.Guid = taskNode.Attributes["guid"].Value;
			if (string.IsNullOrEmpty(task.Guid))
				throw new XmlException("Invalid mirror task GUID.");

			task.Source = taskNode.SelectSingleNode("source").InnerText;

			var node = taskNode.SelectSingleNode("useVolumeShadowCopy");
			if (node != null)
				task.UseVolumeShadowCopy = bool.Parse(node.InnerText);

			node = taskNode.SelectSingleNode("exclusions");
			if (node != null)
			{
				foreach (XmlElement file in node.SelectNodes("file"))
					if (!string.IsNullOrEmpty(file.InnerText))
						task.ExcludedFiles.Add(file.InnerText);
				foreach (XmlElement folder in node.SelectNodes("folder"))
					if (!string.IsNullOrEmpty(folder.InnerText))
						task.ExcludedFolders.Add(folder.InnerText);

				// migrate from RoboMirror format prior to v1.0
				foreach (XmlElement item in node.SelectNodes("item"))
				{
					if (string.IsNullOrEmpty(item.InnerText))
						continue;

					if (item.InnerText[0] == Path.DirectorySeparatorChar &&
						Directory.Exists(task.Source + item.InnerText))
						task.ExcludedFolders.Add(item.InnerText);
					else
						task.ExcludedFiles.Add(item.InnerText);
				}
			}

			node = taskNode.SelectSingleNode("excludedAttributes");
			if (node != null)
				task.ExcludedAttributes = node.InnerText;

			task.Target = taskNode.SelectSingleNode("target").InnerText;

			node = taskNode.SelectSingleNode("extendedAttributes");
			if (node != null)
				task.ExtendedAttributes = node.InnerText;

			node = taskNode.SelectSingleNode("deleteExtraItems");
			if (node != null)
				task.DeleteExtraItems = bool.Parse(node.InnerText);

			node = taskNode.SelectSingleNode("lastBackup");
			if (node != null)
			{
				task.LastBackup = DateTime.ParseExact(node.InnerText, "u",
					System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();
			}

			return task;
		}
	}
}

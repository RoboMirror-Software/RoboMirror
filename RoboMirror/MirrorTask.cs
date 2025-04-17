/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RoboMirror
{
	/// <summary>
	/// Associates a source folder with a target folder, including the
	/// mirroring parameters and the last backup timestamp.
	/// </summary>
	public sealed class MirrorTask
	{
		#region Properties

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
		/// Gets or sets a value indicating whether newer files in the target folder
		/// are to be overwritten by older ones from the source folder.
		/// </summary>
		public bool OverwriteNewerFiles { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether extra files and folders in
		/// the target folder are to be deleted.
		/// </summary>
		public bool DeleteExtraItems { get; set; }


		/// <summary>
		/// Gets or sets optional custom Robocopy command-line switches to be used
		/// instead of the default ones.
		/// </summary>
		public string CustomRobocopySwitches { get; set; }

		/// <summary>
		/// Gets or sets the date and time of the last successful mirror operation.
		/// </summary>
		public DateTime? LastOperation { get; set; }

		#endregion


		public MirrorTask()
		{
			Guid = System.Guid.NewGuid().ToString();
			ExcludedFiles = new List<string>();
			ExcludedFolders = new List<string>();
		}


		/// <summary>
		/// Converts the task to an XML representation at the specified XML node.
		/// </summary>
		public void Serialize(XElement taskElement)
		{
			if (taskElement == null)
				throw new ArgumentNullException("taskElement");

			taskElement.SetAttributeValue("guid", Guid);

			taskElement.SetElementValue("source", Source);
			taskElement.SetElementValue("useVolumeShadowCopy", UseVolumeShadowCopy.ToString());

			if (ExcludedFiles.Count > 0)
			{
				var exclusionsElement = new XElement("exclusions");
				taskElement.Add(exclusionsElement);

				foreach (string exclusion in ExcludedFiles)
				{
					if (!string.IsNullOrEmpty(exclusion))
						exclusionsElement.Add(new XElement("file", exclusion));
				}
			}

			if (ExcludedFolders.Count > 0)
			{
				var exclusionsElement = taskElement.Element("exclusions");
				if (exclusionsElement == null)
				{
					exclusionsElement = new XElement("exclusions");
					taskElement.Add(exclusionsElement);
				}

				foreach (string exclusion in ExcludedFolders)
				{
					if (!string.IsNullOrEmpty(exclusion))
						exclusionsElement.Add(new XElement("folder", exclusion));
				}
			}

			taskElement.SetElementValue("excludedAttributes", ExcludedAttributes ?? string.Empty);
			taskElement.SetElementValue("target", Target);
			taskElement.SetElementValue("extendedAttributes", ExtendedAttributes ?? string.Empty);
			taskElement.SetElementValue("overwriteNewerFiles", OverwriteNewerFiles.ToString());
			taskElement.SetElementValue("deleteExtraItems", DeleteExtraItems.ToString());

			if (!string.IsNullOrEmpty(CustomRobocopySwitches))
				taskElement.SetElementValue("customRobocopySwitches", CustomRobocopySwitches);
		}

		/// <summary>
		/// Recreates a task from the specified XML node.
		/// </summary>
		public static MirrorTask Deserialize(XElement taskElement)
		{
			if (taskElement == null)
				throw new ArgumentNullException("taskElement");

			var task = new MirrorTask();

			task.Guid = taskElement.Attribute("guid").Value;
			task.Source = taskElement.Element("source").Value;

			var element = taskElement.Element("useVolumeShadowCopy");
			if (element != null)
				task.UseVolumeShadowCopy = bool.Parse(element.Value);

			element = taskElement.Element("exclusions");
			if (element != null)
			{
				foreach (var file in element.Elements("file"))
					if (!string.IsNullOrEmpty(file.Value))
						task.ExcludedFiles.Add(file.Value);
				foreach (var folder in element.Elements("folder"))
					if (!string.IsNullOrEmpty(folder.Value))
						task.ExcludedFolders.Add(folder.Value);

				// migrate from RoboMirror format prior to v1.0
				foreach (var item in element.Elements("item"))
				{
					if (string.IsNullOrEmpty(item.Value))
						continue;

					if (item.Value[0] == Path.DirectorySeparatorChar &&
						Directory.Exists(task.Source + item.Value))
						task.ExcludedFolders.Add(item.Value);
					else
						task.ExcludedFiles.Add(item.Value);
				}
			}

			element = taskElement.Element("excludedAttributes");
			if (element != null)
				task.ExcludedAttributes = element.Value;

			task.Target = taskElement.Element("target").Value;

			element = taskElement.Element("extendedAttributes");
			if (element != null)
				task.ExtendedAttributes = element.Value;

			element = taskElement.Element("overwriteNewerFiles");
			if (element != null)
				task.OverwriteNewerFiles = bool.Parse(element.Value);
			else // for backwards compatibility:
				task.OverwriteNewerFiles = true;

			element = taskElement.Element("deleteExtraItems");
			if (element != null)
				task.DeleteExtraItems = bool.Parse(element.Value);

			element = taskElement.Element("customRobocopySwitches");
			if (element != null)
				task.CustomRobocopySwitches = element.Value;

			// for backwards compatibility:
			element = taskElement.Element("lastOperation");
			if (element == null) // for further backwards compatibility
				element = taskElement.Element("lastBackup");
			if (element != null)
			{
				task.LastOperation = DateTime.ParseExact(element.Value, "u",
					System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();
			}

			return task;
		}
	}
}

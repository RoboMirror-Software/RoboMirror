/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace RoboMirror
{
	#region LogEntry class

	/// <summary>
	/// Represents a RoboMirror-specific event log entry.
	/// </summary>
	public class LogEntry : IComparable<LogEntry>
	{
		/// <summary>
		/// Gets the date and time when the entry has been generated.
		/// </summary>
		public DateTime TimeStamp { get; set; }

		/// <summary>
		/// Gets or sets the type of the entry.
		/// </summary>
		public EventLogEntryType Type { get; set; }

		/// <summary>
		/// Gets or sets the message of the entry.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the data associated with the entry.
		/// </summary>
		public string Data { get; set; }


		/// <summary>
		/// Sorts the entries descendingly by their timestamp.
		/// </summary>
		public int CompareTo(LogEntry other)
		{
 			return other.TimeStamp.CompareTo(TimeStamp);
		}
	}

	#endregion


	/// <summary>
	/// Provides an easy way of writing to a XML log.
	/// This class is thread-safe.
	/// </summary>
	public class Log : XmlFileManager
	{
		// path to the XML file, relative to the user's AppData folder
		private static readonly string PATH = Path.Combine("RoboMirror", "Log.xml");


		#region Static stuff

		public static void WriteEntry(string taskGuid, EventLogEntryType type,
			string message, string data)
		{
			if (string.IsNullOrEmpty(taskGuid))
				throw new ArgumentNullException("taskGuid");
			if (string.IsNullOrEmpty(message))
				throw new ArgumentNullException("message");

			var entry = new LogEntry()
			{
				TimeStamp = DateTime.Now,
				Type = type,
				Message = message,
				Data = data
			};

			// ~100 attempts with 100 ms delay to open the file for writing
			for (int i = 0; true; ++i)
			{
				try
				{
					using (var log = new Log(false))
					{
						log.Write(taskGuid, entry);
					}
					break;
				}
				catch (FileLockedException)
				{
					if (i >= 100)
						throw;

					System.Threading.Thread.Sleep(100);
				}
			}
		}

		public static List<LogEntry> LoadEntries(string taskGuid)
		{
			if (string.IsNullOrEmpty(taskGuid))
				throw new ArgumentNullException("taskGuid");

			using (var log = new Log(true))
			{
				return log.Load(taskGuid);
			}
		}


		/// <summary>
		/// Logs a Robocopy run.
		/// </summary>
		/// <param name="process">Terminated Robocopy process.</param>
		public static void LogRun(string taskGuid, RobocopyProcess process, string sourceFolder, string destinationFolder)
		{
			if (string.IsNullOrEmpty(taskGuid))
				throw new ArgumentNullException("taskGuid");
			if (process == null)
				throw new ArgumentNullException("process");
			if (string.IsNullOrEmpty(sourceFolder))
				throw new ArgumentNullException("sourceFolder");
			if (string.IsNullOrEmpty(destinationFolder))
				throw new ArgumentNullException("destinationFolder");

			EventLogEntryType type;
			string messageFormat;

			if (process.ExitCode == -1)
			{
				type = EventLogEntryType.Error;
				messageFormat = "Operation aborted while mirroring {0} to {1}.";
			}
			else if (process.ExitCode == 0)
			{
				type = EventLogEntryType.Information;
				messageFormat = "Already in sync: {0} and {1}";
			}
			else if (process.IsAnyExitFlagSet(RobocopyExitCodes.FatalError))
			{
				type = EventLogEntryType.Error;
				messageFormat = "A fatal error occurred while trying to mirror {0} to {1}.";
			}
			else if (process.IsAnyExitFlagSet(RobocopyExitCodes.CopyErrors))
			{
				type = EventLogEntryType.Error;
				messageFormat = "Some items could not be mirrored from {0} to {1}.";
			}
			else if (process.IsAnyExitFlagSet(RobocopyExitCodes.MismatchedItems))
			{
				type = EventLogEntryType.Warning;
				messageFormat = "Some file <-> folder mismatches while mirroring {0} to {1}.";
			}
			else
			{
				type = EventLogEntryType.Information;
				messageFormat = "Success: {0} mirrored to {1}";
			}

			string message = string.Format(messageFormat,
				PathHelper.Quote(sourceFolder),
				PathHelper.Quote(destinationFolder));

			WriteEntry(taskGuid, type, message, process.FullOutput);
		}

		#endregion


		/// <exception cref="FileLockedException"></exception>
		private Log(bool readOnly)
			: base(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PATH), "log", readOnly)
		{ }


		public void Write(string taskGuid, LogEntry entry)
		{
			if (string.IsNullOrEmpty(taskGuid))
				throw new ArgumentNullException("taskGuid");
			if (entry == null)
				throw new ArgumentNullException("entry");

			var entryNode = Document.CreateElement("entry");
			Document.DocumentElement.AppendChild(entryNode);

			var attribute = Document.CreateAttribute("taskGuid");
			attribute.Value = taskGuid;
			entryNode.Attributes.Append(attribute);

			var node = Document.CreateElement("timeStamp");
			node.InnerText = entry.TimeStamp.ToUniversalTime().ToString("u");
			entryNode.AppendChild(node);

			node = Document.CreateElement("type");
			node.InnerText = entry.Type.ToString();
			entryNode.AppendChild(node);

			node = Document.CreateElement("message");
			node.AppendChild(Document.CreateCDataSection(entry.Message));
			entryNode.AppendChild(node);

			if (!string.IsNullOrEmpty(entry.Data))
			{
				node = Document.CreateElement("data");
				node.AppendChild(Document.CreateCDataSection(entry.Data));
				entryNode.AppendChild(node);
			}

			Save();
		}

		public List<LogEntry> Load(string taskGuid)
		{
			if (string.IsNullOrEmpty(taskGuid))
				throw new ArgumentNullException("taskGuid");

			var entries = Document.DocumentElement.SelectNodes(
				string.Format("entry[@taskGuid=\"{0}\"]", taskGuid));

			var list = new List<LogEntry>(entries.Count);

			foreach (XmlNode entry in entries)
			{
				var item = new LogEntry();
				list.Add(item);

				item.TimeStamp = DateTime.ParseExact(entry.SelectSingleNode("timeStamp").InnerText,
					"u", System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();

				item.Type = (EventLogEntryType)Enum.Parse(typeof(EventLogEntryType), entry.SelectSingleNode("type").InnerText);

				item.Message = entry.SelectSingleNode("message").FirstChild.Value;

				var node = entry.SelectSingleNode("data");
				if (node != null)
					item.Data = node.FirstChild.Value;
			}

			list.Sort();

			return list;
		}
	}
}

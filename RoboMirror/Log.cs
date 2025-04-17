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
using System.Linq;
using System.Xml.Linq;

namespace RoboMirror
{
	/// <summary>
	/// Provides an easy way of writing to an XML log.
	/// The public static members of this class are thread-safe.
	/// </summary>
	public class Log : XmlFileManager
	{
		// path to the XML file, relative to the user's AppData folder
		private static readonly string PATH = Path.Combine("RoboMirror", "Log.xml");

		#region LogEntry class
		public class LogEntry
		{
			private Lazy<string> _data;

			public DateTime TimeStamp { get; private set; }
			public EventLogEntryType Type { get; private set; }
			public string Message { get; private set; }
			public string Data { get { return _data.Value; } }

			public LogEntry(DateTime timeStamp, EventLogEntryType type, string message, string data)
			{
				if (message == null)
					throw new ArgumentNullException("message");

				TimeStamp = timeStamp;
				Type = type;
				Message = message;
				_data = new Lazy<string>(() => data, isThreadSafe: false);
			}

			public LogEntry(DateTime timeStamp, EventLogEntryType type, string message, Func<string> dataFactory)
			{
				if (message == null)
					throw new ArgumentNullException("message");

				TimeStamp = timeStamp;
				Type = type;
				Message = message;
				_data = new Lazy<string>(dataFactory, isThreadSafe: false);
			}
		}
		#endregion

		#region Static stuff

		public static LogEntry WriteEntry(string taskGuid, EventLogEntryType type,
			string message, string data, bool updateLastSuccessTimeStamp)
		{
			if (string.IsNullOrEmpty(taskGuid))
				throw new ArgumentNullException("taskGuid");
			if (string.IsNullOrEmpty(message))
				throw new ArgumentNullException("message");

			var entry = new LogEntry(DateTime.Now, type, message, data);

			// FileLock.RETRY_ATTEMPTS with FileLock.RETRY_DELAY to open the file for writing
			for (int i = 0; true; ++i)
			{
				try
				{
					using (var log = new Log(readOnly: false))
						log.Write(taskGuid, entry, updateLastSuccessTimeStamp);

					break;
				}
				catch (FileLockedException)
				{
					if (i >= FileLock.RETRY_ATTEMPTS)
						throw;

					System.Threading.Thread.Sleep(FileLock.RETRY_DELAY);
				}
			}

			return entry;
		}

		/// <summary>Logs a Robocopy run.</summary>
		/// <param name="process">Terminated Robocopy process.</param>
		public static LogEntry LogRun(string taskGuid, RobocopyProcess process, string sourceFolder,
			string destinationFolder, bool updateLastSuccessTimeStamp)
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

			return WriteEntry(taskGuid, type, message, process.FullOutput,
				updateLastSuccessTimeStamp);
		}

		public static List<LogEntry> LoadEntries(string taskGuid)
		{
			if (string.IsNullOrEmpty(taskGuid))
				throw new ArgumentNullException("taskGuid");

			using (var log = new Log(readOnly: true))
				return log.Load(taskGuid);
		}

		public static void LoadLastSuccessTimeStamps(IEnumerable<MirrorTask> tasks)
		{
			if (tasks == null)
				throw new ArgumentNullException("tasks");

			using (var log = new Log(readOnly: true))
			{
				foreach (var task in tasks)
				{
					var lastSuccess = log.GetLastSuccessTimeStamp(task.Guid);
					if (lastSuccess.HasValue)
						task.LastOperation = lastSuccess.Value;
				}
			}
		}

		#endregion


		/// <exception cref="FileLockedException"></exception>
		private Log(bool readOnly)
			: base(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PATH), "log", readOnly)
		{
			// upgrade to new format:
			// move /log/entry elements to /log/task[@guid=...]/entry
			var flatEntries = RootElement.Elements("entry");
			foreach (var e in flatEntries)
			{
				e.Remove();

				string taskGuid = e.Attribute("taskGuid").Value;
				e.SetAttributeValue("taskGuid", null);

				var taskElement = GetTaskElement(taskGuid, create: true);
				taskElement.Add(e);
			}
		}

		private XElement GetTaskElement(string taskGuid, bool create)
		{
			var element = RootElement.Elements("task").SingleOrDefault(
				e => e.Attribute("guid").Value == taskGuid);

			if (element == null && create)
			{
				element = new XElement("task");
				element.SetAttributeValue("guid", taskGuid);
				RootElement.Add(element);
			}

			return element;
		}


		#region Last success time stamp

		private static string SerializeDateTime(DateTime dateTime)
		{
			return dateTime.ToUniversalTime().ToString("u", System.Globalization.CultureInfo.InvariantCulture);
		}

		private static DateTime ParseDateTime(string text)
		{
			return DateTime.ParseExact(text, "u", System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();
		}

		public DateTime? GetLastSuccessTimeStamp(string taskGuid)
		{
			if (string.IsNullOrEmpty(taskGuid))
				throw new ArgumentNullException("taskGuid");

			var taskElement = GetTaskElement(taskGuid, create: false);
			if (taskElement == null)
				return null;

			var attribute = taskElement.Attribute("lastSuccess");
			if (attribute == null)
				return null;

			return ParseDateTime(attribute.Value);
		}

		private void UpdateLastSuccessTimeStamp(XElement taskElement, DateTime lastSuccess)
		{
			var attribute = taskElement.Attribute("lastSuccess");
			if (attribute == null || lastSuccess > ParseDateTime(attribute.Value))
				taskElement.SetAttributeValue("lastSuccess", SerializeDateTime(lastSuccess));
		}

		#endregion

		#region Writing

		public void Write(string taskGuid, LogEntry entry, bool updateLastSuccessTimeStamp)
		{
			if (string.IsNullOrEmpty(taskGuid))
				throw new ArgumentNullException("taskGuid");
			if (entry == null)
				throw new ArgumentNullException("entry");

			var taskElement = GetTaskElement(taskGuid, create: true);

			if (updateLastSuccessTimeStamp)
				UpdateLastSuccessTimeStamp(taskElement, entry.TimeStamp);

			var entryElement = new XElement("entry");
			taskElement.Add(entryElement);

			entryElement.SetElementValue("timeStamp", SerializeDateTime(entry.TimeStamp));
			entryElement.SetElementValue("type", entry.Type.ToString());
			entryElement.Add(new XElement("message", new XCData(entry.Message)));

			if (!string.IsNullOrEmpty(entry.Data))
			{
				string dataFilePath = SaveData(taskGuid, entry.Data);
				entryElement.SetElementValue("dataRef", Path.GetFileName(dataFilePath));
			}

			Save();
		}

		/// <summary>Saves an entry's data in a new separate file and returns its path.</summary>
		private string SaveData(string taskGuid, string data)
		{
			string taskSubfolder = GetTaskSubfolder(taskGuid);
			Directory.CreateDirectory(taskSubfolder);

			string filePath;
			for (int i = 1; true; ++i)
			{
				filePath = Path.Combine(taskSubfolder, i + ".log");
				if (File.Exists(filePath))
					continue;

				using (var stream = File.Open(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
				{
					using (var writer = new StreamWriter(stream))
					{
						writer.Write(data);
						break;
					}
				}
			}

			return filePath;
		}

		private string GetTaskSubfolder(string taskGuid)
		{
			string baseFolder = Path.GetDirectoryName(FilePath);
			return Path.Combine(baseFolder, taskGuid);
		}

		#endregion

		#region Loading

		public List<LogEntry> Load(string taskGuid)
		{
			if (string.IsNullOrEmpty(taskGuid))
				throw new ArgumentNullException("taskGuid");

			var list = new List<LogEntry>();

			var taskElement = GetTaskElement(taskGuid, create: false);
			if (taskElement == null)
				return list;

			list.AddRange(from entry in taskElement.Elements("entry") select ParseEntry(taskGuid, entry));

			// sort descendingly by time stamp
			list.Sort((a, b) => { return -a.TimeStamp.CompareTo(b.TimeStamp); });

			return list;
		}

		private LogEntry ParseEntry(string taskGuid, XElement entry)
		{
			var timeStamp = ParseDateTime(entry.Element("timeStamp").Value);

			var type = (EventLogEntryType)Enum.Parse(typeof(EventLogEntryType), entry.Element("type").Value);

			string message = entry.Element("message").DescendantNodes().OfType<XCData>().First().Value;

			LogEntry item;
			var dataRef = entry.Element("dataRef");
			if (dataRef != null)
			{
				string fileName = dataRef.Value;
				string taskSubfolder = GetTaskSubfolder(taskGuid);
				string filePath = Path.Combine(taskSubfolder, fileName);

				item = new LogEntry(timeStamp, type, message, () => File.ReadAllText(filePath));
			}
			else // no <dataRef> node; look for <data> node
			{
				var dataElement = entry.Element("data");
				string data = (dataElement == null ? null : dataElement.DescendantNodes().OfType<XCData>().First().Value);

				item = new LogEntry(timeStamp, type, message, data);
			}

			return item;
		}

		#endregion
	}
}

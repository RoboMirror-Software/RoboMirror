/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace RoboMirror
{
	/// <summary>
	/// Base class for managers of atomic XML files.
	/// A read-only manager opens an existing or new XML file (not preventing other readers
	/// or writers from opening it too), locks it, parses it, unlocks it and then immediately
	/// closes the file.
	/// A read-write manager, on the other hand, tries to open the file and, if successful,
	/// prevents other writers from opening it until the manager is explicitly disposed of.
	/// Read and write operations between (in the worst case) multiple readers and a single
	/// writer are synchronized via the FileLock class.
	/// </summary>
	public class XmlFileManager : IDisposable
	{
		private FileStream _file;

		protected string FilePath { get; private set; }
		protected XElement RootElement { get; private set; }

		/// <exception cref="FileLockedException"></exception>
		protected XmlFileManager(string path, string rootTag, bool readOnly)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException("path");
			if (string.IsNullOrEmpty(rootTag))
				throw new ArgumentNullException("rootTag");

			FilePath = path;

			// make sure the folder exists
			string folder = Path.GetDirectoryName(FilePath);
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			// open/create the file
			try
			{
				_file = File.Open(FilePath, FileMode.OpenOrCreate,
					readOnly ? FileAccess.Read : FileAccess.ReadWrite,
					readOnly ? FileShare.ReadWrite : FileShare.Read);
			}
			catch (IOException e)
			{
				throw new FileLockedException(FilePath, e);
			}

			// parse it
			var document = new XDocument();
			try
			{
				using (var fileLock = new FileLock(_file))
				{
					if (_file.Length > 0)
						document = XDocument.Load(_file);
					else // initialize the XML document with the root element
						document = new XDocument(new XElement(rootTag));
				}
			}
			catch (FileLockedException)
			{
				Dispose();
				throw;
			}
			catch (Exception e)
			{
				Dispose();
				throw new InvalidDataException(string.Format("{0} is corrupt.", PathHelper.Quote(FilePath)), e);
			}

			RootElement = document.Root;

			if (readOnly)
				Dispose();
		}

		/// <summary>Releases the file.</summary>
		public void Dispose()
		{
			if (_file != null)
			{
				_file.Dispose();
				_file = null;
			}
		}

		/// <summary>Saves the XML document to the file.</summary>
		protected void Save()
		{
			if (_file == null)
				throw new InvalidOperationException("A read-only XML file manager cannot Save().");

			using (var fileLock = new FileLock(_file))
			{
				// reset the stream
				_file.Position = 0;
				_file.SetLength(0);

				RootElement.Document.Save(_file);

				_file.Flush();
			}
		}
	}
}

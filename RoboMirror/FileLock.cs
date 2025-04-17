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
	#region FileLockedException
	public class FileLockedException : Exception
	{
		public FileLockedException()
			: this((Exception)null)
		{ }

		public FileLockedException(Exception innerException)
			: base("A file is locked.", innerException)
		{ }

		public FileLockedException(string path)
			: this(path, null)
		{ }

		public FileLockedException(string path, Exception innerException)
			: base(string.Format("The file {0} is locked.", PathHelper.Quote(path)), innerException)
		{ }
	}
	#endregion

	/// <summary>
	/// Attempts to lock a given file stream (the whole stream) until the FileLock object is disposed of.
	/// </summary>
	public class FileLock : IDisposable
	{
		private FileStream _file;
		private long _length;

		/// <param name="retryAttempts">Maximum number of retry attempts, each delayed by 100 ms.</param>
		/// <exception cref="FileLockedException"></exception>
		public FileLock(FileStream file, int retryAttempts = 100)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			_file = file;

			for (int i = 0; true; ++i)
			{
				try
				{
					_length = _file.Length;
					_file.Lock(0, _length);
					break;
				}
				catch (IOException e)
				{
					if (i >= retryAttempts)
						throw new FileLockedException(e);

					System.Threading.Thread.Sleep(100);
				}
			}
		}

		public void Dispose()
		{
			_file.Unlock(0, _length);
		}
	}
}

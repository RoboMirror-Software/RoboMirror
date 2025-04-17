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
	#region FileLockedException class

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
		public const int RETRY_ATTEMPTS = 100;
		public const int RETRY_DELAY = 100; // ms

		private FileStream _file;

		/// <param name="retryAttempts">Maximum number of retry attempts, each delayed by RETRY_DELAY ms.</param>
		/// <exception cref="FileLockedException"></exception>
		public FileLock(FileStream file, int retryAttempts = RETRY_ATTEMPTS)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			_file = file;

			for (int i = 0; true; ++i)
			{
				try
				{
					_file.Lock(0, long.MaxValue);
					break;
				}
				catch (IOException e)
				{
					if (i >= retryAttempts)
						throw new FileLockedException(e);

					System.Threading.Thread.Sleep(RETRY_DELAY);
				}
			}
		}

		public void Dispose()
		{
			_file.Unlock(0, long.MaxValue);
		}
	}
}

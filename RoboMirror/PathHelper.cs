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
	public static class PathHelper
	{
		/// <summary>
		/// Corrects a path provided by the user.
		/// </summary>
		public static string CorrectPath(string path)
		{
			if (string.IsNullOrEmpty(path))
				return path;

			path = path.Replace('/', Path.DirectorySeparatorChar) // replace all forward slashes
				.Replace('\\', Path.DirectorySeparatorChar)       // replace all backslashes
				.Trim('"');                                       // eliminate all leading and trailing double-quotes

			// X: => X:\
			if (path.Length == 2 && char.IsLetter(path[0]) && path[1] == Path.VolumeSeparatorChar)
				return path + Path.DirectorySeparatorChar;

			try
			{
				// try to make absolute and remove redundant .. and . subpaths etc.
				return Path.GetFullPath(path);
			}
			catch { return path; }
		}

		/// <summary>
		/// Returns true if the specified path is contained by the specified folder or the folder itself.
		/// </summary>
		/// <param name="relativePath">Set to the resulting relative path, always beginning with a directory separator character.</param>
		public static bool IsInFolder(string path, string folder, out string relativePath)
		{
			if (string.IsNullOrEmpty(folder))
				throw new ArgumentNullException("folder");

			relativePath = null;

			if (string.IsNullOrEmpty(path))
				return false;

			// use a case-insensitive comparison under Windows
			var comparison = (Path.DirectorySeparatorChar == '\\' ?
				StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

			string prefix = folder;
			if (!prefix.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
				prefix += Path.DirectorySeparatorChar;

			if (!path.StartsWith(prefix, comparison))
				return false;

			relativePath = path.Substring(prefix.Length - 1);
			return true;
		}

		/// <summary>
		/// Encloses a path in double-quotes if it contains spaces.
		/// </summary>
		public static string QuotePath(string path, bool force = false)
		{
			if (string.IsNullOrEmpty(path))
				return path;

			if (!force && path.IndexOf(' ') < 0)
				return path;

			return '"' + path +
				// \" is interpreted as escaped double-quote, so if the path ends with \, we need to escape the backslash => \\"
				(path[path.Length - 1] == '\\' ? "\\\"" : "\"");
		}
	}
}

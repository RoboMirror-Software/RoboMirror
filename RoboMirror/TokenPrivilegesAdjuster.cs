/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RoboMirror
{
	public static class TokenPrivilegesAdjuster
	{
		#region WinAPI

		private struct LUID
		{
			public int LowPart;
			public int HighPart;
		}
		private struct LUID_AND_ATTRIBUTES
		{
			public LUID Luid;
			public int Attributes;
		}
		private struct TOKEN_PRIVILEGES
		{
			public int PrivilegeCount;
			public LUID_AND_ATTRIBUTES Privileges;
		}

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, out IntPtr TokenHandle);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AdjustTokenPrivileges(
			IntPtr TokenHandle,
			[MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
			ref TOKEN_PRIVILEGES NewState,
			int BufferLength,
			IntPtr PreviousState,
			IntPtr ReturnLength);

		#endregion

		public static bool Enable(string privilegeName)
		{
			if (string.IsNullOrEmpty(privilegeName))
				throw new ArgumentNullException("privilegeName");

			const int TOKEN_ADJUST_PRIVILEGES = 0x20;
			const short SE_PRIVILEGE_ENABLED = 2;

			IntPtr processHandle = GetCurrentProcess(); // doesn't need to be closed

			IntPtr hToken;
			if (!OpenProcessToken(processHandle, TOKEN_ADJUST_PRIVILEGES, out hToken))
				return false;

			TOKEN_PRIVILEGES tp;
			tp.PrivilegeCount = 1;
			tp.Privileges.Attributes = SE_PRIVILEGE_ENABLED;
			bool success = LookupPrivilegeValue(null, privilegeName, out tp.Privileges.Luid);

			success = success && AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);

			CloseHandle(hToken);

			return success;
		}
	}
}

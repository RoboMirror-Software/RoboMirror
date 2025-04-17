/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Security.Principal;

namespace RoboMirror
{
	public static class UacHelper
	{
		public static bool IsInAdminRole()
		{
			// user needs to have elevated Admin privileges
			var wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			return wp.IsInRole(WindowsBuiltInRole.Administrator);
		}

		public static bool IsRobocopyBackupModeSupported()
		{
			// user needs to be in "Backup Operators" group or have elevated Admin privileges
			var wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			return wp.IsInRole(WindowsBuiltInRole.BackupOperator) || wp.IsInRole(WindowsBuiltInRole.Administrator);
		}
	}
}

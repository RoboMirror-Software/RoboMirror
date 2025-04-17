/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Windows.Forms;

namespace RoboMirror
{
	/// <summary>
	/// Base of all forms.
	/// Mainly used for the default font.
	/// </summary>
	public partial class BaseForm : Form
	{
		public BaseForm()
		{
			InitializeComponent();
		}
	}
}

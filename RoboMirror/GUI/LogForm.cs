/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Windows.Forms;

namespace RoboMirror.GUI
{
	/// <summary>
	/// Simple form containing only a read-only text box.
	/// </summary>
	public partial class LogForm : BaseForm
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="caption">Caption of the form.</param>
		/// <param name="text">Text box content.</param>
		public LogForm(string caption, string text)
		{
			InitializeComponent();

			Text = caption;
			richTextBox1.Text = text;
		}

		protected override void OnShown(EventArgs e)
		{
			// scroll to the end
			richTextBox1.Select(richTextBox1.Text.Length, 0);
			richTextBox1.ScrollToCaret();

			base.OnShown(e);
		}

		private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}
	}
}

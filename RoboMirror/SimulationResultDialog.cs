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
	/// Displays the result of a Robocopy simulation process and prompts the user
	/// to confirm the pending changes.
	/// </summary>
	public partial class SimulationResultDialog : BaseForm
	{
		/// <summary>
		/// Creates a new dialog.
		/// </summary>
		/// <param name="process">Successfully completed simulation process.</param>
		/// <param name="destination">Folder which will be modified.</param>
		public SimulationResultDialog(RobocopyProcess process, string destination)
		{
			if (process == null)
				throw new ArgumentNullException("process");

			InitializeComponent();

			var boldFont = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold);
			numFilesTransferredLabel.Font = boldFont;
			numDeletionsLabel.Font = boldFont;

			label1.Text = string.Format(label1.Text, destination);

			numFilesTransferredLabel.Text = process.TransfersCount.ToString();

			if (process.DeletionsCount > 0)
			{
				numDeletionsLabel.Text = process.DeletionsCount.ToString();
				numDeletionsLabel.ForeColor = System.Drawing.Color.Red;
			}

			richTextBox1.Text = process.FullOutput;
		}

		protected override void OnShown(EventArgs e)
		{
			// scroll to the end
			richTextBox1.Select(richTextBox1.Text.Length, 0);
			richTextBox1.ScrollToCaret();

			// put focus away from text box
			label1.Focus();

			base.OnShown(e);
		}
	}
}

/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace RoboMirror.GUI
{
	/// <summary>
	/// Displays the result of a Robocopy simulation process and prompts the user
	/// to confirm the pending changes.
	/// </summary>
	public partial class SimulationResultDialog : BaseForm
	{
		private static readonly Color RED = Color.FromArgb(208, 0, 0);
		private static readonly Color GREEN = Color.FromArgb(0, 128, 0);

		/// <param name="process">Successfully completed simulation process.</param>
		public SimulationResultDialog(RobocopyProcess process)
		{
			if (process == null)
				throw new ArgumentNullException("process");

			InitializeComponent();

			var boldFont = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold);
			copiedFoldersLabel.Font = boldFont;
			deletedFoldersLabel.Font = boldFont;
			copiedFilesLabel.Font = boldFont;
			deletedFilesLabel.Font = boldFont;

			label1.Text = string.Format(label1.Text, PathHelper.Quote(process.DestinationFolder));

			SetupFolderLabels(process);
			SetupFileLabels(process);

			richTextBox1.Text = process.FullOutput;
		}

		private void SetupFolderLabels(RobocopyProcess process)
		{
			string copied = process.GetSummary(RobocopySummaryRow.Dirs, RobocopySummaryColumn.Copied);
			string total = process.GetSummary(RobocopySummaryRow.Dirs, RobocopySummaryColumn.Total);
			string extras = process.GetSummary(RobocopySummaryRow.Dirs, RobocopySummaryColumn.Extras);

			copiedFoldersLabel.Text = string.Format("{0} out of {1}", copied, total);
			if (!IsZero(copied))
				copiedFoldersLabel.ForeColor = GREEN;

			if (process.PurgeExtraItems)
			{
				deletedFoldersLabel.Text = extras;
				if (!IsZero(extras))
					deletedFoldersLabel.ForeColor = RED;
			}
			else
				deletedFoldersLabel.Text = string.Format("{0} out of {1}", 0, extras);
		}

		private void SetupFileLabels(RobocopyProcess process)
		{
			string copied = process.GetSummary(RobocopySummaryRow.Files, RobocopySummaryColumn.Copied);
			string total = process.GetSummary(RobocopySummaryRow.Files, RobocopySummaryColumn.Total);
			string extras = process.GetSummary(RobocopySummaryRow.Files, RobocopySummaryColumn.Extras);

			string copiedBytes = GetBytes(process, RobocopySummaryColumn.Copied);
			string totalBytes = GetBytes(process, RobocopySummaryColumn.Total);
			string extraBytes = GetBytes(process, RobocopySummaryColumn.Extras);

			copiedFilesLabel.Text = string.Format("{0} out of {1}\r\n{2} out of {3}", copied, total, copiedBytes, totalBytes);
			if (!IsZero(copied))
				copiedFilesLabel.ForeColor = GREEN;

			if (process.PurgeExtraItems)
			{
				string text = extras;
				if (!IsZero(extras))
				{
					text += string.Format("\r\n{0}", extraBytes);
					deletedFilesLabel.ForeColor = RED;
				}
				deletedFilesLabel.Text = text;
			}
			else
				deletedFilesLabel.Text = string.Format("{0} out of {1}", 0, extras);
		}

		private static string GetBytes(RobocopyProcess process, RobocopySummaryColumn column)
		{
			string rawValue = process.GetSummary(RobocopySummaryRow.Bytes, column);
			string separator = (rawValue.Contains(" ") ? string.Empty : " ");
			return string.Format("{0}{1}bytes", rawValue, separator);
		}

		private static bool IsZero(string summaryField)
		{
			return summaryField == (0).ToString();
		}

		protected override void OnShown(EventArgs e)
		{
			// scroll to the end
			richTextBox1.Select(richTextBox1.Text.Length, 0);
			richTextBox1.ScrollToCaret();

			// steal focus from text box
			label1.Focus();

			base.OnShown(e);
		}
	}
}

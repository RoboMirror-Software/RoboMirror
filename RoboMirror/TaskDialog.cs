/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace RoboMirror
{
	/// <summary>
	/// Allows editing of a mirror task.
	/// </summary>
	public partial class TaskDialog : BaseDialog
	{
		private MirrorTask _task;

		private ExcludedItemsDialog _excludedItemsDialog;


		public TaskDialog(MirrorTask task)
		{
			if (task == null)
				throw new ArgumentNullException("task");

			_task = task;

			InitializeComponent();

			var boldFont = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold);
			sourceLabel.Font = boldFont;
			targetLabel.Font = boldFont;

			var sourceFileDropTargetWrapper = new FileDropTargetWrapper(sourceFolderTextBox, FileDropMode.Folder);
			sourceFileDropTargetWrapper.FilesDropped += (s, e) => { sourceFolderTextBox.Text = e.Paths[0]; };

			var targetFileDropTargetWrapper = new FileDropTargetWrapper(targetFolderTextBox, FileDropMode.Folder);
			targetFileDropTargetWrapper.FilesDropped += (s, e) => { targetFolderTextBox.Text = e.Paths[0]; };

			// set up the source
			if (!string.IsNullOrEmpty(_task.Source))
				sourceFolderTextBox.Text = _task.Source;

			// persistent volume shadow copies are supported since Windows Vista
			if (Environment.OSVersion.Version.Major < 6)
				vscCheckBox.Enabled = false;
			else
				vscCheckBox.Checked = _task.UseVolumeShadowCopy;

			// set up the target
			if (!string.IsNullOrEmpty(_task.Target))
				targetFolderTextBox.Text = _task.Target;

			if (!string.IsNullOrEmpty(_task.ExtendedAttributes))
			{
				if (_task.ExtendedAttributes == "S")
					aclsOnlyRadioButton.Checked = true;
				else if (_task.ExtendedAttributes.Length == 3)
					allRadioButton.Checked = true;
			}

			overwriteNewerFilesCheckBox.Checked = _task.OverwriteNewerFiles;
			deleteExtraItemsCheckBox.Checked = _task.DeleteExtraItems;

			if (!string.IsNullOrEmpty(_task.CustomRobocopySwitches))
			{
				robocopySwitchesCheckBox.Checked = true;
				robocopySwitchesTextBox.Text = _task.CustomRobocopySwitches;
			}
		}


		private void sourceFolderTextBox_TextChanged(object sender, EventArgs e)
		{
			excludedItemsButton.Enabled = Directory.Exists(PathHelper.CorrectPath(sourceFolderTextBox.Text));
			HasChanged = true;
		}

		private void browseSourceFolderButton_Click(object sender, EventArgs e)
		{
			sourceFolderBrowserDialog.SelectedPath = PathHelper.CorrectPath(sourceFolderTextBox.Text);

			if (sourceFolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				sourceFolderTextBox.Text = sourceFolderBrowserDialog.SelectedPath;
				HasChanged = true;
			}
		}

		private void excludedItemsButton_Click(object sender, EventArgs e)
		{
			if (_excludedItemsDialog == null)
				_excludedItemsDialog = new ExcludedItemsDialog(_task);

			if (_excludedItemsDialog.ShowDialog(this, PathHelper.CorrectPath(sourceFolderTextBox.Text)) == DialogResult.Yes)
				HasChanged = true;
			else
			{
				_excludedItemsDialog.Dispose();
				_excludedItemsDialog = null;
			}
		}

		private void browseTargetFolderButton_Click(object sender, EventArgs e)
		{
			targetFolderBrowserDialog.SelectedPath = PathHelper.CorrectPath(targetFolderTextBox.Text);

			if (targetFolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				targetFolderTextBox.Text = targetFolderBrowserDialog.SelectedPath;
				HasChanged = true;
			}
		}

		private void RadioButton_CheckedChanged(object sender, EventArgs e)
		{
			var button = sender as RadioButton;

			if (!button.Checked)
				return;

			foreach (RadioButton b in groupBox1.Controls)
			{
				if (b == button)
					continue;

				b.Checked = false;
			}

			HasChanged = true;
		}

		private void robocopySwitchesCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			robocopySwitchesTextBox.Enabled = robocopySwitchesCheckBox.Checked;

			if (robocopySwitchesTextBox.Enabled && robocopySwitchesTextBox.TextLength == 0)
				robocopySwitchesTextBox.Text = Properties.Settings.Default.RobocopySwitches;

			HasChanged = true;
		}

		private void Control_Changed(object sender, EventArgs e)
		{
			HasChanged = true;
		}


		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			if (_excludedItemsDialog != null)
				_excludedItemsDialog.Dispose();
		}

		protected override bool ApplyChanges()
		{
			// check if the changes are valid

			string source = PathHelper.CorrectPath(sourceFolderTextBox.Text);
			if (!Directory.Exists(source))
			{
				MessageBox.Show("The source folder does not exist.", "Invalid source folder",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				sourceFolderTextBox.Focus();
				return false;
			}

			string target = PathHelper.CorrectPath(targetFolderTextBox.Text);
			if (!Directory.Exists(target))
			{
				MessageBox.Show("The target folder does not exist.", "Invalid target folder",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				targetFolderTextBox.Focus();
				return false;
			}

			string relativePath;
			if (PathHelper.IsInFolder(target, source, out relativePath))
			{
				MessageBox.Show("The target folder must not be in the source folder.",
					"Invalid target folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
				targetFolderTextBox.Focus();
				return false;
			}

			// apply the changes

			_task.Source = source;
			_task.Target = target;

			if (_excludedItemsDialog != null)
			{
				_task.ExcludedFiles.Clear();
				_task.ExcludedFiles.AddRange(_excludedItemsDialog.ExcludedFiles);

				_task.ExcludedFolders.Clear();
				_task.ExcludedFolders.AddRange(_excludedItemsDialog.ExcludedFolders);

				_task.ExcludedAttributes = _excludedItemsDialog.ExcludedAttributes;
			}

			_task.UseVolumeShadowCopy = vscCheckBox.Checked;

			if (allRadioButton.Checked)
				_task.ExtendedAttributes = "SOU";
			else if (aclsOnlyRadioButton.Checked)
				_task.ExtendedAttributes = "S";
			else
				_task.ExtendedAttributes = string.Empty;

			_task.OverwriteNewerFiles = overwriteNewerFilesCheckBox.Checked;
			_task.DeleteExtraItems = deleteExtraItemsCheckBox.Checked;

			_task.CustomRobocopySwitches = (robocopySwitchesCheckBox.Checked ? robocopySwitchesTextBox.Text : null);

			return true;
		}
	}
}

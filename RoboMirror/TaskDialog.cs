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


		/// <summary>
		/// Creates a new dialog.
		/// </summary>
		/// <param name="task">Task to be edited.</param>
		public TaskDialog(MirrorTask task)
		{
			if (task == null)
				throw new ArgumentNullException("task");

			_task = task;

			InitializeComponent();

			// set up the source
			if (!string.IsNullOrEmpty(_task.Source))
				sourceFolderTextBox.Text = _task.Source;

			// set up the target
			if (!string.IsNullOrEmpty(_task.Target))
				targetFolderTextBox.Text = _task.Target;

			// persistent volume shadow copies are supported since Windows Vista
			if (Environment.OSVersion.Version.Major < 6)
				vscCheckBox.Enabled = false;
			else
				vscCheckBox.Checked = _task.UseVolumeShadowCopy;

			if (!string.IsNullOrEmpty(_task.ExtendedAttributes))
			{
				if (_task.ExtendedAttributes == "S")
					aclsOnlyRadioButton.Checked = true;
				else if (_task.ExtendedAttributes.Length == 3)
					allRadioButton.Checked = true;
			}
		}


		private void sourceFolderTextBox_TextChanged(object sender, EventArgs e)
		{
			excludedItemsButton.Enabled = Directory.Exists(sourceFolderTextBox.Text);

			// vshadow.exe cannot create volume shadow copies of network shares
			if (sourceFolderTextBox.Text.StartsWith(@"\\", StringComparison.Ordinal))
			{
				vscCheckBox.Checked = false;
				vscCheckBox.Enabled = false;
			}
			else
				vscCheckBox.Enabled = true;

			HasChanged = true;
		}

		private void browseSourceFolderButton_Click(object sender, EventArgs e)
		{
			sourceFolderBrowserDialog.SelectedPath = sourceFolderTextBox.Text;

			if (sourceFolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				sourceFolderTextBox.Text = sourceFolderBrowserDialog.SelectedPath;
				HasChanged = true;
			}
		}

		private void excludedItemsButton_Click(object sender, EventArgs e)
		{
			if (_excludedItemsDialog == null)
			{
				_excludedItemsDialog = new ExcludedItemsDialog(_task,
					GetFullPath(sourceFolderTextBox.Text));
			}

			if (_excludedItemsDialog.ShowDialog(this) == DialogResult.Yes)
				HasChanged = true;
			else
			{
				_excludedItemsDialog.Dispose();
				_excludedItemsDialog = null;
			}
		}

		private void browseTargetFolderButton_Click(object sender, EventArgs e)
		{
			targetFolderBrowserDialog.SelectedPath = targetFolderTextBox.Text;

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

		private void Control_Changed(object sender, EventArgs e)
		{
			HasChanged = true;
		}


		private static string GetFullPath(string path)
		{
			string fullPath = Path.GetFullPath(path);
			if (fullPath[fullPath.Length - 1] == Path.DirectorySeparatorChar)
				fullPath = fullPath.Remove(fullPath.Length - 1);

			return fullPath;
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

			string source = sourceFolderTextBox.Text;
			if (!Directory.Exists(source))
			{
				MessageBox.Show("The source folder does not exist.", "Invalid source folder",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				sourceFolderTextBox.Focus();
				return false;
			}
			source = GetFullPath(source);

			string target = targetFolderTextBox.Text;
			if (!Directory.Exists(target))
			{
				MessageBox.Show("The target folder does not exist.", "Invalid target folder",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				targetFolderTextBox.Focus();
				return false;
			}
			target = GetFullPath(target);

			// use a case-insensitive path comparison under Windows
			var comparison = (Path.DirectorySeparatorChar == '\\' ?
				StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

			if (target.Length >= source.Length &&
			    string.Compare(target + Path.DirectorySeparatorChar, 0,
						source + Path.DirectorySeparatorChar, 0, source.Length + 1, comparison) == 0)
			{
				MessageBox.Show("The target folder cannot be in the source folder.",
					"Invalid target folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
				targetFolderTextBox.Focus();
				return false;
			}

			// apply the changes

			// reset the last synchronization timestamp if source or target have changed
			if (string.Compare(_task.Source, source, comparison) != 0 ||
			    string.Compare(_task.Target, target, comparison) != 0)
				_task.LastBackup = null;

			_task.Source = source;
			_task.Target = target;

			if (_excludedItemsDialog != null)
			{
				_task.Exclusions.Clear();
				_task.Exclusions.AddRange(_excludedItemsDialog.ExcludedItems);

				_task.ExcludedAttributes = _excludedItemsDialog.ExcludedAttributes;
			}

			_task.UseVolumeShadowCopy = vscCheckBox.Checked;

			if (allRadioButton.Checked)
				_task.ExtendedAttributes = "SOU";
			else if (aclsOnlyRadioButton.Checked)
				_task.ExtendedAttributes = "S";
			else
				_task.ExtendedAttributes = string.Empty;

			return true;
		}
	}
}

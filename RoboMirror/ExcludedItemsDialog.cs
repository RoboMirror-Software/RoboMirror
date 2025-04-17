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
	/// Allows management of subfolders and files to be excluded
	/// from a mirror task.
	/// </summary>
	public partial class ExcludedItemsDialog : BaseDialog
	{
		private string _sourceFolder;


		/// <summary>
		/// Gets the list of excluded items.
		/// </summary>
		public List<string> ExcludedItems { get; private set; }

		/// <summary>
		/// Gets the string encoding the excluded attributes (RASHCNETO).
		/// </summary>
		public string ExcludedAttributes { get; private set; }


		/// <summary>
		/// Creates a new dialog.
		/// </summary>
		/// <param name="task">Task whose excluded items are to be edited.</param>
		/// <param name="sourceFolder">Path to the current source folder.</param>
		public ExcludedItemsDialog(MirrorTask task, string sourceFolder)
		{
			if (task == null)
				throw new ArgumentNullException("task");

			_sourceFolder = sourceFolder;
			if (!Directory.Exists(_sourceFolder))
				throw new InvalidOperationException("The source folder does not exist.");

			ExcludedItems = new List<string>(task.Exclusions);
			ExcludedAttributes = (task.ExcludedAttributes == null ? string.Empty : task.ExcludedAttributes);

			InitializeComponent();

			listBox1.Items.AddRange(ExcludedItems.ToArray());

			if (!string.IsNullOrEmpty(ExcludedAttributes))
			{
				foreach (CheckBox child in groupBox1.Controls)
				{
					child.Checked = ExcludedAttributes.Contains((string)child.Tag);
				}
			}

			folderBrowserDialog1.SelectedPath = _sourceFolder;
			openFileDialog1.InitialDirectory = _sourceFolder;
		}


		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			removeButton.Enabled = (listBox1.SelectedIndices.Count > 0);
		}

		private void listBox1_KeyDown(object sender, KeyEventArgs e)
		{
			// simulate a click on the remove button when del or backspace is pressed
			if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
				removeButton.PerformClick();
		}

		private void removeButton_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndices.Count == 0)
				return;

			if (MessageBox.Show("Are you sure you want to remove the selected exclusion(s)?", "Confirmation",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			var selectedIndices = new System.Collections.ArrayList(listBox1.SelectedIndices);

			// if the items are deleted beginning with the highest index,
			// the lower indices do not get decremented by one
			selectedIndices.Sort();
			for (int i = selectedIndices.Count - 1; i >= 0; --i)
			{
				listBox1.Items.RemoveAt((int)selectedIndices[i]);
			}

			HasChanged = true;
		}

		private void addFolderButton_Click(object sender, EventArgs e)
		{
			string subfolder;

			while (true)
			{
				if (folderBrowserDialog1.ShowDialog(this) != DialogResult.OK)
					return;

				subfolder = folderBrowserDialog1.SelectedPath;
				if (!IsInSourceFolder(subfolder))
				{
					MessageBox.Show("The selected folder is not contained in the source folder.",
						"Invalid subfolder", MessageBoxButtons.OK, MessageBoxIcon.Error);

					folderBrowserDialog1.SelectedPath = _sourceFolder;
				}
				else
					break;
			}

			string relativeSubfolder = subfolder.Substring(_sourceFolder.Length);

			if (!listBox1.Items.Contains(relativeSubfolder))
			{
				listBox1.Items.Add(relativeSubfolder);
				HasChanged = true;
			}
		}

		private void addFilesButton_Click(object sender, EventArgs e)
		{
			List<string> relativeFiles;

			bool allInSourceFolder;

			do
			{
				if (openFileDialog1.ShowDialog(this) != DialogResult.OK)
					return;

				relativeFiles = new List<string>(openFileDialog1.FileNames.Length);
				allInSourceFolder = true;

				foreach (string file in openFileDialog1.FileNames)
				{
					if (!IsInSourceFolder(file))
					{
						MessageBox.Show("The selected file is not contained in the source folder.",
							"Invalid file", MessageBoxButtons.OK, MessageBoxIcon.Error);

						openFileDialog1.InitialDirectory = _sourceFolder;
						openFileDialog1.FileName = string.Empty;

						allInSourceFolder = false;
						break;
					}
					else
					{
						string relativeFile = file.Substring(_sourceFolder.Length);

						if (!listBox1.Items.Contains(relativeFile))
							relativeFiles.Add(relativeFile);
					}
				}
			}
			while (!allInSourceFolder);

			if (relativeFiles.Count > 0)
			{
				listBox1.Items.AddRange(relativeFiles.ToArray());
				HasChanged = true;
			}

			openFileDialog1.InitialDirectory = Path.GetDirectoryName(openFileDialog1.FileName);
			openFileDialog1.FileName = string.Empty;
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			addWildcardButton.Enabled = !string.IsNullOrEmpty(textBox1.Text);
		}

		private void addWildcardButton_Click(object sender, EventArgs e)
		{
			if (textBox1.Text.Length == 0)
				return;

			if (textBox1.Text.Contains(Path.DirectorySeparatorChar.ToString()))
			{
				MessageBox.Show("Wildcards must not contain any path information.",
					"Invalid wildcard", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else if (!listBox1.Items.Contains(textBox1.Text))
			{
				listBox1.Items.Add(textBox1.Text);
				HasChanged = true;
			}

			textBox1.Text = string.Empty;
			textBox1.Focus();
		}

		private void checkBox_CheckedChanged(object sender, EventArgs e)
		{
			HasChanged = true;
		}


		private bool IsInSourceFolder(string path)
		{
			if (path.Length < _sourceFolder.Length + 2)
				return false;

			// use a case-insensitive comparison under Windows
			var comparison = (Path.DirectorySeparatorChar == '\\' ?
				StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

			return (string.Compare(path, 0, _sourceFolder, 0, _sourceFolder.Length, comparison) == 0 &&
				path[_sourceFolder.Length] == Path.DirectorySeparatorChar);
		}


		protected override bool ApplyChanges()
		{
			ExcludedItems.Clear();
			foreach (string item in listBox1.Items)
			{
				ExcludedItems.Add(item);
			}

			foreach (CheckBox child in groupBox1.Controls)
			{
				string tag = (string)child.Tag;

				if (child.Checked)
				{
					if (!ExcludedAttributes.Contains(tag))
						ExcludedAttributes += tag;
				}
				else
					ExcludedAttributes = ExcludedAttributes.Replace(tag, string.Empty);
			}

			return true;
		}
	}
}

/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace RoboMirror
{
	public partial class ExcludedItemsControl : UserControl
	{
		public ExcludedItemsMode Mode { get; set; }
		public string BaseFolder { get; set; }
		public System.Collections.IList ExcludedItems { get { return listBox.Items; } }

		public event EventHandler Changed;

		public ExcludedItemsControl()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (DesignMode)
				return;

			if (string.IsNullOrEmpty(BaseFolder))
				throw new InvalidOperationException("The BaseFolder property has not been set yet.");

			var fileDropTargetWrapper = new FileDropTargetWrapper(listBox, Mode == ExcludedItemsMode.Folders ? FileDropMode.Folders : FileDropMode.Files);
			fileDropTargetWrapper.FilesDropped += (s, ea) =>
			{
				if (!TryAddItems(ea.Paths))
				{
					MessageBox.Show("A dropped item is not contained in the source folder.",
						"Invalid item", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			};

			toolTip.SetToolTip(browseButton, Mode == ExcludedItemsMode.Folders
				? "Add a specific subfolder to be excluded with all of its contents."
				: "Add specific files to be excluded.");

			toolTip.SetToolTip(wildcardTextBox, Mode == ExcludedItemsMode.Folders
				? "Enter a wildcard, e.g. \"temp*\" or \"obj\"."
				: "Enter a wildcard, e.g. \"*.tmp\" or \"thumbs.db\".");

			toolTip.SetToolTip(addWildcardButton, Mode == ExcludedItemsMode.Folders
				? "Add the wildcard above to the list.\nAll subfolders with a matching name will be excluded with all of their contents."
				: "Add the wildcard above to the list.\nAll files with a matching name will be excluded from each folder.");

			folderBrowserDialog.SelectedPath = BaseFolder;
			openFileDialog.InitialDirectory = BaseFolder;
		}

		protected virtual void OnChanged(EventArgs e)
		{
			if (Changed != null)
				Changed(this, e);
		}


		private void listBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			removeButton.Enabled = (listBox.SelectedIndices.Count > 0);
		}

		private void listBox_KeyDown(object sender, KeyEventArgs e)
		{
			// simulate a click on the remove button when del or backspace is pressed
			if ((e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back) && removeButton.Enabled)
				removeButton.PerformClick();
		}


		private void browseButton_Click(object sender, EventArgs e)
		{
			if (Mode == ExcludedItemsMode.Folders)
				AddFolder();
			else
				AddFiles();
		}

		private void AddFolder()
		{
			while (true)
			{
				if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
					return;

				if (TryAddItems(new[] { folderBrowserDialog.SelectedPath }))
					break;

				MessageBox.Show("The selected folder is not contained in the source folder.",
					"Invalid subfolder", MessageBoxButtons.OK, MessageBoxIcon.Error);

				folderBrowserDialog.SelectedPath = BaseFolder;
			}

			folderBrowserDialog.SelectedPath = BaseFolder;
		}

		private void AddFiles()
		{
			while (true)
			{
				if (openFileDialog.ShowDialog(this) != DialogResult.OK)
					return;

				if (TryAddItems(openFileDialog.FileNames))
					break;

				MessageBox.Show("A selected file is not contained in the source folder.",
					"Invalid file", MessageBoxButtons.OK, MessageBoxIcon.Error);

				openFileDialog.InitialDirectory = BaseFolder;
				openFileDialog.FileName = string.Empty;
			}

			openFileDialog.InitialDirectory = Path.GetDirectoryName(openFileDialog.FileName);
			openFileDialog.FileName = string.Empty;
		}

		bool TryAddItems(IEnumerable<string> paths)
		{
			var relativePaths = new List<string>();

			foreach (string rawPath in paths)
			{
				string path = CorrectPath(rawPath);
				if (!IsInBaseFolder(path))
					return false;

				string relativePath = path.Substring(BaseFolder.Length);
				if (!listBox.Items.Contains(relativePath))
					relativePaths.Add(relativePath);
			}

			if (relativePaths.Count > 0)
			{
				listBox.Items.AddRange(relativePaths.ToArray());
				OnChanged(EventArgs.Empty);
			}

			return true;
		}


		private void removeButton_Click(object sender, EventArgs e)
		{
			if (listBox.SelectedIndices.Count == 0)
				return;

			if (MessageBox.Show("Are you sure you want to remove the selected exclusion(s)?", "Confirmation",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			var selectedIndices = new System.Collections.ArrayList(listBox.SelectedIndices);

			// if the items are deleted beginning with the highest index,
			// the lower indices remain valid
			selectedIndices.Sort();
			for (int i = selectedIndices.Count - 1; i >= 0; --i)
				listBox.Items.RemoveAt((int)selectedIndices[i]);

			OnChanged(EventArgs.Empty);
		}


		private void wildcardTextBox_TextChanged(object sender, EventArgs e)
		{
			addWildcardButton.Enabled = !string.IsNullOrEmpty(wildcardTextBox.Text);
		}

		private void addWildcardButton_Click(object sender, EventArgs e)
		{
			if (wildcardTextBox.Text.Length == 0)
				return;

			string wildcard = CorrectPath(wildcardTextBox.Text);
			if (wildcard.Contains(Path.DirectorySeparatorChar.ToString()))
			{
				MessageBox.Show("Wildcards must not contain any path information.",
					"Invalid wildcard", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else if (!listBox.Items.Contains(wildcard))
			{
				listBox.Items.Add(wildcard);
				OnChanged(EventArgs.Empty);
			}

			wildcardTextBox.Text = string.Empty;
			wildcardTextBox.Focus();
		}


		private static string CorrectPath(string path)
		{
			return path.Replace('/', Path.DirectorySeparatorChar) // replace all forward slashes
				.Replace('\\', Path.DirectorySeparatorChar)       // replace all backslashes
				.Trim()                                           // eliminate all leading and trailing white-spaces
				.Trim('"');                                       // eliminate all leading and trailing double-quotes
		}

		private bool IsInBaseFolder(string path)
		{
			if (path.Length < BaseFolder.Length + 2)
				return false;

			// use a case-insensitive comparison under Windows
			var comparison = (Path.DirectorySeparatorChar == '\\' ?
				StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

			return (string.Compare(path, 0, BaseFolder, 0, BaseFolder.Length, comparison) == 0 &&
				path[BaseFolder.Length] == Path.DirectorySeparatorChar);
		}
	}

	public enum ExcludedItemsMode
	{
		Folders,
		Files
	}
}

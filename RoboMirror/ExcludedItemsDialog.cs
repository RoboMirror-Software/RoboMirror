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
		/// Gets the list of excluded files.
		/// </summary>
		public List<string> ExcludedFiles { get; private set; }

		/// <summary>
		/// Gets the list of excluded folders.
		/// </summary>
		public List<string> ExcludedFolders { get; private set; }

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

			ExcludedFiles = new List<string>(task.ExcludedFiles);
			ExcludedFolders = new List<string>(task.ExcludedFolders);
			ExcludedAttributes = (task.ExcludedAttributes == null ? string.Empty : task.ExcludedAttributes);

			InitializeComponent();

			excludedFilesControl.BaseFolder = sourceFolder;
			excludedFoldersControl.BaseFolder = sourceFolder;

			foreach (string file in ExcludedFiles)
				excludedFilesControl.ExcludedItems.Add(file);
			foreach (string folder in ExcludedFolders)
				excludedFoldersControl.ExcludedItems.Add(folder);

			if (!string.IsNullOrEmpty(ExcludedAttributes))
			{
				foreach (CheckBox child in tableLayoutPanel1.Controls)
					child.Checked = ExcludedAttributes.Contains((string)child.Tag);
			}
		}


		private void Control_Changed(object sender, EventArgs e)
		{
			HasChanged = true;
		}


		protected override bool ApplyChanges()
		{
			ExcludedFiles.Clear();
			foreach (string item in excludedFilesControl.ExcludedItems)
				ExcludedFiles.Add(item);

			ExcludedFolders.Clear();
			foreach (string item in excludedFoldersControl.ExcludedItems)
				ExcludedFolders.Add(item);

			foreach (CheckBox child in tableLayoutPanel1.Controls)
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

namespace RoboMirror
{
	partial class TaskDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskDialog));
			this.sourceFolderTextBox = new System.Windows.Forms.TextBox();
			this.sourceFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.targetFolderTextBox = new System.Windows.Forms.TextBox();
			this.targetFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.sourceLabel = new System.Windows.Forms.Label();
			this.targetLabel = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.excludedItemsButton = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.browseSourceFolderButton = new System.Windows.Forms.Button();
			this.browseTargetFolderButton = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.vscCheckBox = new System.Windows.Forms.CheckBox();
			this.aclsOnlyRadioButton = new System.Windows.Forms.RadioButton();
			this.allRadioButton = new System.Windows.Forms.RadioButton();
			this.noneRadioButton = new System.Windows.Forms.RadioButton();
			this.deleteExtraItemsCheckBox = new System.Windows.Forms.CheckBox();
			this.overwriteNewerFilesCheckBox = new System.Windows.Forms.CheckBox();
			this.robocopySwitchesCheckBox = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.robocopySwitchesTextBox = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// sourceFolderTextBox
			// 
			this.sourceFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.sourceFolderTextBox.Location = new System.Drawing.Point(52, 43);
			this.sourceFolderTextBox.MaxLength = 1000;
			this.sourceFolderTextBox.Name = "sourceFolderTextBox";
			this.sourceFolderTextBox.Size = new System.Drawing.Size(339, 23);
			this.sourceFolderTextBox.TabIndex = 1;
			this.sourceFolderTextBox.TextChanged += new System.EventHandler(this.sourceFolderTextBox_TextChanged);
			// 
			// sourceFolderBrowserDialog
			// 
			this.sourceFolderBrowserDialog.Description = "Please select the source folder.";
			this.sourceFolderBrowserDialog.ShowNewFolderButton = false;
			// 
			// targetFolderTextBox
			// 
			this.targetFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.targetFolderTextBox.Location = new System.Drawing.Point(52, 150);
			this.targetFolderTextBox.MaxLength = 1000;
			this.targetFolderTextBox.Name = "targetFolderTextBox";
			this.targetFolderTextBox.Size = new System.Drawing.Size(339, 23);
			this.targetFolderTextBox.TabIndex = 6;
			this.targetFolderTextBox.TextChanged += new System.EventHandler(this.Control_Changed);
			// 
			// targetFolderBrowserDialog
			// 
			this.targetFolderBrowserDialog.Description = "Please select the target folder.";
			// 
			// sourceLabel
			// 
			this.sourceLabel.AutoSize = true;
			this.sourceLabel.Location = new System.Drawing.Point(49, 19);
			this.sourceLabel.Name = "sourceLabel";
			this.sourceLabel.Size = new System.Drawing.Size(80, 15);
			this.sourceLabel.TabIndex = 0;
			this.sourceLabel.Text = "Source folder:";
			// 
			// targetLabel
			// 
			this.targetLabel.AutoSize = true;
			this.targetLabel.Location = new System.Drawing.Point(49, 126);
			this.targetLabel.Name = "targetLabel";
			this.targetLabel.Size = new System.Drawing.Size(78, 15);
			this.targetLabel.TabIndex = 5;
			this.targetLabel.Text = "Target folder:";
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::RoboMirror.Properties.Resources.data_into24;
			this.pictureBox2.Location = new System.Drawing.Point(14, 121);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(28, 28);
			this.pictureBox2.TabIndex = 7;
			this.pictureBox2.TabStop = false;
			// 
			// excludedItemsButton
			// 
			this.excludedItemsButton.Enabled = false;
			this.excludedItemsButton.Image = global::RoboMirror.Properties.Resources.data_forbidden;
			this.excludedItemsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.excludedItemsButton.Location = new System.Drawing.Point(52, 76);
			this.excludedItemsButton.Name = "excludedItemsButton";
			this.excludedItemsButton.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.excludedItemsButton.Size = new System.Drawing.Size(147, 27);
			this.excludedItemsButton.TabIndex = 3;
			this.excludedItemsButton.Text = "Excluded items...";
			this.excludedItemsButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.excludedItemsButton, "Edit the list of excluded subfolders, files and wildcards.\r\nFiles may be excluded" +
        " by their attributes too.");
			this.excludedItemsButton.UseVisualStyleBackColor = true;
			this.excludedItemsButton.Click += new System.EventHandler(this.excludedItemsButton_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::RoboMirror.Properties.Resources.data24;
			this.pictureBox1.Location = new System.Drawing.Point(14, 14);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(28, 28);
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			// 
			// browseSourceFolderButton
			// 
			this.browseSourceFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browseSourceFolderButton.Image = global::RoboMirror.Properties.Resources.folder_view;
			this.browseSourceFolderButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.browseSourceFolderButton.Location = new System.Drawing.Point(399, 41);
			this.browseSourceFolderButton.Name = "browseSourceFolderButton";
			this.browseSourceFolderButton.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.browseSourceFolderButton.Size = new System.Drawing.Size(105, 27);
			this.browseSourceFolderButton.TabIndex = 2;
			this.browseSourceFolderButton.Text = "Browse...";
			this.browseSourceFolderButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.browseSourceFolderButton.UseVisualStyleBackColor = true;
			this.browseSourceFolderButton.Click += new System.EventHandler(this.browseSourceFolderButton_Click);
			// 
			// browseTargetFolderButton
			// 
			this.browseTargetFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browseTargetFolderButton.Image = global::RoboMirror.Properties.Resources.folder_view;
			this.browseTargetFolderButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.browseTargetFolderButton.Location = new System.Drawing.Point(399, 148);
			this.browseTargetFolderButton.Name = "browseTargetFolderButton";
			this.browseTargetFolderButton.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.browseTargetFolderButton.Size = new System.Drawing.Size(105, 27);
			this.browseTargetFolderButton.TabIndex = 7;
			this.browseTargetFolderButton.Text = "Browse...";
			this.browseTargetFolderButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.browseTargetFolderButton.UseVisualStyleBackColor = true;
			this.browseTargetFolderButton.Click += new System.EventHandler(this.browseTargetFolderButton_Click);
			// 
			// vscCheckBox
			// 
			this.vscCheckBox.AutoSize = true;
			this.vscCheckBox.Location = new System.Drawing.Point(331, 81);
			this.vscCheckBox.Name = "vscCheckBox";
			this.vscCheckBox.Size = new System.Drawing.Size(170, 19);
			this.vscCheckBox.TabIndex = 4;
			this.vscCheckBox.Text = "Use a volume shadow copy";
			this.toolTip1.SetToolTip(this.vscCheckBox, "Create a shadow copy of the source volume during mirroring.\r\nThis allows to copy " +
        "files which are locked by running processes\r\nbut requires elevated privileges an" +
        "d Windows Vista or later.");
			this.vscCheckBox.UseVisualStyleBackColor = true;
			this.vscCheckBox.CheckedChanged += new System.EventHandler(this.Control_Changed);
			// 
			// aclsOnlyRadioButton
			// 
			this.aclsOnlyRadioButton.AutoSize = true;
			this.aclsOnlyRadioButton.Location = new System.Drawing.Point(85, 25);
			this.aclsOnlyRadioButton.Name = "aclsOnlyRadioButton";
			this.aclsOnlyRadioButton.Size = new System.Drawing.Size(78, 19);
			this.aclsOnlyRadioButton.TabIndex = 1;
			this.aclsOnlyRadioButton.Text = "ACLs only";
			this.toolTip1.SetToolTip(this.aclsOnlyRadioButton, "Copy only the rules allowing and denying users access to the files.");
			this.aclsOnlyRadioButton.UseVisualStyleBackColor = true;
			this.aclsOnlyRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
			// 
			// allRadioButton
			// 
			this.allRadioButton.AutoSize = true;
			this.allRadioButton.Location = new System.Drawing.Point(188, 25);
			this.allRadioButton.Name = "allRadioButton";
			this.allRadioButton.Size = new System.Drawing.Size(39, 19);
			this.allRadioButton.TabIndex = 2;
			this.allRadioButton.Text = "All";
			this.toolTip1.SetToolTip(this.allRadioButton, "Copy all extended attributes (ACLs, owner and auditing information).\r\nThis requir" +
        "es elevated privileges.");
			this.allRadioButton.UseVisualStyleBackColor = true;
			this.allRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
			// 
			// noneRadioButton
			// 
			this.noneRadioButton.AutoSize = true;
			this.noneRadioButton.Checked = true;
			this.noneRadioButton.Location = new System.Drawing.Point(10, 25);
			this.noneRadioButton.Name = "noneRadioButton";
			this.noneRadioButton.Size = new System.Drawing.Size(54, 19);
			this.noneRadioButton.TabIndex = 0;
			this.noneRadioButton.TabStop = true;
			this.noneRadioButton.Text = "None";
			this.toolTip1.SetToolTip(this.noneRadioButton, "Do not copy any extended attributes.\r\nIf you do not know what these attributes ar" +
        "e, you should probably\r\nuse this option.");
			this.noneRadioButton.UseVisualStyleBackColor = true;
			this.noneRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
			// 
			// deleteExtraItemsCheckBox
			// 
			this.deleteExtraItemsCheckBox.AutoSize = true;
			this.deleteExtraItemsCheckBox.Location = new System.Drawing.Point(331, 217);
			this.deleteExtraItemsCheckBox.Name = "deleteExtraItemsCheckBox";
			this.deleteExtraItemsCheckBox.Size = new System.Drawing.Size(173, 19);
			this.deleteExtraItemsCheckBox.TabIndex = 10;
			this.deleteExtraItemsCheckBox.Text = "Delete extra files and folders";
			this.toolTip1.SetToolTip(this.deleteExtraItemsCheckBox, "Delete files and folders in the target folder which do not exist\r\nin the source f" +
        "older.\r\nUse this option if you want an exact copy of the source folder.");
			this.deleteExtraItemsCheckBox.UseVisualStyleBackColor = true;
			this.deleteExtraItemsCheckBox.CheckedChanged += new System.EventHandler(this.Control_Changed);
			// 
			// overwriteNewerFilesCheckBox
			// 
			this.overwriteNewerFilesCheckBox.AutoSize = true;
			this.overwriteNewerFilesCheckBox.Location = new System.Drawing.Point(331, 191);
			this.overwriteNewerFilesCheckBox.Name = "overwriteNewerFilesCheckBox";
			this.overwriteNewerFilesCheckBox.Size = new System.Drawing.Size(136, 19);
			this.overwriteNewerFilesCheckBox.TabIndex = 9;
			this.overwriteNewerFilesCheckBox.Text = "Overwrite newer files";
			this.toolTip1.SetToolTip(this.overwriteNewerFilesCheckBox, "Overwrite newer files in the target folder by older ones from the source folder.\r" +
        "\nUse this option if you want an exact copy of the source folder.");
			this.overwriteNewerFilesCheckBox.UseVisualStyleBackColor = true;
			this.overwriteNewerFilesCheckBox.CheckedChanged += new System.EventHandler(this.Control_Changed);
			// 
			// robocopySwitchesCheckBox
			// 
			this.robocopySwitchesCheckBox.AutoSize = true;
			this.robocopySwitchesCheckBox.Location = new System.Drawing.Point(14, 261);
			this.robocopySwitchesCheckBox.Name = "robocopySwitchesCheckBox";
			this.robocopySwitchesCheckBox.Size = new System.Drawing.Size(176, 19);
			this.robocopySwitchesCheckBox.TabIndex = 11;
			this.robocopySwitchesCheckBox.Text = "Custom Robocopy switches:";
			this.toolTip1.SetToolTip(this.robocopySwitchesCheckBox, "Customize the basic Robocopy command-line switches for this mirror task.\r\nUse thi" +
        "s option with care and only if you know what you are doing!");
			this.robocopySwitchesCheckBox.UseVisualStyleBackColor = true;
			this.robocopySwitchesCheckBox.CheckedChanged += new System.EventHandler(this.robocopySwitchesCheckBox_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.noneRadioButton);
			this.groupBox1.Controls.Add(this.allRadioButton);
			this.groupBox1.Controls.Add(this.aclsOnlyRadioButton);
			this.groupBox1.Location = new System.Drawing.Point(52, 185);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(233, 58);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Copy extended NTFS attributes";
			// 
			// robocopySwitchesTextBox
			// 
			this.robocopySwitchesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.robocopySwitchesTextBox.Enabled = false;
			this.robocopySwitchesTextBox.Location = new System.Drawing.Point(196, 259);
			this.robocopySwitchesTextBox.MaxLength = 1000;
			this.robocopySwitchesTextBox.Name = "robocopySwitchesTextBox";
			this.robocopySwitchesTextBox.Size = new System.Drawing.Size(308, 23);
			this.robocopySwitchesTextBox.TabIndex = 12;
			this.robocopySwitchesTextBox.TextChanged += new System.EventHandler(this.Control_Changed);
			// 
			// TaskDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(518, 349);
			this.Controls.Add(this.robocopySwitchesCheckBox);
			this.Controls.Add(this.robocopySwitchesTextBox);
			this.Controls.Add(this.overwriteNewerFilesCheckBox);
			this.Controls.Add(this.deleteExtraItemsCheckBox);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.vscCheckBox);
			this.Controls.Add(this.targetLabel);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.excludedItemsButton);
			this.Controls.Add(this.sourceLabel);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.browseSourceFolderButton);
			this.Controls.Add(this.sourceFolderTextBox);
			this.Controls.Add(this.browseTargetFolderButton);
			this.Controls.Add(this.targetFolderTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "TaskDialog";
			this.Text = "Mirror task";
			this.Controls.SetChildIndex(this.targetFolderTextBox, 0);
			this.Controls.SetChildIndex(this.browseTargetFolderButton, 0);
			this.Controls.SetChildIndex(this.sourceFolderTextBox, 0);
			this.Controls.SetChildIndex(this.browseSourceFolderButton, 0);
			this.Controls.SetChildIndex(this.pictureBox1, 0);
			this.Controls.SetChildIndex(this.sourceLabel, 0);
			this.Controls.SetChildIndex(this.excludedItemsButton, 0);
			this.Controls.SetChildIndex(this.pictureBox2, 0);
			this.Controls.SetChildIndex(this.targetLabel, 0);
			this.Controls.SetChildIndex(this.vscCheckBox, 0);
			this.Controls.SetChildIndex(this.groupBox1, 0);
			this.Controls.SetChildIndex(this.deleteExtraItemsCheckBox, 0);
			this.Controls.SetChildIndex(this.overwriteNewerFilesCheckBox, 0);
			this.Controls.SetChildIndex(this.robocopySwitchesTextBox, 0);
			this.Controls.SetChildIndex(this.robocopySwitchesCheckBox, 0);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox sourceFolderTextBox;
		private System.Windows.Forms.Button browseSourceFolderButton;
		private System.Windows.Forms.FolderBrowserDialog sourceFolderBrowserDialog;
		private System.Windows.Forms.Button browseTargetFolderButton;
		private System.Windows.Forms.TextBox targetFolderTextBox;
		private System.Windows.Forms.FolderBrowserDialog targetFolderBrowserDialog;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label sourceLabel;
		private System.Windows.Forms.Button excludedItemsButton;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Label targetLabel;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.CheckBox vscCheckBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton allRadioButton;
		private System.Windows.Forms.RadioButton aclsOnlyRadioButton;
		private System.Windows.Forms.RadioButton noneRadioButton;
		private System.Windows.Forms.CheckBox deleteExtraItemsCheckBox;
		private System.Windows.Forms.CheckBox overwriteNewerFilesCheckBox;
		private System.Windows.Forms.TextBox robocopySwitchesTextBox;
		private System.Windows.Forms.CheckBox robocopySwitchesCheckBox;
	}
}


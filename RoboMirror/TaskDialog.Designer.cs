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
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// sourceFolderTextBox
			// 
			this.sourceFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.sourceFolderTextBox.Location = new System.Drawing.Point(45, 37);
			this.sourceFolderTextBox.MaxLength = 1000;
			this.sourceFolderTextBox.Name = "sourceFolderTextBox";
			this.sourceFolderTextBox.Size = new System.Drawing.Size(291, 21);
			this.sourceFolderTextBox.TabIndex = 1;
			this.toolTip1.SetToolTip(this.sourceFolderTextBox, "All contents of this folder except the excluded items below will be mirrored.");
			this.sourceFolderTextBox.TextChanged += new System.EventHandler(this.sourceFolderTextBox_TextChanged);
			// 
			// sourceFolderBrowserDialog
			// 
			this.sourceFolderBrowserDialog.ShowNewFolderButton = false;
			// 
			// targetFolderTextBox
			// 
			this.targetFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.targetFolderTextBox.Location = new System.Drawing.Point(45, 130);
			this.targetFolderTextBox.MaxLength = 1000;
			this.targetFolderTextBox.Name = "targetFolderTextBox";
			this.targetFolderTextBox.Size = new System.Drawing.Size(291, 21);
			this.targetFolderTextBox.TabIndex = 5;
			this.toolTip1.SetToolTip(this.targetFolderTextBox, "This folder will receive the mirrored files and subfolders.\r\nAll other existing f" +
					"iles will be deleted.");
			this.targetFolderTextBox.TextChanged += new System.EventHandler(this.Control_Changed);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(42, 17);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(170, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Source folder to be mirrored:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(42, 110);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(120, 13);
			this.label7.TabIndex = 4;
			this.label7.Text = "Mirror target folder:";
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::RoboMirror.Properties.Resources.data_into24;
			this.pictureBox2.Location = new System.Drawing.Point(12, 105);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(24, 24);
			this.pictureBox2.TabIndex = 7;
			this.pictureBox2.TabStop = false;
			// 
			// excludedItemsButton
			// 
			this.excludedItemsButton.Enabled = false;
			this.excludedItemsButton.Image = global::RoboMirror.Properties.Resources.data_forbidden;
			this.excludedItemsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.excludedItemsButton.Location = new System.Drawing.Point(45, 66);
			this.excludedItemsButton.Name = "excludedItemsButton";
			this.excludedItemsButton.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.excludedItemsButton.Size = new System.Drawing.Size(126, 23);
			this.excludedItemsButton.TabIndex = 3;
			this.excludedItemsButton.Text = "&Excluded items...";
			this.excludedItemsButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.excludedItemsButton, "Edit the list of excluded subfolders, files and wildcards.\r\nFiles may be excluded" +
					" by their attributes too.");
			this.excludedItemsButton.UseVisualStyleBackColor = true;
			this.excludedItemsButton.Click += new System.EventHandler(this.excludedItemsButton_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::RoboMirror.Properties.Resources.data24;
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(24, 24);
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			// 
			// browseSourceFolderButton
			// 
			this.browseSourceFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browseSourceFolderButton.Image = global::RoboMirror.Properties.Resources.folder_view;
			this.browseSourceFolderButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.browseSourceFolderButton.Location = new System.Drawing.Point(342, 35);
			this.browseSourceFolderButton.Name = "browseSourceFolderButton";
			this.browseSourceFolderButton.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.browseSourceFolderButton.Size = new System.Drawing.Size(90, 23);
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
			this.browseTargetFolderButton.Location = new System.Drawing.Point(342, 129);
			this.browseTargetFolderButton.Name = "browseTargetFolderButton";
			this.browseTargetFolderButton.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.browseTargetFolderButton.Size = new System.Drawing.Size(90, 23);
			this.browseTargetFolderButton.TabIndex = 6;
			this.browseTargetFolderButton.Text = "Browse...";
			this.browseTargetFolderButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.browseTargetFolderButton.UseVisualStyleBackColor = true;
			this.browseTargetFolderButton.Click += new System.EventHandler(this.browseTargetFolderButton_Click);
			// 
			// vscCheckBox
			// 
			this.vscCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.vscCheckBox.AutoSize = true;
			this.vscCheckBox.Location = new System.Drawing.Point(279, 183);
			this.vscCheckBox.Name = "vscCheckBox";
			this.vscCheckBox.Size = new System.Drawing.Size(156, 17);
			this.vscCheckBox.TabIndex = 8;
			this.vscCheckBox.Text = "&Use a volume shadow copy";
			this.toolTip1.SetToolTip(this.vscCheckBox, "Create a volume shadow copy of the source volume during backup.\r\nThis allows to b" +
					"ackup files which are locked by running processes.");
			this.vscCheckBox.UseVisualStyleBackColor = true;
			this.vscCheckBox.CheckedChanged += new System.EventHandler(this.Control_Changed);
			// 
			// aclsOnlyRadioButton
			// 
			this.aclsOnlyRadioButton.AutoSize = true;
			this.aclsOnlyRadioButton.Location = new System.Drawing.Point(73, 22);
			this.aclsOnlyRadioButton.Name = "aclsOnlyRadioButton";
			this.aclsOnlyRadioButton.Size = new System.Drawing.Size(72, 17);
			this.aclsOnlyRadioButton.TabIndex = 1;
			this.aclsOnlyRadioButton.Text = "ACLs &only";
			this.toolTip1.SetToolTip(this.aclsOnlyRadioButton, "Copy only the rules allowing and denying users access to the files.");
			this.aclsOnlyRadioButton.UseVisualStyleBackColor = true;
			this.aclsOnlyRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
			// 
			// allRadioButton
			// 
			this.allRadioButton.AutoSize = true;
			this.allRadioButton.Location = new System.Drawing.Point(161, 22);
			this.allRadioButton.Name = "allRadioButton";
			this.allRadioButton.Size = new System.Drawing.Size(36, 17);
			this.allRadioButton.TabIndex = 2;
			this.allRadioButton.Text = "&All";
			this.toolTip1.SetToolTip(this.allRadioButton, "Copy all extended attributes (ACLs, owner and auditing information).");
			this.allRadioButton.UseVisualStyleBackColor = true;
			this.allRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
			// 
			// noneRadioButton
			// 
			this.noneRadioButton.AutoSize = true;
			this.noneRadioButton.Checked = true;
			this.noneRadioButton.Location = new System.Drawing.Point(9, 22);
			this.noneRadioButton.Name = "noneRadioButton";
			this.noneRadioButton.Size = new System.Drawing.Size(50, 17);
			this.noneRadioButton.TabIndex = 0;
			this.noneRadioButton.TabStop = true;
			this.noneRadioButton.Text = "&None";
			this.toolTip1.SetToolTip(this.noneRadioButton, "Do not copy any extended attributes.\r\nIf you do not know what these attributes ar" +
					"e, you should probably\r\nuse this option.");
			this.noneRadioButton.UseVisualStyleBackColor = true;
			this.noneRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.noneRadioButton);
			this.groupBox1.Controls.Add(this.allRadioButton);
			this.groupBox1.Controls.Add(this.aclsOnlyRadioButton);
			this.groupBox1.Location = new System.Drawing.Point(45, 160);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 50);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Copy extended NTFS attributes";
			// 
			// TaskDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(444, 269);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.vscCheckBox);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.excludedItemsButton);
			this.Controls.Add(this.label6);
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
			this.Controls.SetChildIndex(this.label6, 0);
			this.Controls.SetChildIndex(this.excludedItemsButton, 0);
			this.Controls.SetChildIndex(this.pictureBox2, 0);
			this.Controls.SetChildIndex(this.label7, 0);
			this.Controls.SetChildIndex(this.vscCheckBox, 0);
			this.Controls.SetChildIndex(this.groupBox1, 0);
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
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button excludedItemsButton;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.CheckBox vscCheckBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton allRadioButton;
		private System.Windows.Forms.RadioButton aclsOnlyRadioButton;
		private System.Windows.Forms.RadioButton noneRadioButton;
	}
}


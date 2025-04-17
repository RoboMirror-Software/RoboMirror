namespace RoboMirror
{
	partial class ExcludedItemsDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExcludedItemsDialog));
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.removeButton = new System.Windows.Forms.Button();
			this.addFilesButton = new System.Windows.Forms.Button();
			this.addFolderButton = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.addWildcardButton = new System.Windows.Forms.Button();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.IntegralHeight = false;
			this.listBox1.Location = new System.Drawing.Point(12, 62);
			this.listBox1.Name = "listBox1";
			this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBox1.Size = new System.Drawing.Size(374, 149);
			this.listBox1.Sorted = true;
			this.listBox1.TabIndex = 1;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
			// 
			// folderBrowserDialog1
			// 
			this.folderBrowserDialog1.ShowNewFolderButton = false;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Multiselect = true;
			// 
			// removeButton
			// 
			this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.removeButton.Enabled = false;
			this.removeButton.Image = global::RoboMirror.Properties.Resources.delete2;
			this.removeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.removeButton.Location = new System.Drawing.Point(392, 126);
			this.removeButton.Name = "removeButton";
			this.removeButton.Padding = new System.Windows.Forms.Padding(4, 0, 14, 0);
			this.removeButton.Size = new System.Drawing.Size(100, 23);
			this.removeButton.TabIndex = 4;
			this.removeButton.Text = "&Remove";
			this.removeButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.removeButton, "Remove the selected item(s) from the list.");
			this.removeButton.UseVisualStyleBackColor = true;
			this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
			// 
			// addFilesButton
			// 
			this.addFilesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addFilesButton.Image = global::RoboMirror.Properties.Resources.document_add;
			this.addFilesButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.addFilesButton.Location = new System.Drawing.Point(392, 91);
			this.addFilesButton.Name = "addFilesButton";
			this.addFilesButton.Padding = new System.Windows.Forms.Padding(4, 0, 5, 0);
			this.addFilesButton.Size = new System.Drawing.Size(100, 23);
			this.addFilesButton.TabIndex = 3;
			this.addFilesButton.Text = "Add f&iles...";
			this.addFilesButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.addFilesButton, "Add files to be excluded.");
			this.addFilesButton.UseVisualStyleBackColor = true;
			this.addFilesButton.Click += new System.EventHandler(this.addFilesButton_Click);
			// 
			// addFolderButton
			// 
			this.addFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addFolderButton.Image = global::RoboMirror.Properties.Resources.folder_add;
			this.addFolderButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.addFolderButton.Location = new System.Drawing.Point(392, 62);
			this.addFolderButton.Name = "addFolderButton";
			this.addFolderButton.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.addFolderButton.Size = new System.Drawing.Size(100, 23);
			this.addFolderButton.TabIndex = 2;
			this.addFolderButton.Text = "Add &folder...";
			this.addFolderButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.addFolderButton, "Add a subfolder to be excluded with all its contents.");
			this.addFolderButton.UseVisualStyleBackColor = true;
			this.addFolderButton.Click += new System.EventHandler(this.addFolderButton_Click);
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(392, 161);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 21);
			this.textBox1.TabIndex = 5;
			this.toolTip1.SetToolTip(this.textBox1, "Enter a wildcard, e.g. \"*.bak\".");
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// addWildcardButton
			// 
			this.addWildcardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.addWildcardButton.Enabled = false;
			this.addWildcardButton.Image = global::RoboMirror.Properties.Resources.document_find;
			this.addWildcardButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.addWildcardButton.Location = new System.Drawing.Point(392, 188);
			this.addWildcardButton.Name = "addWildcardButton";
			this.addWildcardButton.Padding = new System.Windows.Forms.Padding(4, 0, 2, 0);
			this.addWildcardButton.Size = new System.Drawing.Size(100, 23);
			this.addWildcardButton.TabIndex = 6;
			this.addWildcardButton.Text = "Add &wildcard";
			this.addWildcardButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.addWildcardButton, "Add the wildcard above to the exclusions list.\r\nAll matching files will be exclud" +
					"ed.");
			this.addWildcardButton.UseVisualStyleBackColor = true;
			this.addWildcardButton.Click += new System.EventHandler(this.addWildcardButton_Click);
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(12, 23);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(59, 17);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Tag = "H";
			this.checkBox1.Text = "&Hidden";
			this.toolTip1.SetToolTip(this.checkBox1, "Exclude hidden files.");
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point(150, 23);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(61, 17);
			this.checkBox2.TabIndex = 1;
			this.checkBox2.Tag = "S";
			this.checkBox2.Text = "&System";
			this.toolTip1.SetToolTip(this.checkBox2, "Exclude system files.");
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// checkBox3
			// 
			this.checkBox3.AutoSize = true;
			this.checkBox3.Location = new System.Drawing.Point(293, 23);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(75, 17);
			this.checkBox3.TabIndex = 2;
			this.checkBox3.Tag = "E";
			this.checkBox3.Text = "&Encrypted";
			this.toolTip1.SetToolTip(this.checkBox3, "Exclude encrypted files.");
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.checkBox3);
			this.groupBox1.Controls.Add(this.checkBox2);
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Location = new System.Drawing.Point(12, 227);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(374, 50);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Exclude files having any of these attributes set:";
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(154)))), ((int)(((byte)(181)))));
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(12, 0, 56, 0);
			this.label1.Size = new System.Drawing.Size(504, 50);
			this.label1.TabIndex = 0;
			this.label1.Text = "These files and folders are treated as if they would not exist. They are not copi" +
				"ed to the destination folder, and any matching existing items in the destination" +
				" folder are deleted.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pictureBox3
			// 
			this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(154)))), ((int)(((byte)(181)))));
			this.pictureBox3.Image = global::RoboMirror.Properties.Resources.data_forbidden32;
			this.pictureBox3.Location = new System.Drawing.Point(460, 9);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(32, 32);
			this.pictureBox3.TabIndex = 1002;
			this.pictureBox3.TabStop = false;
			// 
			// ExcludedItemsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(504, 337);
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.addWildcardButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.removeButton);
			this.Controls.Add(this.addFilesButton);
			this.Controls.Add(this.addFolderButton);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(520, 373);
			this.Name = "ExcludedItemsDialog";
			this.Text = "Excluded items";
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.listBox1, 0);
			this.Controls.SetChildIndex(this.addFolderButton, 0);
			this.Controls.SetChildIndex(this.addFilesButton, 0);
			this.Controls.SetChildIndex(this.removeButton, 0);
			this.Controls.SetChildIndex(this.textBox1, 0);
			this.Controls.SetChildIndex(this.groupBox1, 0);
			this.Controls.SetChildIndex(this.addWildcardButton, 0);
			this.Controls.SetChildIndex(this.pictureBox3, 0);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button addFolderButton;
		private System.Windows.Forms.Button addFilesButton;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button addWildcardButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox3;
	}
}
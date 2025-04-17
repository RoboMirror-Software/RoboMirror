namespace RoboMirror.GUI
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
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.filesGroupBox = new System.Windows.Forms.GroupBox();
			this.excludedFilesControl = new RoboMirror.GUI.ExcludedItemsControl();
			this.foldersGroupBox = new System.Windows.Forms.GroupBox();
			this.excludedFoldersControl = new RoboMirror.GUI.ExcludedItemsControl();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			this.filesGroupBox.SuspendLayout();
			this.foldersGroupBox.SuspendLayout();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(0, 0);
			this.checkBox1.Margin = new System.Windows.Forms.Padding(0);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(65, 19);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Tag = "H";
			this.checkBox1.Text = "Hidden";
			this.toolTip1.SetToolTip(this.checkBox1, "Exclude hidden files.");
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.Control_Changed);
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point(162, 0);
			this.checkBox2.Margin = new System.Windows.Forms.Padding(0);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(64, 19);
			this.checkBox2.TabIndex = 1;
			this.checkBox2.Tag = "S";
			this.checkBox2.Text = "System";
			this.toolTip1.SetToolTip(this.checkBox2, "Exclude system files.");
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.CheckedChanged += new System.EventHandler(this.Control_Changed);
			// 
			// checkBox3
			// 
			this.checkBox3.AutoSize = true;
			this.checkBox3.Location = new System.Drawing.Point(323, 0);
			this.checkBox3.Margin = new System.Windows.Forms.Padding(0);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(79, 19);
			this.checkBox3.TabIndex = 2;
			this.checkBox3.Tag = "E";
			this.checkBox3.Text = "Encrypted";
			this.toolTip1.SetToolTip(this.checkBox3, "Exclude encrypted files.");
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox3.CheckedChanged += new System.EventHandler(this.Control_Changed);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.tableLayoutPanel1);
			this.groupBox1.Location = new System.Drawing.Point(14, 157);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(414, 47);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Exclude files having any of these attributes set:";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 5;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.checkBox1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.checkBox2, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.checkBox3, 4, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 22);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(402, 19);
			this.tableLayoutPanel1.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(14, 0, 65, 0);
			this.label1.Size = new System.Drawing.Size(597, 58);
			this.label1.TabIndex = 0;
			this.label1.Text = "These files and folders will not be copied.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pictureBox3
			// 
			this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.pictureBox3.Image = global::RoboMirror.Properties.Resources.data_forbidden32;
			this.pictureBox3.Location = new System.Drawing.Point(550, 13);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(32, 32);
			this.pictureBox3.TabIndex = 1002;
			this.pictureBox3.TabStop = false;
			// 
			// filesGroupBox
			// 
			this.filesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.filesGroupBox.Controls.Add(this.groupBox1);
			this.filesGroupBox.Controls.Add(this.excludedFilesControl);
			this.filesGroupBox.Location = new System.Drawing.Point(0, 0);
			this.filesGroupBox.Margin = new System.Windows.Forms.Padding(0);
			this.filesGroupBox.Name = "filesGroupBox";
			this.filesGroupBox.Size = new System.Drawing.Size(568, 217);
			this.filesGroupBox.TabIndex = 0;
			this.filesGroupBox.TabStop = false;
			this.filesGroupBox.Text = "Excluded files";
			// 
			// excludedFilesControl
			// 
			this.excludedFilesControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.excludedFilesControl.BaseFolder = null;
			this.excludedFilesControl.Font = this.Font;
			this.excludedFilesControl.Location = new System.Drawing.Point(14, 22);
			this.excludedFilesControl.Mode = RoboMirror.GUI.ExcludedItemsMode.Files;
			this.excludedFilesControl.Name = "excludedFilesControl";
			this.excludedFilesControl.Size = new System.Drawing.Size(540, 124);
			this.excludedFilesControl.TabIndex = 0;
			this.excludedFilesControl.Changed += new System.EventHandler(this.Control_Changed);
			// 
			// foldersGroupBox
			// 
			this.foldersGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.foldersGroupBox.Controls.Add(this.excludedFoldersControl);
			this.foldersGroupBox.Location = new System.Drawing.Point(0, 7);
			this.foldersGroupBox.Margin = new System.Windows.Forms.Padding(0);
			this.foldersGroupBox.Name = "foldersGroupBox";
			this.foldersGroupBox.Size = new System.Drawing.Size(568, 160);
			this.foldersGroupBox.TabIndex = 0;
			this.foldersGroupBox.TabStop = false;
			this.foldersGroupBox.Text = "Excluded folders";
			// 
			// excludedFoldersControl
			// 
			this.excludedFoldersControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.excludedFoldersControl.BaseFolder = null;
			this.excludedFoldersControl.Location = new System.Drawing.Point(14, 22);
			this.excludedFoldersControl.Mode = RoboMirror.GUI.ExcludedItemsMode.Folders;
			this.excludedFoldersControl.Name = "excludedFoldersControl";
			this.excludedFoldersControl.Size = new System.Drawing.Size(540, 125);
			this.excludedFoldersControl.TabIndex = 0;
			this.excludedFoldersControl.Changed += new System.EventHandler(this.Control_Changed);
			// 
			// splitContainer
			// 
			this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitContainer.Location = new System.Drawing.Point(14, 72);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer.Panel1.Controls.Add(this.filesGroupBox);
			this.splitContainer.Panel1MinSize = 100;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer.Panel2.Controls.Add(this.foldersGroupBox);
			this.splitContainer.Panel2MinSize = 100;
			this.splitContainer.Size = new System.Drawing.Size(568, 397);
			this.splitContainer.SplitterDistance = 227;
			this.splitContainer.SplitterWidth = 3;
			this.splitContainer.TabIndex = 1;
			// 
			// ExcludedItemsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(597, 533);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(605, 564);
			this.Name = "ExcludedItemsDialog";
			this.Text = "Excluded items";
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.pictureBox3, 0);
			this.Controls.SetChildIndex(this.splitContainer, 0);
			this.groupBox1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			this.filesGroupBox.ResumeLayout(false);
			this.foldersGroupBox.ResumeLayout(false);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.GroupBox filesGroupBox;
		private System.Windows.Forms.GroupBox foldersGroupBox;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private ExcludedItemsControl excludedFilesControl;
		private ExcludedItemsControl excludedFoldersControl;
	}
}
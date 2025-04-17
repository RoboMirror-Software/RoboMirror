namespace RoboMirror.GUI
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.mainPanel = new System.Windows.Forms.Panel();
			this.historyButton = new System.Windows.Forms.Button();
			this.scheduleButton = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.restoreButton = new System.Windows.Forms.Button();
			this.addButton = new System.Windows.Forms.Button();
			this.backupButton = new System.Windows.Forms.Button();
			this.editButton = new System.Windows.Forms.Button();
			this.removeButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.mainPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "data_copy.png");
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.pictureBox1.Image = global::RoboMirror.Properties.Resources.about32;
			this.pictureBox1.Location = new System.Drawing.Point(595, 10);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(37, 37);
			this.pictureBox1.TabIndex = 1003;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(14, 0, 65, 0);
			this.label1.Size = new System.Drawing.Size(646, 58);
			this.label1.TabIndex = 0;
			this.label1.Text = "Manage your mirror tasks and perform backup and restore operations.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mainPanel
			// 
			this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mainPanel.Controls.Add(this.historyButton);
			this.mainPanel.Controls.Add(this.scheduleButton);
			this.mainPanel.Controls.Add(this.listView1);
			this.mainPanel.Controls.Add(this.restoreButton);
			this.mainPanel.Controls.Add(this.addButton);
			this.mainPanel.Controls.Add(this.backupButton);
			this.mainPanel.Controls.Add(this.editButton);
			this.mainPanel.Controls.Add(this.removeButton);
			this.mainPanel.Location = new System.Drawing.Point(14, 72);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(618, 249);
			this.mainPanel.TabIndex = 0;
			// 
			// historyButton
			// 
			this.historyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.historyButton.Enabled = false;
			this.historyButton.Image = global::RoboMirror.Properties.Resources.history;
			this.historyButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.historyButton.Location = new System.Drawing.Point(501, 112);
			this.historyButton.Name = "historyButton";
			this.historyButton.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
			this.historyButton.Size = new System.Drawing.Size(117, 27);
			this.historyButton.TabIndex = 4;
			this.historyButton.Text = "History...";
			this.historyButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.historyButton, "Display the history of the selected mirror task.");
			this.historyButton.UseVisualStyleBackColor = true;
			this.historyButton.Click += new System.EventHandler(this.historyButton_Click);
			// 
			// scheduleButton
			// 
			this.scheduleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.scheduleButton.Enabled = false;
			this.scheduleButton.Image = global::RoboMirror.Properties.Resources.clock;
			this.scheduleButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.scheduleButton.Location = new System.Drawing.Point(501, 157);
			this.scheduleButton.Name = "scheduleButton";
			this.scheduleButton.Padding = new System.Windows.Forms.Padding(10, 0, 12, 0);
			this.scheduleButton.Size = new System.Drawing.Size(117, 27);
			this.scheduleButton.TabIndex = 5;
			this.scheduleButton.Text = "Schedule...";
			this.scheduleButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.scheduleButton, "Schedule the selected mirror task.");
			this.scheduleButton.UseVisualStyleBackColor = true;
			this.scheduleButton.Click += new System.EventHandler(this.scheduleButton_Click);
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
			this.listView1.FullRowSelect = true;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(0, 0);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.ShowItemToolTips = true;
			this.listView1.Size = new System.Drawing.Size(495, 184);
			this.listView1.SmallImageList = this.imageList1;
			this.listView1.TabIndex = 0;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
			this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "";
			this.columnHeader1.Width = 20;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Source";
			this.columnHeader2.Width = 180;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Target";
			this.columnHeader3.Width = 180;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Last successful operation";
			this.columnHeader4.Width = 110;
			// 
			// restoreButton
			// 
			this.restoreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.restoreButton.Enabled = false;
			this.restoreButton.Image = global::RoboMirror.Properties.Resources.data_previous24;
			this.restoreButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.restoreButton.Location = new System.Drawing.Point(355, 203);
			this.restoreButton.Name = "restoreButton";
			this.restoreButton.Padding = new System.Windows.Forms.Padding(20, 0, 30, 0);
			this.restoreButton.Size = new System.Drawing.Size(140, 46);
			this.restoreButton.TabIndex = 7;
			this.restoreButton.Text = "Restore";
			this.restoreButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.restoreButton.UseVisualStyleBackColor = true;
			this.restoreButton.Click += new System.EventHandler(this.restoreButton_Click);
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addButton.Image = global::RoboMirror.Properties.Resources.data_copy_add;
			this.addButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.addButton.Location = new System.Drawing.Point(501, 0);
			this.addButton.Name = "addButton";
			this.addButton.Padding = new System.Windows.Forms.Padding(10, 0, 12, 0);
			this.addButton.Size = new System.Drawing.Size(117, 27);
			this.addButton.TabIndex = 1;
			this.addButton.Text = "Add task...";
			this.addButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.addButton, "Add a new mirror task.");
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// backupButton
			// 
			this.backupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.backupButton.Enabled = false;
			this.backupButton.Image = global::RoboMirror.Properties.Resources.data_next24;
			this.backupButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.backupButton.Location = new System.Drawing.Point(0, 203);
			this.backupButton.Name = "backupButton";
			this.backupButton.Padding = new System.Windows.Forms.Padding(20, 0, 27, 0);
			this.backupButton.Size = new System.Drawing.Size(140, 46);
			this.backupButton.TabIndex = 6;
			this.backupButton.Text = "Backup";
			this.backupButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.backupButton.UseVisualStyleBackColor = true;
			this.backupButton.Click += new System.EventHandler(this.backupButton_Click);
			// 
			// editButton
			// 
			this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.editButton.Enabled = false;
			this.editButton.Image = global::RoboMirror.Properties.Resources.data_copy;
			this.editButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.editButton.Location = new System.Drawing.Point(501, 33);
			this.editButton.Name = "editButton";
			this.editButton.Padding = new System.Windows.Forms.Padding(10, 0, 32, 0);
			this.editButton.Size = new System.Drawing.Size(117, 27);
			this.editButton.TabIndex = 2;
			this.editButton.Text = "Edit...";
			this.editButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.editButton, "Edit the selected mirror task.");
			this.editButton.UseVisualStyleBackColor = true;
			this.editButton.Click += new System.EventHandler(this.editButton_Click);
			// 
			// removeButton
			// 
			this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.removeButton.Enabled = false;
			this.removeButton.Image = global::RoboMirror.Properties.Resources.data_copy_delete;
			this.removeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.removeButton.Location = new System.Drawing.Point(501, 67);
			this.removeButton.Name = "removeButton";
			this.removeButton.Padding = new System.Windows.Forms.Padding(10, 0, 23, 0);
			this.removeButton.Size = new System.Drawing.Size(117, 27);
			this.removeButton.TabIndex = 3;
			this.removeButton.Text = "Remove";
			this.removeButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.removeButton, "Remove the selected mirror task.");
			this.removeButton.UseVisualStyleBackColor = true;
			this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(646, 333);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.mainPanel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(617, 364);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "RoboMirror";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.mainPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.Button editButton;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button backupButton;
		private System.Windows.Forms.Button restoreButton;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button scheduleButton;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button historyButton;
	}
}
namespace RoboMirror
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
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.restoreButton = new System.Windows.Forms.Button();
			this.mainPanel = new System.Windows.Forms.Panel();
			this.scheduleButton = new System.Windows.Forms.Button();
			this.addButton = new System.Windows.Forms.Button();
			this.backupButton = new System.Windows.Forms.Button();
			this.editButton = new System.Windows.Forms.Button();
			this.removeButton = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.mainPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
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
			this.listView1.Size = new System.Drawing.Size(494, 190);
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
			this.columnHeader4.Text = "Last backup";
			this.columnHeader4.Width = 110;
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "data_copy.png");
			// 
			// restoreButton
			// 
			this.restoreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.restoreButton.Enabled = false;
			this.restoreButton.Image = global::RoboMirror.Properties.Resources.data_previous24;
			this.restoreButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.restoreButton.Location = new System.Drawing.Point(374, 206);
			this.restoreButton.Name = "restoreButton";
			this.restoreButton.Padding = new System.Windows.Forms.Padding(8, 0, 22, 0);
			this.restoreButton.Size = new System.Drawing.Size(120, 40);
			this.restoreButton.TabIndex = 6;
			this.restoreButton.Text = "Res&tore";
			this.restoreButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.restoreButton.UseVisualStyleBackColor = true;
			this.restoreButton.Click += new System.EventHandler(this.restoreButton_Click);
			// 
			// mainPanel
			// 
			this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mainPanel.Controls.Add(this.scheduleButton);
			this.mainPanel.Controls.Add(this.listView1);
			this.mainPanel.Controls.Add(this.restoreButton);
			this.mainPanel.Controls.Add(this.addButton);
			this.mainPanel.Controls.Add(this.backupButton);
			this.mainPanel.Controls.Add(this.editButton);
			this.mainPanel.Controls.Add(this.removeButton);
			this.mainPanel.Location = new System.Drawing.Point(12, 62);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(600, 246);
			this.mainPanel.TabIndex = 0;
			// 
			// scheduleButton
			// 
			this.scheduleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.scheduleButton.Enabled = false;
			this.scheduleButton.Image = global::RoboMirror.Properties.Resources.clock;
			this.scheduleButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.scheduleButton.Location = new System.Drawing.Point(500, 97);
			this.scheduleButton.Name = "scheduleButton";
			this.scheduleButton.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.scheduleButton.Size = new System.Drawing.Size(100, 23);
			this.scheduleButton.TabIndex = 4;
			this.scheduleButton.Text = "&Schedule...";
			this.scheduleButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.scheduleButton, "Schedule the selected mirror task.");
			this.scheduleButton.UseVisualStyleBackColor = true;
			this.scheduleButton.Click += new System.EventHandler(this.scheduleButton_Click);
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addButton.Image = global::RoboMirror.Properties.Resources.data_copy_add;
			this.addButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.addButton.Location = new System.Drawing.Point(500, 0);
			this.addButton.Name = "addButton";
			this.addButton.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.addButton.Size = new System.Drawing.Size(100, 23);
			this.addButton.TabIndex = 1;
			this.addButton.Text = "&Add task...";
			this.addButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.addButton, "Add a new mirror task.");
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// backupButton
			// 
			this.backupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.backupButton.Enabled = false;
			this.backupButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.backupButton.Image = global::RoboMirror.Properties.Resources.data_next24;
			this.backupButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.backupButton.Location = new System.Drawing.Point(0, 206);
			this.backupButton.Name = "backupButton";
			this.backupButton.Padding = new System.Windows.Forms.Padding(8, 0, 21, 0);
			this.backupButton.Size = new System.Drawing.Size(120, 40);
			this.backupButton.TabIndex = 5;
			this.backupButton.Text = "&Backup";
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
			this.editButton.Location = new System.Drawing.Point(500, 29);
			this.editButton.Name = "editButton";
			this.editButton.Padding = new System.Windows.Forms.Padding(4, 0, 16, 0);
			this.editButton.Size = new System.Drawing.Size(100, 23);
			this.editButton.TabIndex = 2;
			this.editButton.Text = "&Edit...";
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
			this.removeButton.Location = new System.Drawing.Point(500, 58);
			this.removeButton.Name = "removeButton";
			this.removeButton.Padding = new System.Windows.Forms.Padding(4, 0, 12, 0);
			this.removeButton.Size = new System.Drawing.Size(100, 23);
			this.removeButton.TabIndex = 3;
			this.removeButton.Text = "&Remove";
			this.removeButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.removeButton, "Remove the selected mirror task.");
			this.removeButton.UseVisualStyleBackColor = true;
			this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(154)))), ((int)(((byte)(181)))));
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(12, 0, 56, 0);
			this.label1.Size = new System.Drawing.Size(624, 50);
			this.label1.TabIndex = 0;
			this.label1.Text = "Manage your mirror tasks and perform backup and restore operations as you please." +
				"";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(154)))), ((int)(((byte)(181)))));
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(580, 9);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			this.pictureBox1.TabIndex = 1003;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(624, 324);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.mainPanel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(530, 290);
			this.Name = "MainForm";
			this.Text = "RoboMirror";
			this.mainPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
	}
}
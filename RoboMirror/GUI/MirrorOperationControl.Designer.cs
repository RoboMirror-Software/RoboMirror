namespace RoboMirror.GUI
{
	partial class MirrorOperationControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.mainPanel = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.simulateCheckBox = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.statusLabel = new System.Windows.Forms.Label();
			this.startToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.abortToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.mainPanel.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(105, 29);
			this.progressBar.Maximum = 1000;
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(81, 13);
			this.progressBar.Step = 1;
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar.TabIndex = 5;
			this.progressBar.Visible = false;
			// 
			// mainPanel
			// 
			this.mainPanel.Controls.Add(this.label2);
			this.mainPanel.Controls.Add(this.simulateCheckBox);
			this.mainPanel.Controls.Add(this.label1);
			this.mainPanel.Controls.Add(this.progressBar);
			this.mainPanel.Controls.Add(this.label3);
			this.mainPanel.Controls.Add(this.statusLabel);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(197, 44);
			this.mainPanel.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(44, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(151, 26);
			this.label2.TabIndex = 1;
			this.label2.Text = "{0}\r\n{1}";
			// 
			// simulateCheckBox
			// 
			this.simulateCheckBox.AutoSize = true;
			this.simulateCheckBox.BackColor = System.Drawing.Color.Transparent;
			this.simulateCheckBox.Location = new System.Drawing.Point(105, 28);
			this.simulateCheckBox.Name = "simulateCheckBox";
			this.simulateCheckBox.Size = new System.Drawing.Size(85, 17);
			this.simulateCheckBox.TabIndex = 4;
			this.simulateCheckBox.Text = "Simulate first";
			this.simulateCheckBox.UseVisualStyleBackColor = false;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 26);
			this.label1.TabIndex = 0;
			this.label1.Text = "From:\r\nTo:";
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Location = new System.Drawing.Point(3, 29);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(41, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Status:";
			// 
			// statusLabel
			// 
			this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.statusLabel.BackColor = System.Drawing.Color.Transparent;
			this.statusLabel.Location = new System.Drawing.Point(44, 29);
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(151, 13);
			this.statusLabel.TabIndex = 3;
			this.statusLabel.Text = "Pending...";
			// 
			// startToolStripButton
			// 
			this.startToolStripButton.AutoSize = false;
			this.startToolStripButton.Image = global::RoboMirror.Properties.Resources.data_copy;
			this.startToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.startToolStripButton.Margin = new System.Windows.Forms.Padding(1);
			this.startToolStripButton.Name = "startToolStripButton";
			this.startToolStripButton.Size = new System.Drawing.Size(59, 20);
			this.startToolStripButton.Text = " Start";
			this.startToolStripButton.Click += new System.EventHandler(this.startToolStripButton_Click);
			// 
			// abortToolStripButton
			// 
			this.abortToolStripButton.AutoSize = false;
			this.abortToolStripButton.Image = global::RoboMirror.Properties.Resources.delete;
			this.abortToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.abortToolStripButton.Margin = new System.Windows.Forms.Padding(1, 1, 1, 2);
			this.abortToolStripButton.Name = "abortToolStripButton";
			this.abortToolStripButton.Size = new System.Drawing.Size(59, 20);
			this.abortToolStripButton.Text = " Abort";
			this.abortToolStripButton.Click += new System.EventHandler(this.abortToolStripButton_Click);
			// 
			// toolStrip
			// 
			this.toolStrip.AutoSize = false;
			this.toolStrip.CanOverflow = false;
			this.toolStrip.Dock = System.Windows.Forms.DockStyle.Right;
			this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripButton,
            this.abortToolStripButton});
			this.toolStrip.Location = new System.Drawing.Point(197, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.ShowItemToolTips = false;
			this.toolStrip.Size = new System.Drawing.Size(60, 44);
			this.toolStrip.TabIndex = 1;
			// 
			// MirrorOperationControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.mainPanel);
			this.Controls.Add(this.toolStrip);
			this.Name = "MirrorOperationControl";
			this.Size = new System.Drawing.Size(257, 44);
			this.mainPanel.ResumeLayout(false);
			this.mainPanel.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox simulateCheckBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label statusLabel;
		private System.Windows.Forms.ToolStripButton startToolStripButton;
		private System.Windows.Forms.ToolStripButton abortToolStripButton;
		private System.Windows.Forms.ToolStrip toolStrip;
	}
}

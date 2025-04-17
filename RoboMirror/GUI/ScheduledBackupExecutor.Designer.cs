namespace RoboMirror.GUI
{
	partial class ScheduledBackupExecutor
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
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.destinationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.abortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
			this.notifyIcon.Text = "RoboMirroring...";
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.destinationToolStripMenuItem,
            this.abortToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(109, 48);
			// 
			// destinationToolStripMenuItem
			// 
			this.destinationToolStripMenuItem.Enabled = false;
			this.destinationToolStripMenuItem.Name = "destinationToolStripMenuItem";
			this.destinationToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
			this.destinationToolStripMenuItem.Text = "To: {0}";
			// 
			// abortToolStripMenuItem
			// 
			this.abortToolStripMenuItem.Enabled = false;
			this.abortToolStripMenuItem.Image = global::RoboMirror.Properties.Resources.delete;
			this.abortToolStripMenuItem.Name = "abortToolStripMenuItem";
			this.abortToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
			this.abortToolStripMenuItem.Text = "Abort";
			this.abortToolStripMenuItem.Click += new System.EventHandler(this.abortToolStripMenuItem_Click);
			// 
			// ScheduledBackupExecutor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 269);
			this.Name = "ScheduledBackupExecutor";
			this.ShowInTaskbar = false;
			this.Text = "ScheduledBackupExecutor";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem destinationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem abortToolStripMenuItem;
	}
}
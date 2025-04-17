namespace RoboMirror.GUI
{
	partial class BaseDialog
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
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.No;
			this.cancelButton.Image = global::RoboMirror.Properties.Resources.delete;
			this.cancelButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cancelButton.Location = new System.Drawing.Point(210, 0);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
			this.cancelButton.Size = new System.Drawing.Size(117, 37);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.okButton.Image = global::RoboMirror.Properties.Resources.check;
			this.okButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.okButton.Location = new System.Drawing.Point(75, 0);
			this.okButton.Name = "okButton";
			this.okButton.Padding = new System.Windows.Forms.Padding(20, 0, 35, 0);
			this.okButton.Size = new System.Drawing.Size(117, 37);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.cancelButton);
			this.panel1.Controls.Add(this.okButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 256);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(341, 51);
			this.panel1.TabIndex = 1000;
			// 
			// BaseDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(341, 307);
			this.Controls.Add(this.panel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BaseDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "BaseDialog";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;


	}
}
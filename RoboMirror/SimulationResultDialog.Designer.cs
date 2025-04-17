namespace RoboMirror
{
	partial class SimulationResultDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimulationResultDialog));
			this.abortButton = new System.Windows.Forms.Button();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.proceedButton = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.numDeletionsLabel = new System.Windows.Forms.Label();
			this.numFilesTransferredLabel = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// abortButton
			// 
			this.abortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.abortButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.abortButton.Image = global::RoboMirror.Properties.Resources.delete;
			this.abortButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.abortButton.Location = new System.Drawing.Point(519, 351);
			this.abortButton.Name = "abortButton";
			this.abortButton.Padding = new System.Windows.Forms.Padding(9, 0, 24, 0);
			this.abortButton.Size = new System.Drawing.Size(117, 37);
			this.abortButton.TabIndex = 8;
			this.abortButton.Text = "Abort";
			this.abortButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.abortButton.UseVisualStyleBackColor = true;
			// 
			// pictureBox3
			// 
			this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.pictureBox3.Image = global::RoboMirror.Properties.Resources.data_information32;
			this.pictureBox3.Location = new System.Drawing.Point(602, 10);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(37, 37);
			this.pictureBox3.TabIndex = 9;
			this.pictureBox3.TabStop = false;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(14, 0, 65, 0);
			this.label1.Size = new System.Drawing.Size(653, 58);
			this.label1.TabIndex = 0;
			this.label1.Text = "These are the pending changes to \"{0}\".";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// proceedButton
			// 
			this.proceedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.proceedButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.proceedButton.Image = global::RoboMirror.Properties.Resources.check;
			this.proceedButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.proceedButton.Location = new System.Drawing.Point(384, 351);
			this.proceedButton.Name = "proceedButton";
			this.proceedButton.Padding = new System.Windows.Forms.Padding(9, 0, 17, 0);
			this.proceedButton.Size = new System.Drawing.Size(117, 37);
			this.proceedButton.TabIndex = 7;
			this.proceedButton.Text = "Proceed";
			this.proceedButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.proceedButton.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(14, 148);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(45, 15);
			this.label4.TabIndex = 5;
			this.label4.Text = "Details:";
			// 
			// numDeletionsLabel
			// 
			this.numDeletionsLabel.Location = new System.Drawing.Point(276, 112);
			this.numDeletionsLabel.Name = "numDeletionsLabel";
			this.numDeletionsLabel.Size = new System.Drawing.Size(64, 15);
			this.numDeletionsLabel.TabIndex = 4;
			this.numDeletionsLabel.Text = "0";
			this.numDeletionsLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// numFilesTransferredLabel
			// 
			this.numFilesTransferredLabel.Location = new System.Drawing.Point(276, 77);
			this.numFilesTransferredLabel.Name = "numFilesTransferredLabel";
			this.numFilesTransferredLabel.Size = new System.Drawing.Size(64, 15);
			this.numFilesTransferredLabel.TabIndex = 2;
			this.numFilesTransferredLabel.Text = "0";
			this.numFilesTransferredLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::RoboMirror.Properties.Resources.delete224;
			this.pictureBox2.Location = new System.Drawing.Point(17, 106);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(28, 28);
			this.pictureBox2.TabIndex = 4;
			this.pictureBox2.TabStop = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::RoboMirror.Properties.Resources.redo24;
			this.pictureBox1.Location = new System.Drawing.Point(17, 72);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(28, 28);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(52, 112);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(226, 15);
			this.label3.TabIndex = 3;
			this.label3.Text = "Number of files and folders to be deleted:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(52, 77);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(223, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "Number of files and folders to be copied:";
			// 
			// richTextBox1
			// 
			this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.richTextBox1.DetectUrls = false;
			this.richTextBox1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox1.Location = new System.Drawing.Point(17, 166);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(618, 163);
			this.richTextBox1.TabIndex = 6;
			this.richTextBox1.Text = "";
			this.richTextBox1.WordWrap = false;
			// 
			// SimulationResultDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.abortButton;
			this.ClientSize = new System.Drawing.Size(653, 402);
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.abortButton);
			this.Controls.Add(this.proceedButton);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.numDeletionsLabel);
			this.Controls.Add(this.numFilesTransferredLabel);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.richTextBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(419, 262);
			this.Name = "SimulationResultDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Pending changes";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Label numFilesTransferredLabel;
		private System.Windows.Forms.Label numDeletionsLabel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button abortButton;
		private System.Windows.Forms.Button proceedButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.RichTextBox richTextBox1;
	}
}
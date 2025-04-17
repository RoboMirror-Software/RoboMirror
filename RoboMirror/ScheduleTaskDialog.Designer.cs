namespace RoboMirror
{
	partial class ScheduleTaskDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScheduleTaskDialog));
			this.intervalComboBox = new System.Windows.Forms.ComboBox();
			this.timeMaskedTextBox = new System.Windows.Forms.MaskedTextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// intervalComboBox
			// 
			this.intervalComboBox.Enabled = false;
			this.intervalComboBox.FormattingEnabled = true;
			this.intervalComboBox.Items.AddRange(new object[] {
            "Daily",
            "Weekly",
            "Monthly"});
			this.intervalComboBox.Location = new System.Drawing.Point(42, 57);
			this.intervalComboBox.Name = "intervalComboBox";
			this.intervalComboBox.Size = new System.Drawing.Size(132, 21);
			this.intervalComboBox.TabIndex = 1;
			this.intervalComboBox.SelectedIndexChanged += new System.EventHandler(this.control_Changed);
			// 
			// timeMaskedTextBox
			// 
			this.timeMaskedTextBox.Enabled = false;
			this.timeMaskedTextBox.Location = new System.Drawing.Point(203, 57);
			this.timeMaskedTextBox.Mask = "90:00";
			this.timeMaskedTextBox.Name = "timeMaskedTextBox";
			this.timeMaskedTextBox.Size = new System.Drawing.Size(34, 21);
			this.timeMaskedTextBox.TabIndex = 3;
			this.timeMaskedTextBox.ValidatingType = typeof(System.DateTime);
			this.timeMaskedTextBox.TextChanged += new System.EventHandler(this.control_Changed);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(180, 60);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "at";
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(24, 24);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(156, 17);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Text = "Perform automatic backups";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// ScheduleTaskDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(261, 154);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.timeMaskedTextBox);
			this.Controls.Add(this.intervalComboBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ScheduleTaskDialog";
			this.Text = "Schedule";
			this.Controls.SetChildIndex(this.intervalComboBox, 0);
			this.Controls.SetChildIndex(this.timeMaskedTextBox, 0);
			this.Controls.SetChildIndex(this.label2, 0);
			this.Controls.SetChildIndex(this.checkBox1, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox intervalComboBox;
		private System.Windows.Forms.MaskedTextBox timeMaskedTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkBox1;
	}
}
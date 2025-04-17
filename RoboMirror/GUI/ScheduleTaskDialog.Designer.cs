namespace RoboMirror.GUI
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
			this.label2 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.datePicker = new System.Windows.Forms.DateTimePicker();
			this.timePicker = new System.Windows.Forms.DateTimePicker();
			this.SuspendLayout();
			// 
			// intervalComboBox
			// 
			this.intervalComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.intervalComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.intervalComboBox.Enabled = false;
			this.intervalComboBox.FormattingEnabled = true;
			this.intervalComboBox.Items.AddRange(new object[] {
            "Daily",
            "Weekly",
            "Monthly"});
			this.intervalComboBox.Location = new System.Drawing.Point(41, 58);
			this.intervalComboBox.Name = "intervalComboBox";
			this.intervalComboBox.Size = new System.Drawing.Size(319, 23);
			this.intervalComboBox.TabIndex = 1;
			this.intervalComboBox.SelectedIndexChanged += new System.EventHandler(this.control_Changed);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(38, 97);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "Beginning:";
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(20, 20);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(173, 19);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Text = "Perform automatic backups";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// datePicker
			// 
			this.datePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.datePicker.Enabled = false;
			this.datePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePicker.Location = new System.Drawing.Point(108, 93);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new System.Drawing.Size(117, 23);
			this.datePicker.TabIndex = 3;
			this.datePicker.ValueChanged += new System.EventHandler(this.control_Changed);
			// 
			// timePicker
			// 
			this.timePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.timePicker.Enabled = false;
			this.timePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.timePicker.Location = new System.Drawing.Point(243, 93);
			this.timePicker.Name = "timePicker";
			this.timePicker.ShowUpDown = true;
			this.timePicker.Size = new System.Drawing.Size(117, 23);
			this.timePicker.TabIndex = 4;
			this.timePicker.ValueChanged += new System.EventHandler(this.control_Changed);
			// 
			// ScheduleTaskDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(374, 197);
			this.Controls.Add(this.timePicker);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.intervalComboBox);
			this.Controls.Add(this.datePicker);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ScheduleTaskDialog";
			this.Text = "Schedule";
			this.Controls.SetChildIndex(this.datePicker, 0);
			this.Controls.SetChildIndex(this.intervalComboBox, 0);
			this.Controls.SetChildIndex(this.label2, 0);
			this.Controls.SetChildIndex(this.checkBox1, 0);
			this.Controls.SetChildIndex(this.timePicker, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox intervalComboBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.DateTimePicker datePicker;
		private System.Windows.Forms.DateTimePicker timePicker;
	}
}
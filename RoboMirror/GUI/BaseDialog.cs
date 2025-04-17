/*
 * Copyright (c) Martin Kinkelin
 *
 * See the "License.txt" file in the root directory for infos
 * about permitted and prohibited uses of this code.
 */

using System;
using System.Windows.Forms;

namespace RoboMirror.GUI
{
	/// <summary>
	/// Base of all dialogs.
	/// Sets some typical properties for dialogs, contains an "OK" and a "Cancel"
	/// button and manages the DialogResult in a way that
	/// a) DialogResult.Yes means that there were applied changes,
	/// b) DialogResult.No means that either there were no changes or the changes
	///    have been discarded.
	/// </summary>
	public partial class BaseDialog : BaseForm
	{
		/// <summary>
		/// Gets or sets a value indicating whether the dialog has changed.
		/// If it has changed and the dialog is being closed by the user, the
		/// user will be prompted to apply or discard the changes.
		/// </summary>
		protected bool HasChanged { get; set; }


		/// <summary>
		/// Creates a new BaseDialog.
		/// </summary>
		public BaseDialog()
		{
			InitializeComponent();
		}


		/// <summary>
		/// Invoked when the dialog has been shown.
		/// </summary>
		protected override void OnShown(EventArgs e)
		{
			// make sure the HasChanged property is set to false because
			// event handlers invoked during initialization may have
			// already set it to true
			HasChanged = false;

			base.OnShown(e);
		}

		/// <summary>
		/// Invoked when the dialog is about to be closed.
		/// </summary>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			// fire the event
			base.OnFormClosing(e);

			// return if an event handler already cancelled the operation
			if (e.Cancel)
				return;

			// prompt the user to save the changes if no button has been pressed
			if (HasChanged && DialogResult != DialogResult.Yes && DialogResult != DialogResult.No)
			{
				DialogResult answer = MessageBox.Show(this, "Would you like to save your changes?",
					"Pending changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

				if (answer == DialogResult.Cancel)
				{
					e.Cancel = true;
					return;
				}

				// simulate having pressed one of the two buttons
				DialogResult = answer;
			}

			if (DialogResult == DialogResult.Yes)
			{
				if (!ApplyChanges())
				{
					e.Cancel = true;
					DialogResult = System.Windows.Forms.DialogResult.None;
					return;
				}
			}
			else
				DialogResult = DialogResult.No;
		}

		/// <summary>
		/// Tries to apply the changes.
		/// If the method returns false, the dialog is not closed.
		/// </summary>
		protected virtual bool ApplyChanges()
		{
			throw new NotImplementedException("BaseDialog.ApplyChanges()");
		}
	}
}

using System;
using System.Drawing;
using System.Windows.Forms;
using Utility.Input.Enums;

namespace Utility.Input
{
	/// <summary>The form for setting a keyboard key.</summary>
	public partial class SetKey : Form
	{
		#region Members
		/// <summary>The key being created or editted.</summary>
		private Key key { get; set; }
		#endregion Members

		#region Properties
		/// <summary>Gets or sets the internal key.</summary>
		public Key Key
		{
			get { return this.key; }
			set { this.key = value; }
		}
		#endregion Properties

		#region Constructors
		/// <summary>Default constructor, used in property grid.</summary>
		public SetKey()
		{
			InitializeComponent();
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ControlBox = false;
			this.ShowInTaskbar = false;
			this.TopLevel = false;
			this.acceptBtn.Visible = false;
			this.cancelBtn.Visible = false;
			this.FormBorderStyle = FormBorderStyle.None;
			key = new Key();
			foreach (VirtualKeyCode vk in Enum.GetValues(typeof(VirtualKeyCode)))
			{
				this.keyChoices.Items.Add(vk);
			}
			for (int i = 0x30; i < 0x40; i++)
			{
				this.shiftKeys.Items.Add((VirtualKeyCode)i);
			}
		}

		/// <summary>Default constructor for editting a key. Used for property grid.</summary>
		/// <param name="key">The key to be editted.</param>
		public SetKey(Key key)
		{
			InitializeComponent();
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ControlBox = false;
			this.ShowInTaskbar = false;
			this.TopLevel = false;
			this.TopMost = true;
			this.acceptBtn.Visible = false;
			this.cancelBtn.Visible = false;
			this.FormBorderStyle = FormBorderStyle.None;
			this.key = key;

			foreach (VirtualKeyCode tempVK in Enum.GetValues(typeof(VirtualKeyCode)))
			{
				this.keyChoices.Items.Add(tempVK);
			}
			for (int i = 0x30; i < 0x40; i++)
			{
				this.shiftKeys.Items.Add((VirtualKeyCode)i);
			}
			if ((this.key.ShiftType & ShiftType.ALT) == ShiftType.ALT)
				this.altChkBox.Checked = true;
			if ((this.key.ShiftType & ShiftType.CTRL) == ShiftType.CTRL)
				this.ctrlChkBox.Checked = true;
			if ((this.key.ShiftType & ShiftType.SHIFT) == ShiftType.SHIFT)
				this.shiftChkBox.Checked = true;
			this.keyChoices.SelectedItem = key.Vk;
			this.shiftKeys.SelectedItem = key.ShiftKey;
		}

		/// <summary>Constructor for key bindings not in a property grid.</summary>
		/// <param name="p">Where the form should load.</param>
		/// <param name="key">The key to be editted.</param>
		public SetKey(Point p, Key key)
		{
			InitializeComponent();
			this.key = key;
			this.TopMost = true;
			foreach (VirtualKeyCode tempVK in Enum.GetValues(typeof(VirtualKeyCode)))
			{
				this.keyChoices.Items.Add(tempVK);
			}
			for (int i = 0x30; i < 0x3A; i++)
			{
				this.shiftKeys.Items.Add((VirtualKeyCode)i);
			}
			if ((this.key.ShiftType & ShiftType.ALT) == ShiftType.ALT)
				this.altChkBox.Checked = true;
			if ((this.key.ShiftType & ShiftType.CTRL) == ShiftType.CTRL)
				this.ctrlChkBox.Checked = true;
			if ((this.key.ShiftType & ShiftType.SHIFT) == ShiftType.SHIFT)
				this.shiftChkBox.Checked = true;
			this.keyChoices.SelectedItem = this.key.Vk;
			this.shiftKeys.SelectedItem = key.ShiftKey;
		}
		#endregion Constructors

		#region Methods
		#region Events
		/// <summary>The event for handling the accept button press.</summary>
		/// <param name="sender">The accept button.</param>
		/// <param name="e">The button click event.</param>
		private void acceptBtn_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		/// <summary>Event to handle the key change.</summary>
		/// <param name="sender">The dropdown box.</param>
		/// <param name="e">The selected index change mouse event.</param>
		private void keyChoices_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.key.Vk = (VirtualKeyCode)this.keyChoices.SelectedItem;
		}

		/// <summary>Event to handle the shift keys index change.</summary>
		/// <param name="sender">The dropdown box.</param>
		/// <param name="e">The click event.</param>
		private void shiftKeys_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.key.ShiftKey = (VirtualKeyCode)this.shiftKeys.SelectedItem;
		}

		/// <summary>Event to handle the alt checkbox being enabled/disabled.</summary>
		/// <param name="sender">The alt checkbox</param>
		/// <param name="e">The event.</param>
		private void altChkBox_CheckedChanged(object sender, EventArgs e)
		{
			if (this.altChkBox.Checked)
			{
				this.key.ShiftType |= ShiftType.ALT;
			}
			else
			{
				this.key.ShiftType ^= ShiftType.ALT;
			}
		}

		/// <summary>Event to handle the shift checkbox being enabled/disabled.</summary>
		/// <param name="sender">The shift checkbox</param>
		/// <param name="e">The event.</param>
		private void shiftChkBox_CheckedChanged(object sender, EventArgs e)
		{
			if (this.shiftChkBox.Checked)
			{
				this.key.ShiftType |= ShiftType.SHIFT;
				this.shiftKeys.Enabled = true;
			}
			else
			{
				this.key.ShiftType ^= ShiftType.SHIFT;
				this.key.ShiftKey = VirtualKeyCode.NULL;
				this.shiftKeys.Enabled = false;
			}
		}

		/// <summary>Event to handle the ctrl checkbox being enabled/disabled.</summary>
		/// <param name="sender">The ctrl checkbox</param>
		/// <param name="e">The event.</param>
		private void ctrlChkBox_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ctrlChkBox.Checked)
			{
				this.key.ShiftType |= ShiftType.CTRL;
			}
			else
			{
				this.key.ShiftType ^= ShiftType.CTRL;
			}
		}

		/// <summary>Event to handle the cancel button</summary>
		/// <param name="sender">The cancel button sending the event.</param>
		/// <param name="e">The event arguments.</param>
		private void cancelBtn_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Abort;
			this.Close();
		}

		/// <summary>Event to handle key presses inside key choices.</summary>
		/// <param name="sender">The drop down list, combo box of key choices.</param>
		/// <param name="e">The event arguments.</param>
		private void keyChoices_KeyDown(object sender, KeyEventArgs e)
		{
			HandleKey(sender, e);
		}

		/// <summary>Event to handle key presses inside the shift key combo box.</summary>
		/// <param name="sender">The shift key combo box.</param>
		/// <param name="e">The event arguments</param>
		private void shiftKeys_KeyDown(object sender, KeyEventArgs e)
		{
			HandleKey(sender, e);
		}
		#endregion Events

		#region Private
		/// <summary>Handles key presses and converts to key options.</summary>
		/// <param name="sender">The caller of this function</param>
		/// <param name="e">The key press event arguments.</param>
		private void HandleKey(object sender, KeyEventArgs e)
		{
			if (e.Alt)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
				if (altChkBox.Checked)
				{
					altChkBox.Checked = false;
				}
				else
				{
					altChkBox.Checked = true;
				}
			}
			else if (e.Control)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
				if (ctrlChkBox.Checked)
				{
					ctrlChkBox.Checked = false;
				}
				else
				{
					ctrlChkBox.Checked = true;
				}
			}
			else if (e.Shift)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
				if (shiftChkBox.Checked)
				{
					shiftChkBox.Checked = false;
				}
				else
				{
					shiftChkBox.Checked = true;
				}
			}
			if (e.KeyValue != 0)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
				ComboBox lb = sender as ComboBox;
				if (lb.Name == shiftKeys.Name)
				{
					shiftKeys.SelectedItem = (VirtualKeyCode)e.KeyValue;
				}
				else if (lb.Name == keyChoices.Name)
				{
					keyChoices.SelectedItem = (VirtualKeyCode)e.KeyValue;
				}
			}
		}
		#endregion Private
		#endregion Methods
	}
}

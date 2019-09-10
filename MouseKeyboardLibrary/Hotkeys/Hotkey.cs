using System;
using System.Windows.Forms;

namespace MouseKeyboardLibrary.Hotkeys
{
    /// <summary>
    /// A class to configure hotkeys. Must be registered with a HotkeyHook instance.
    /// </summary>
    public class Hotkey
    {
        /// <summary>
        ///     If the hotkey is pressed, surpress further processing of the input by other applications
        /// </summary>
        public bool SuppressHotkeys { get; set; }

        /// <summary>
        ///     THE KEY - "Caution hot"
        /// </summary>
        public Keys? Key { get; set; }

        /// <summary>
        ///     Set to true if Control must be pressed
        /// </summary>
        public bool Control { get; set; }

        /// <summary>
        ///     Set to true if Alt must be pressed
        /// </summary>
        public bool Alt { get; set; }

        /// <summary>
        ///     Set to true if Shift must be pressed
        /// </summary>
        public bool Shift { get; set; }

        /// <summary>
        /// If set to true, the Hotkey will change its values to the next registered input
        /// </summary>
        public bool ChangeOnNextInput { get; set; }

        /// <summary>
        /// Fires when pressed hotkey is detected
        /// </summary>
        public event EventHandler Pressed;

        /// <summary>
        /// Fires when hotkey condiguration changed
        /// </summary>
        public event EventHandler Changed;

        private bool IsPressed(KeyEventArgs e)
        {
            return !ChangeOnNextInput &&
                   Key == e.KeyCode &&
                   Control == e.Control &&
                   Shift == e.Shift &&
                   Alt == e.Alt;
        }
       
        internal void ProcessKeyeventArgs(KeyEventArgs e)
        {
            if (ChangeHotkey(e) ||!IsPressed(e)) return;

            e.SuppressKeyPress = SuppressHotkeys;
            Pressed?.Invoke(this, e);
        }

        private bool ChangeHotkey(KeyEventArgs e)
        {
            if (!ChangeOnNextInput || e.Modifiers == Keys.None) return false;

            Key               = e.KeyCode;
            Control           = e.Control;
            Shift             = e.Shift;
            Alt               = e.Shift;
            ChangeOnNextInput = false;
            Changed?.Invoke(this, new EventArgs());

            return true;
        }

        public override string ToString()
        {
            var control = Control ? " + Control" : string.Empty;
            var shift = Shift ? " + Shift" : string.Empty;
            var alt = Alt ? " + Alt" : string.Empty;
            return $"{Key}{control}{shift}{alt}";
        }
    }
}
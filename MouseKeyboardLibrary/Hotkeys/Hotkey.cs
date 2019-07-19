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
        public bool SurpressKeypress { get; set; }

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
        /// Thrown the moment the keys configured are pressed
        /// </summary>
        public event EventHandler Pressed;

        private bool IsPressed(KeyEventArgs e)
        {
            return Key == e.KeyCode &&
                   Control == e.Control &&
                   Shift == e.Shift &&
                   Alt == e.Alt;
        }

        internal void ProcessKeyeventArgs(KeyEventArgs e)
        {
            if (!IsPressed(e)) return;

            e.SuppressKeyPress = SurpressKeypress;
            Pressed?.Invoke(this, e);
        }
    }
}
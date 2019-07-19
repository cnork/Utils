using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MouseKeyboardLibrary.Hotkeys
{
    /// <summary>
    /// Keyboard hook for easy hotkey registration
    /// </summary>
    public class HotkeyHook : IDisposable
    {
        private readonly KeyboardHook keyboardHook = new KeyboardHook();
        private readonly List<Hotkey> hotkeys = new List<Hotkey>();

        /// <summary>
        /// Create a new instance of HotkeyHook
        /// </summary>
        /// <param name="global">set this if you want the hotkeys to be registered globally</param>
        public HotkeyHook(bool global)
        {
            keyboardHook.Start(!global);
            LinkEvents();
        }

        /// <summary>
        /// Registers and activates the given hotkey
        /// </summary>
        /// <param name="hotkey"></param>
        public void Register(Hotkey hotkey)
        {
            hotkeys.Add(hotkey);
        }

        /// <summary>
        /// Unegisters and deactivates the given hotkey
        /// </summary>
        /// <param name="hotkey"></param>
        public void Unregister(Hotkey hotkey)
        {
            hotkeys.Remove(hotkey);
        }

        private void LinkEvents()
        {
            keyboardHook.KeyDown += KeyboardHook_KeyDown;
        }

        private void KeyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            hotkeys.ForEach(x => x.ProcessKeyeventArgs(e));
        }

        public void Dispose()
        {
            UnlinkEvents();
            keyboardHook.Stop();
        }

        private void UnlinkEvents()
        {
            keyboardHook.KeyDown -= KeyboardHook_KeyDown;
        }
    }
}

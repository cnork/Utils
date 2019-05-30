using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Input.Enums
{
    public enum Message
    {
        NCHITTEST = (0x0084),
        KEY_DOWN = (0x0100), //Key down
        KEY_UP = (0x0101), //Key Up
        VM_CHAR = (0x0102), //The character being pressed
        SYSKEYDOWN = (0x0104), //An Alt/ctrl/shift + key down message
        SYSKEYUP = (0x0105), //An Alt/Ctrl/Shift + Key up Message
        SYSCHAR = (0x0106), //An Alt/Ctrl/Shift + Key character Message
        LBUTTONDOWN = (0x201), //Left mousebutton down 
        LBUTTONUP = (0x202), //Left mousebutton up 
        LBUTTONDBLCLK = (0x203), //Left mousebutton doubleclick 
        RBUTTONDOWN = (0x204), //Right mousebutton down 
        RBUTTONUP = (0x205), //Right mousebutton up 
        RBUTTONDBLCLK = (0x206), //Right mousebutton doubleclick

        /// <summary>Middle mouse button down</summary>
        MBUTTONDOWN = (0x207),

        /// <summary>Middle mouse button up</summary>
        MBUTTONUP = (0x208)
    }
}

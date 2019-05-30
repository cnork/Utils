using System;

namespace Utility.Input.Enums
{
    [Serializable]
    public enum ShiftType
    {
        NONE = 0x0,
        SHIFT = 0x1,
        CTRL = 0x2,
        SHIFT_CTRL = SHIFT | CTRL,
        ALT = 0x4,
        SHIFT_ALT = ALT | SHIFT,
        CTRL_ALT = CTRL | ALT,
        SHIFT_CTRL_ALT = SHIFT | CTRL | ALT
    }
}

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    public enum MouseButton
    {
        Left = 0x2,
        Right = 0x8,
        Middle = 0x20
    }

    public static class MouseSimulator
    {
        #region Windows API Code

        [DllImport("user32.dll")]
        static extern int ShowCursor(bool show);

        [DllImport("user32.dll")]
        static extern void mouse_event(int flags, int dX, int dY, int buttons, int extraInfo);

        const int MOUSEEVENTF_MOVE = 0x1;
        const int MOUSEEVENTF_LEFTDOWN = 0x2;
        const int MOUSEEVENTF_LEFTUP = 0x4;
        const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        const int MOUSEEVENTF_RIGHTUP = 0x10;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        const int MOUSEEVENTF_MIDDLEUP = 0x40;
        const int MOUSEEVENTF_WHEEL = 0x800;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; 

        #endregion

        #region Properties
    
        public static Point Position
        {
            get
            {
                return new Point(Cursor.Position.X, Cursor.Position.Y);
            }
            set
            {
                Cursor.Position = value;
            }
        }

  
        public static int X
        {
            get
            {
                return Cursor.Position.X;
            }
            set
            {
                Cursor.Position = new Point(value, Y);
            }
        }

 
        public static int Y
        {
            get
            {
                return Cursor.Position.Y;
            }
            set
            {
                Cursor.Position = new Point(X, value);
            }
        } 

        #endregion

        #region Methods


        public static void MouseDown(MouseButton button)
        {
            mouse_event(((int)button), 0, 0, 0, 0);
        }

        public static void MouseDown(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    MouseDown(MouseButton.Left);
                    break;
                case MouseButtons.Middle:
                    MouseDown(MouseButton.Middle);
                    break;
                case MouseButtons.Right:
                    MouseDown(MouseButton.Right);
                    break;
            }
        }
   
        public static void MouseUp(MouseButton button)
        {
            mouse_event(((int)button) * 2, 0, 0, 0, 0);
        }

        public static void MouseUp(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    MouseUp(MouseButton.Left);
                    break;
                case MouseButtons.Middle:
                    MouseUp(MouseButton.Middle);
                    break;
                case MouseButtons.Right:
                    MouseUp(MouseButton.Right);
                    break;
            }
        }

        public static void Click(MouseButton button, int delay = 0)
        {
            MouseDown(button);

            if (delay > 0)
                Thread.Sleep(delay);

            MouseUp(button);
        }


        public static void Click(MouseButtons button, int delay = 0)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    Click(MouseButton.Left, delay);
                    break;
                case MouseButtons.Middle:
                    Click(MouseButton.Middle, delay);
                    break;
                case MouseButtons.Right:
                    Click(MouseButton.Right, delay);
                    break;
            }
        }

        public static void DoubleClick(MouseButton button, int delay = 0)
        {
            Click(button, delay);
            Click(button, delay);
        }

        public static void DoubleClick(MouseButtons button, int delay = 0)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    DoubleClick(MouseButton.Left, delay);
                    break;
                case MouseButtons.Middle:
                    DoubleClick(MouseButton.Middle, delay);
                    break;
                case MouseButtons.Right:
                    DoubleClick(MouseButton.Right, delay);
                    break;
            }
        }

        public static void MouseWheel(int delta)
        {

            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, delta, 0);

        }

        public static void Show()
        {
            ShowCursor(true);
        }

   
        public static void Hide()
        {
            ShowCursor(false);
        }

        public static void Move(int xDelta, int yDelta)
        {
            mouse_event(MOUSEEVENTF_MOVE, xDelta, yDelta, 0, 0);
        }

        #endregion
    }

}

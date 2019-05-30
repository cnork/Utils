using System;
using System.Runtime.InteropServices;
using System.Threading;
using Utility.Input.Enums;

namespace Utility.Input
{
	/// <summary>Class for messaging and key presses</summary>
	[Serializable]
	public class Input
	{
		#region Constants

		/// <summary>Maps a virtual key to a key code.</summary>
		private const uint MAPVK_VK_TO_VSC = 0x00;

		/// <summary>Maps a key code to a virtual key.</summary>
		private const uint MAPVK_VSC_TO_VK = 0x01;

		/// <summary>Maps a virtual key to a character.</summary>
		private const uint MAPVK_VK_TO_CHAR = 0x02;

		/// <summary>Maps a key code to a virtual key with specified keyboard.</summary>
		private const uint MAPVK_VSC_TO_VK_EX = 0x03;

		/// <summary>Maps a virtual key to a key code with specified keyboard.</summary>
		private const uint MAPVK_VK_TO_VSC_EX = 0x04;

		/// <summary>Code if the key is toggled.</summary>
		private const ushort KEY_TOGGLED = 0x1;

		/// <summary>Code for if the key is pressed.</summary>
		private const ushort KEY_PRESSED = 0xF000;

		/// <summary>Code for no keyboard event.</summary>
		private const uint KEYEVENTF_NONE = 0x0;

		/// <summary>Code for extended key pressed.</summary>
		private const uint KEYEVENTF_EXTENDEDKEY = 0x1;

		/// <summary>Code for keyup event.</summary>
		private const uint KEYEVENTF_KEYUP = 0x2;

		/// <summary>Mouse input type.</summary>
		private const int INPUT_MOUSE = 0;

		/// <summary>Keyboard input type.</summary>
		private const int INPUT_KEYBOARD = 1;

		/// <summary>Hardware input type.</summary>
		private const int INPUT_HARDWARE = 2;

        #endregion Constants

        #region Windows API Imports

        [DllImport("user32.dll")]
        static extern short VkKeyScan(char ch);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetMessageExtraInfo();

        /// <summary>Gets the key state of the specified key.</summary>
        /// <param name="nVirtKey">The key to check.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern ushort GetKeyState(int nVirtKey);

        /// <summary>Gets the state of the entire keyboard.</summary>
        /// <param name="lpKeyState">The byte array to receive all the keys states.</param>
        /// <returns>Whether it succeed or failed.</returns>
        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        /// <summary>Allows for foreground hardware keyboard key presses</summary>
        /// <param name="nInputs">The number of inputs in pInputs</param>
        /// <param name="pInputs">A Input structure for what is to be pressed.</param>
        /// <param name="cbSize">The size of the structure.</param>
        /// <returns>A message.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        /// <summary>
        ///     The GetForegroundWindow function returns a handle to the foreground window.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SendMessage(IntPtr hWnd, int wMsg, uint wParam, uint lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        #endregion

        #region Structures

	    [StructLayout(LayoutKind.Explicit)]
	    struct Helper
	    {
	        [FieldOffset(0)] public short Value;
	        [FieldOffset(0)] public byte Low;
	        [FieldOffset(1)] public byte High;
	    };

	    public struct KEYBDINPUT
	    {
	        public ushort wVk;
	        public ushort wScan;
	        public uint dwFlags;
	        public long time;
	        public uint dwExtraInfo;
	    };

	    [StructLayout(LayoutKind.Explicit, Size = 28)]
	    public struct INPUT
	    {
	        [FieldOffset(0)] public uint type;
	        [FieldOffset(4)] public KEYBDINPUT ki;
	    };

		#endregion //Structures

		#region Methods

		#region Public

		public static bool IsKeyPressed(Key key)
		{
			if ((GetKeyState((int) key.Vk) & 0xF0) == 1)
				return true;

			return false;
		}

		public static uint GetVirtualKeyCode(char c)
		{
			var helper = new Helper { Value = VkKeyScan(c) };
			byte virtualKeyCode = helper.Low;
			return virtualKeyCode;
		}

		public static void BackgroundMousePosition(IntPtr hWnd, int x, int y)
		{
			PostMessage(hWnd, (int) WindowsMessages.WM_MOUSEMOVE, 0, GetLParam(x, y));
		}

		public static void BackgroundMouseClick(IntPtr hWnd, Key key, int x, int y, int delay = 100)
		{
			switch (key.Vk)
			{
				case VirtualKeyCode.MBUTTON:
					PostMessage(hWnd, (int) Message.MBUTTONDOWN, (uint) key.Vk, GetLParam(x, y));
					Thread.Sleep(delay);
					PostMessage(hWnd, (int) Message.MBUTTONUP, (uint) key.Vk, GetLParam(x, y));
					break;
				case VirtualKeyCode.LBUTTON:
					PostMessage(hWnd, (int) Message.LBUTTONDOWN, (uint) key.Vk, GetLParam(x, y));
					Thread.Sleep(delay);
					PostMessage(hWnd, (int) Message.LBUTTONUP, (uint) key.Vk, GetLParam(x, y));
					break;
				case VirtualKeyCode.RBUTTON:
					PostMessage(hWnd, (int) Message.RBUTTONDOWN, (uint) key.Vk, GetLParam(x, y));
					Thread.Sleep(delay);
					PostMessage(hWnd, (int) Message.RBUTTONUP, (uint) key.Vk, GetLParam(x, y));
					break;
			}
		}

		public static void SendChatTextPost(IntPtr hWnd, string msg)
		{
			PostMessage(hWnd, new Key(VirtualKeyCode.RETURN));

			foreach (char c in msg)
			{
				PostMessage(hWnd, new Key(c));
			}

			PostMessage(hWnd, new Key(VirtualKeyCode.RETURN));
		}

		public static void SendChatTextSend(IntPtr hWnd, string msg)
		{
			SendMessage(hWnd, new Key(VirtualKeyCode.RETURN), true);
			foreach (char c in msg)
			{
				SendChar(hWnd, c, true);
			}
			SendMessage(hWnd, new Key(VirtualKeyCode.RETURN), true);
		}

		public static bool ForegroundKeyPress(Key key, int delay = 100)
		{
			bool temp = true;

			temp &= ForegroundKeyDown(key);
			Thread.Sleep(delay);
			temp &= ForegroundKeyUp(key);
			Thread.Sleep(delay);
			return temp;
		}

		public static bool ForegroundKeyPress(IntPtr hWnd, Key key, int delay = 100)
		{
			bool temp = true;

			temp &= ForegroundKeyDown(hWnd, key);
			Thread.Sleep(delay);
			temp &= ForegroundKeyUp(hWnd, key);
			Thread.Sleep(delay);
			return temp;
		}

		public static bool ForegroundKeyDown(Key key)
		{
			uint intReturn;
			INPUT structInput;
			structInput = new INPUT();
			structInput.type = INPUT_KEYBOARD;

			// Key down shift, ctrl, and/or alt
			structInput.ki.wScan = 0;
			structInput.ki.time = 0;
			structInput.ki.dwFlags = 0;
			// Key down the actual key-code
			structInput.ki.wVk = (ushort) key.Vk;
			intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));

			// Key up shift, ctrl, and/or alt
			//keybd_event((int)key.VK, GetScanCode(key.VK) + 0x80, KEYEVENTF_NONE, 0);
			//keybd_event((int)key.VK, GetScanCode(key.VK) + 0x80, KEYEVENTF_KEYUP, 0);
			return true;
		}

		public static bool ForegroundKeyUp(Key key)
		{
			uint intReturn;
			INPUT structInput;
			structInput = new INPUT();
			structInput.type = INPUT_KEYBOARD;

			// Key down shift, ctrl, and/or alt
			structInput.ki.wScan = 0;
			structInput.ki.time = 0;
			structInput.ki.dwFlags = 0;
			// Key down the actual key-code
			structInput.ki.wVk = (ushort) key.Vk;

			// Key up the actual key-code
			structInput.ki.dwFlags = KEYEVENTF_KEYUP;
			intReturn = SendInput(1, ref structInput, Marshal.SizeOf(typeof (INPUT)));
			return true;
		}

		public static bool ForegroundKeyDown(IntPtr hWnd, Key key)
		{
			if (GetForegroundWindow() != hWnd)
			{
				if (!SetForegroundWindow(hWnd))
					return false;
			}
			return ForegroundKeyDown(key);
		}

		public static bool ForegroundKeyUp(IntPtr hWnd, Key key)
		{
			if (GetForegroundWindow() != hWnd)
			{
				if (!SetForegroundWindow(hWnd))
					return false;
			}
			return ForegroundKeyUp(key);
		}

		public static bool ForegroundKeyPressAll(IntPtr hWnd, Key key, bool alt, bool ctrl, bool shift, int delay = 100)
		{
			if (GetForegroundWindow() != hWnd)
			{
				if (!SetForegroundWindow(hWnd))
					return false;
			}
			uint intReturn;
			INPUT structInput;
			structInput = new INPUT();
			structInput.type = INPUT_KEYBOARD;

			// Key down shift, ctrl, and/or alt
			structInput.ki.wScan = 0;
			structInput.ki.time = 0;
			structInput.ki.dwFlags = 0;
			if (alt)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.MENU;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.CONTROL;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (shift)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.SHIFT;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);

				if (key.ShiftKey != VirtualKeyCode.NULL)
				{
					structInput.ki.wVk = (ushort) key.ShiftKey;
					intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
					Thread.Sleep(delay);
				}
			}

			// Key up the actual key-code			
			ForegroundKeyPress(hWnd, key);

			structInput.ki.dwFlags = KEYEVENTF_KEYUP;
			if (shift && key.ShiftKey == VirtualKeyCode.NULL)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.SHIFT;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.CONTROL;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (alt)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.MENU;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			return true;
		}

		public static bool PostMessage(IntPtr hWnd, Key key, int delay = 100)
		{
			//Send KEY_DOWN
			if (PostMessage(hWnd, (int) Message.KEY_DOWN, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);
			//Send VM_CHAR
			if (PostMessage(hWnd, (int) Message.VM_CHAR, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);
			if (PostMessage(hWnd, (int) Message.KEY_UP, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);

			return true;
		}

		public static bool PostMessageAll(IntPtr hWnd, Key key, bool alt, bool ctrl, bool shift, int delay = 100)
		{
			CheckKeyShiftState();
			uint intReturn;
			INPUT structInput;
			structInput = new INPUT();
			structInput.type = INPUT_KEYBOARD;

			// Key down shift, ctrl, and/or alt
			structInput.ki.wScan = 0;
			structInput.ki.time = 0;
			structInput.ki.dwFlags = 0;
			if (alt)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.MENU;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.CONTROL;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (shift)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.SHIFT;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);

				if (key.ShiftKey != VirtualKeyCode.NULL)
				{
					//Send KEY_DOWN
					if (PostMessage(hWnd, (int) Message.KEY_DOWN, (uint) key.Vk, GetLParam(1, key.ShiftKey, 0, 0, 0, 0)))
						return false;
					Thread.Sleep(delay);
				}
			}

			PostMessage(hWnd, key);

			structInput.ki.dwFlags = KEYEVENTF_KEYUP;
			if (shift && key.ShiftKey == VirtualKeyCode.NULL)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.SHIFT;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.CONTROL;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (alt)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.MENU;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}

			return true;
		}

		public static bool SendMessageDown(IntPtr hWnd, Key key, bool checkKeyboardState, int delay = 100)
		{
			if (checkKeyboardState)
				CheckKeyShiftState();
			//Send KEY_DOWN
			if (SendMessage(hWnd, (int) Message.KEY_DOWN, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);

			//Send VM_CHAR
			if (SendMessage(hWnd, (int) Message.VM_CHAR, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);

			return true;
		}

		public static bool SendMessageUp(IntPtr hWnd, Key key, bool checkKeyboardState, int delay = 100)
		{
			if (checkKeyboardState)
				CheckKeyShiftState();

			//Send KEY_UP
			if (SendMessage(hWnd, (int) Message.KEY_UP, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 1, 1)))
				return false;
			Thread.Sleep(delay);

			return true;
		}

		public static bool SendChar(IntPtr hWnd, char c, bool checkKeyboardState)
		{
			if (checkKeyboardState)
				CheckKeyShiftState();

			//Send VM_CHAR
			if (SendMessage(hWnd, (int) Message.VM_CHAR, c, 0))
				return false;

			return true;
		}

		public static bool SendMessage(IntPtr hWnd, Key key, bool checkKeyboardState, int delay = 100)
		{
			if (checkKeyboardState)
				CheckKeyShiftState();

			//Send KEY_DOWN
			if (SendMessage(hWnd, (int) Message.KEY_DOWN, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);

			//Send VM_CHAR
			if (SendMessage(hWnd, (int) Message.VM_CHAR, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);

			//Send KEY_UP
			if (SendMessage(hWnd, (int) Message.KEY_UP, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 1, 1)))
				return false;
			Thread.Sleep(delay);

			return true;
		}

		public static bool SendMessageAll(IntPtr hWnd, Key key, bool alt, bool ctrl, bool shift, int delay = 100)
		{
			CheckKeyShiftState();
			uint intReturn;
			INPUT structInput = new INPUT {type = INPUT_KEYBOARD, ki = {wScan = 0, time = 0, dwFlags = 0}};

			// Key down shift, ctrl, and/or alt
			if (alt)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.MENU;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.CONTROL;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (shift)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.SHIFT;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);

				if (key.ShiftKey != VirtualKeyCode.NULL)
				{
					//Send KEY_DOWN
					if (SendMessage(hWnd, (int) Message.KEY_DOWN, (uint) key.Vk, GetLParam(1, key.ShiftKey, 0, 0, 0, 0)))
						return false;
					Thread.Sleep(delay);
				}
			}

			SendMessage(hWnd, key, false);

			structInput.ki.dwFlags = KEYEVENTF_KEYUP;
			if (shift && key.ShiftKey == VirtualKeyCode.NULL)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.SHIFT;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.CONTROL;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}
			if (alt)
			{
				structInput.ki.wVk = (ushort) VirtualKeyCode.MENU;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
				Thread.Sleep(delay);
			}

			return true;
		}

		public static void CheckKeyShiftState()
		{
			while ((GetKeyState((int) VirtualKeyCode.MENU) & KEY_PRESSED) == KEY_PRESSED ||
			       (GetKeyState((int) VirtualKeyCode.CONTROL) & KEY_PRESSED) == KEY_PRESSED ||
			       (GetKeyState((int) VirtualKeyCode.SHIFT) & KEY_PRESSED) == KEY_PRESSED)
			{
				Thread.Sleep(1);
			}
		}

		#endregion //Public

		#region Private

		private static uint GetScanCode(VirtualKeyCode key)
		{
			return MapVirtualKey((uint) key, MAPVK_VK_TO_VSC_EX);
		}

		private static uint GetDwExtraInfo(Int16 repeatCount, VirtualKeyCode key, byte extended, byte contextCode, byte previousState,
			byte transitionState)
		{
			var lParam = (uint) repeatCount;
			uint scanCode = MapVirtualKey((uint) key, MAPVK_VK_TO_VSC_EX) + 0x80;
			lParam += scanCode*0x10000;
			lParam += (uint) ((extended)*0x1000000);
			lParam += (uint) ((contextCode*2)*0x10000000);
			lParam += (uint) ((previousState*4)*0x10000000);
			lParam += (uint) ((transitionState*8)*0x10000000);
			return lParam;
		}

		private static uint GetLParam(int x, int y)
		{
			return (uint) ((y << 16) | (x & 0xFFFF));
		}

		private static uint GetLParam(Int16 repeatCount, VirtualKeyCode key, byte extended, byte contextCode, byte previousState,
			byte transitionState)
		{
			var lParam = (uint) repeatCount;
			//uint scanCode = MapVirtualKey((uint)key, MAPVK_VK_TO_CHAR);
			uint scanCode = GetScanCode(key);
			lParam += scanCode*0x10000;
			lParam += (uint) ((extended)*0x1000000);
			lParam += (uint) ((contextCode*2)*0x10000000);
			lParam += (uint) ((previousState*4)*0x10000000);
			lParam += (uint) ((transitionState*8)*0x10000000);
			return lParam;
		}

		private static uint RemoveLeadingDigit(uint number)
		{
			return (number - ((number%(0x10000000))*(0x10000000)));
		}

		#endregion Private

		#endregion //Methods
	}
}

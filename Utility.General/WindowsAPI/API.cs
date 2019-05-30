using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Utility.General.WindowsAPI
{
    public class API
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        #region Fields

        private static Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

        #endregion

        #region Methods

        public static Color GetColorAt(int x, int y)
        {
            return GetColorAt(new Point(x, y));
        }

        public static Color GetColorAt(Point location)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }

        public static Point GetCursorPoint()
        {
            Point cursor = new Point();
            GetCursorPos(ref cursor);
            return cursor;
        }

        public static string GetForegroundProcessName()
        {
            IntPtr hwnd = GetForegroundWindow();

            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.Id == pid)
                    return p.ProcessName;
            }

            return "Unknown";
        }

        public static IntPtr GetWindowHandle(string appName)
        {
            IntPtr windowHandle = IntPtr.Zero;
            Process[] processes = Process.GetProcessesByName(appName);

            if (processes.Length > 0)
                windowHandle = processes[0].MainWindowHandle;

            return windowHandle;
        }

        #endregion

    }
}


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
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        #region Fields

        private static readonly Bitmap ScreenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

        #endregion

        #region Methods

        public static Color GetColorAt(int x, int y)
        {
            return GetColorAt(new Point(x, y));
        }

        public static Color GetColorAt(Point location)
        {
            using (var gdest = Graphics.FromImage(ScreenPixel))
            {
                using (var gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    var hSrcDC = gsrc.GetHdc();
                    var hDC = gdest.GetHdc();
                    var retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return ScreenPixel.GetPixel(0, 0);
        }

        public static Point GetCursorPoint()
        {
            Point cursor = new Point();
            GetCursorPos(ref cursor);
            return cursor;
        }

        public static string GetForegroundProcessName()
        {
            var hwnd = GetForegroundWindow();
            GetWindowThreadProcessId(hwnd, out var pid);

            foreach (var p in Process.GetProcesses())
            {
                if (p.Id == pid)
                    return p.ProcessName;
            }

            return "Unknown";
        }

        public static IntPtr GetWindowHandle(string appName)
        {
            var windowHandle = IntPtr.Zero;
            var processes = Process.GetProcessesByName(appName);

            if (processes.Length > 0)
                windowHandle = processes[0].MainWindowHandle;

            return windowHandle;
        }

        #endregion

    }
}


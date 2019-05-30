using System;
using System.Runtime.InteropServices;

namespace Utility.Memory
{

    public static class Memory
    {

        public const byte ASCII_CHAR_LENGTH = 1;


        [DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
        public static extern bool ReadProcessMemory(
           IntPtr hProcess,
           uint dwAddress,
           IntPtr lpBuffer,
           int nSize,
           out int lpBytesRead);


        public static int ReadRawMemory(IntPtr hProcess, uint dwAddress, IntPtr lpBuffer, int nSize)
        {
            int lpBytesRead = 0;

            try
            {
                if (!ReadProcessMemory(hProcess, dwAddress, lpBuffer, nSize, out lpBytesRead))
                    throw new Exception("ReadProcessMemory failed");

                return (int)lpBytesRead;
            }
            catch
            {
                return 0;
            }
        }

        public static byte[] ReadBytes(IntPtr hProcess, uint dwAddress, int nSize)
        {
            IntPtr lpBuffer = IntPtr.Zero;
            int iBytesRead;
            byte[] baRet;

            try
            {
                lpBuffer = Marshal.AllocHGlobal(nSize);

                iBytesRead = ReadRawMemory(hProcess, dwAddress, lpBuffer, nSize);
                if (iBytesRead != nSize)
                    throw new Exception("ReadProcessMemory error in ReadBytes");

                baRet = new byte[iBytesRead];
                Marshal.Copy(lpBuffer, baRet, 0, iBytesRead);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (lpBuffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(lpBuffer);
            }

            return baRet;
        }
        public static uint ReadUInt(IntPtr hProcess, uint dwAddress, bool bReverse)
        {
            byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(uint));
            if (buf == null)
                throw new Exception("ReadUInt failed.");

            if (bReverse)
                Array.Reverse(buf);

            return BitConverter.ToUInt32(buf, 0);
        }

        public static int ReadInt(IntPtr hProcess, uint dwAddress, bool bReverse)
        {
            byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(int));
            if (buf == null)
                throw new Exception("ReadInt failed.");

            if (bReverse)
                Array.Reverse(buf);

            return BitConverter.ToInt32(buf, 0);
        }

        public static float ReadFloat(IntPtr hProcess, uint dwAddress, bool bReverse)
        {
            byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(float));
            if (buf == null)
                throw new Exception("ReadFloat failed.");

            if (bReverse)
                Array.Reverse(buf);

            return BitConverter.ToSingle(buf, 0);
        }

        public static string ReadASCIIString(IntPtr hProcess, uint dwAddress, int nLength)
        {
            IntPtr lpBuffer = IntPtr.Zero;
            int iBytesRead, nSize;
            string sRet;

            try
            {
                nSize = nLength * ASCII_CHAR_LENGTH;
                lpBuffer = Marshal.AllocHGlobal(nSize + ASCII_CHAR_LENGTH);
                Marshal.WriteByte(lpBuffer, nLength, 0);

                iBytesRead = ReadRawMemory(hProcess, dwAddress, lpBuffer, nSize);
                if (iBytesRead != nSize)
                    throw new Exception();

                sRet = Marshal.PtrToStringAnsi(lpBuffer);
            }
            catch
            {
                return String.Empty;
            }
            finally
            {
                if (lpBuffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(lpBuffer);
            }

            return sRet;
        }
    }
}

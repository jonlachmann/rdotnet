using System;
using System.Runtime.InteropServices;

namespace RDotNet.NativeLibrary
{
    public class EnvFunctions
    {
        public static int SetEnvironmentVariable(string var, string value, bool isWindows)
        {
            Environment.SetEnvironmentVariable(var, value);
            return isWindows
                ? WindowsEnvFunctions.SetEnvironmentVariable(var, value)
                : UnixEnvFunctions.SetEnvironmentVariable(var, value);
        }
    }

    public static class WindowsEnvFunctions
    {
        public static int SetEnvironmentVariable(string var, string value)
        {
            return _putenv(var + "=" + value);
        }

        /// <summary>
        /// Used for setting env variables which will be visible to R on Windows
        /// </summary>
        /// <param name="envVar">The variable to set, on the format VAR=VALUE</param>
        /// <returns></returns>
        [DllImport( "ucrtbase.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _putenv(string envVar);
    }

    public static class UnixEnvFunctions
    {
        public static int SetEnvironmentVariable(string var, string value)
        {
            return setenv(var, value, 1);
        }

        // Import setenv from libc (part of standard C library)
        [DllImport("libc", SetLastError = true)]
        public static extern int setenv(string name, string value, int overwrite);
    }
}

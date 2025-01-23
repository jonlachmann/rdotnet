using System.Runtime.InteropServices;

namespace RDotNet.NativeLibrary
{
    public class WindowsEnvFunctions
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
}

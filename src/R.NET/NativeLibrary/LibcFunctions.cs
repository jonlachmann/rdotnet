using System.Runtime.InteropServices;

namespace RDotNet.NativeLibrary
{
    public class LibcFunctions
    {
        // Import setenv from libc (part of standard C library)
        [DllImport("libc", SetLastError = true)]
        public static extern int setenv(string name, string value, int overwrite);
    }
}

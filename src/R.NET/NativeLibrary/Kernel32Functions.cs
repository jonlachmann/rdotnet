using System.Runtime.InteropServices;

namespace RDotNet.NativeLibrary
{
    public class Kernel32Functions
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetEnvironmentVariable(string name, string value);
    }
}

using System.Runtime.InteropServices;

namespace RDotNet.Internals.ALTREP
{
    // Definition of the struct available at: https://cran.r-project.org/doc/manuals/r-release/R-ints.html#Rest-of-header
    // Formally defined in Rinternals.h: https://github.com/wch/r-source/blob/trunk/src/include/Rinternals.h
    // Note that this structure was greatly changed in the R 3.5 release, using the platform-dependent pointer size (represented
    //   here as IntPtr), with fields added and the order changed.
    [StructLayout(LayoutKind.Sequential)]
    internal struct sxpinfo
    {
        private ulong bits;

        private const int NAMED_BITS = 16;

        public SymbolicExpressionType type => (SymbolicExpressionType)(bits & 31u); // 5 bits

        public uint scalar => (uint)((bits & 32u) / 32); // 1 bit

        public uint obj => (uint)((bits & 64u) / 64); // 1 bit

        public uint alt => (uint)((bits & 128u) / 128); // 1 bit

        public uint gp => (uint)((bits & 16776960u) / 256); // 16 bits

        public uint mark => (uint)((bits & 16777216u) / 16777216); // 1 bit

        public uint debug => (uint)((bits & 33554432u) / 33554432); // 1 bit

        public uint trace => (uint)((bits & 67108864u) / 67108864); // 1 bit

        public uint spare => (uint)((bits & 134217728u) / 134217728); // 1 bit

        public uint gcgen => (uint)((bits & 268435456u) / 268435456); // 1 bit

        public uint gccls => (uint)((bits & 3758096384u) / 536870912); // 3 bits

        public uint named => (uint)((bits & 281470681743360u) / 4294967296); // NAMED_BITS

        public uint extra => (uint)((bits & 18446462598732800000u) / 281474976710656); // 32 - NAMED_BITS
    }
}

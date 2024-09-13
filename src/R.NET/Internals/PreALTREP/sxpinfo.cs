using System.Runtime.InteropServices;

namespace RDotNet.Internals.PreALTREP
{
    // Definition of the struct available at: https://cran.r-project.org/doc/manuals/r-release/R-ints.html#Rest-of-header
    // Formally defined in Rinternals.h: https://github.com/wch/r-source/blob/trunk/src/include/Rinternals.h
    // Note that this structure was greatly changed in the R 3.5 release, using the platform-dependent pointer size (represented
    //   here as IntPtr), with fields added and the order changed.
    [StructLayout(LayoutKind.Sequential)]
    internal struct sxpinfo
    {
        private uint bits;

        public SymbolicExpressionType type => (SymbolicExpressionType)(bits & 31u);

        public uint obj => ((bits & 32u) / 32);

        public uint named => ((bits & 192u) / 64);

        public uint gp => ((bits & 16776960u) / 256);

        public uint mark => ((bits & 16777216u) / 16777216);

        public uint debug => ((bits & 33554432u) / 33554432);

        public uint trace => ((bits & 67108864u) / 67108864);

        public uint spare => ((bits & 134217728u) / 134217728);

        public uint gcgen => ((bits & 268435456u) / 268435456);

        public uint gccls => ((bits & 3758096384u) / 536870912);
    }
}

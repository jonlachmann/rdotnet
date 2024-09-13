using System;
using System.Runtime.InteropServices;

namespace RDotNet.Internals.ALTREP
{
    // In R 3.5, the length & true length values went from pure 32-bit int to platform-dependent pointer length (32 or 64 bits in length).
    // These are defined in R as R_xlen_t (previously R_len_t) - https://github.com/wch/r-source/blob/trunk/src/include/Rinternals.h
    // Here we use the .NET equivalent - IntPtr.

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct VECTOR_SEXPREC
    {
        private SEXPREC_HEADER header;
        private vecsxp vecsxp;

        public sxpinfo sxpinfo => header.sxpinfo;

        public IntPtr attrib => header.attrib;

        public IntPtr gengc_next_node => header.gengc_next_node;

        public IntPtr gengc_prev_node => header.gengc_prev_node;

        public IntPtr Length => vecsxp.length;

        public IntPtr TrueLength => vecsxp.truelength;
    }
}

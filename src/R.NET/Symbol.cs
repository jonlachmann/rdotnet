using RDotNet.Internals;
using RDotNet.Utilities;
using System;
using System.Runtime.InteropServices;

namespace RDotNet
{
    /// <summary>
    /// A symbol object.
    /// </summary>
    public class Symbol : SymbolicExpression
    {
        /// <summary>
        /// Creates a symbol.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="pointer">The pointer.</param>
        protected internal Symbol(REngine engine, IntPtr pointer)
            : base(engine, pointer)
        { }

        /// <summary>
        /// Gets and sets the name.
        /// </summary>
        public string PrintName
        {
            get
            {
                dynamic sexp = GetInternalStructure();
                return new InternalString(Engine, sexp.symsxp.pname).ToString();
            }
            set
            {
                var pointer = (value == null ? Engine.NilValue : new InternalString(Engine, value)).DangerousGetHandle();
                var offset = GetOffsetOf("pname");
                Marshal.WriteIntPtr(handle, offset, pointer);
            }
        }

        /// <summary>
        /// Gets the internal function.
        /// </summary>
        public SymbolicExpression Internal
        {
            get
            {
                dynamic sexp = GetInternalStructure();
                return Engine.EqualsRNilValue((IntPtr)sexp.symsxp.value) ? null : new SymbolicExpression(Engine, sexp.symsxp.@internal);
            }
        }

        /// <summary>
        /// Gets the symbol value.
        /// </summary>
        public SymbolicExpression Value
        {
            get
            {
                dynamic sexp = GetInternalStructure();
                return Engine.EqualsRNilValue((IntPtr)sexp.symsxp.value) ? null : new SymbolicExpression(Engine, sexp.symsxp.value);
            }
        }

        private int GetOffsetOf(string fieldName)
        {
            return Marshal.OffsetOf(Engine.GetSEXPRECType(), "u").ToInt32() + Marshal.OffsetOf(Engine.GetSymSxpType(), fieldName).ToInt32();
        }
    }
}

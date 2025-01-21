using RDotNet.Internals;
using System;

namespace RDotNet
{
    internal class ProtectedPointer : IDisposable
    {
        private readonly REngine _engine;

        protected TDelegate GetFunction<TDelegate>() where TDelegate : class
        {
            return _engine.GetFunction<TDelegate>();
        }

        private readonly IntPtr _sexp;

        public ProtectedPointer(REngine engine, IntPtr sexp)
        {
            _sexp = sexp;
            _engine = engine;

            GetFunction<Rf_protect>()(_sexp);
        }

        public ProtectedPointer(SymbolicExpression sexp)
        {
            _sexp = sexp.DangerousGetHandle();
            _engine = sexp.Engine;

            GetFunction<Rf_protect>()(_sexp);
        }

        #region IDisposable Members

        public void Dispose()
        {
            GetFunction<Rf_unprotect_ptr>()(_sexp);
        }

        #endregion IDisposable Members

        public static implicit operator IntPtr(ProtectedPointer p)
        {
            return p._sexp;
        }
    }
}

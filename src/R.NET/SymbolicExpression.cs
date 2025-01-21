using RDotNet.Dynamic;
using RDotNet.Internals;
using RDotNet.Utilities;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace RDotNet
{
    /// <summary>
    /// An expression in R environment.
    /// </summary>
    [DebuggerDisplay("RObjectType = {Type}")]
    public class SymbolicExpression : SafeHandle, IEquatable<SymbolicExpression>  //, IDynamicMetaObjectProvider
    {
        private readonly REngine engine;
        private readonly dynamic sexp;

        /// <summary>
        /// An object to use to get a lock on if EnableLock is true;
        /// </summary>
        /// <remarks>
        /// Following recommended practices in http://msdn.microsoft.com/en-us/library/vstudio/c5kehkcz(v=vs.120).aspx
        /// </remarks>
        private static readonly Object lockObject = new Object();

        private bool isProtected;

        /// <summary>
        /// Creates new instance of SymbolicExpression.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="pointer">The pointer.</param>
        protected internal SymbolicExpression(REngine engine, IntPtr pointer)
            : base(IntPtr.Zero, true)
        {
            this.engine = engine;
            var sexprecType = engine.GetSEXPRECType();
            sexp = Convert.ChangeType(Marshal.PtrToStructure(pointer,  sexprecType), sexprecType);
            SetHandle(pointer);
            Preserve();
        }

        /// <summary>
        /// Is the handle of this SEXP invalid (zero, i.e. null pointer)
        /// </summary>
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <summary>
        /// Gets the <see cref="REngine"/> to which this expression belongs.
        /// </summary>
        public REngine Engine => engine;

        /// <summary>
        /// Creates the delegate function for the specified function defined in the DLL.
        /// </summary>
        /// <typeparam name="TDelegate">The type of delegate.</typeparam>
        /// <returns>The delegate.</returns>
        protected internal TDelegate GetFunction<TDelegate>() where TDelegate : class
        {
            return Engine.GetFunction<TDelegate>();
        }

        /// <summary>
        /// Gets whether this expression is protected from the garbage collection.
        /// </summary>
        public bool IsProtected => isProtected;

        /// <summary>
        /// Gets the <see cref="SymbolicExpressionType"/>.
        /// </summary>
        public SymbolicExpressionType Type => sexp.sxpinfo.type;

        #region IDynamicMetaObjectProvider Members

        /// <summary>
        /// returns a new SymbolicExpressionDynamicMeta for this SEXP
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            return new SymbolicExpressionDynamicMeta(parameter, this);
        }

        #endregion IDynamicMetaObjectProvider Members

        #region IEquatable<SymbolicExpression> Members

        /// <summary>
        /// Testing the equality of SEXP, based on handle equality.
        /// </summary>
        /// <param name="other">other SEXP</param>
        /// <returns>True if the objects have a handle that is the same, i.e. pointing to the same address in unmanaged memory</returns>
        public bool Equals(SymbolicExpression other)
        {
            return other != null && handle == other.handle;
        }

        #endregion IEquatable<SymbolicExpression> Members

        internal object GetInternalStructure()
        {
            var sexprecType = engine.GetSEXPRECType();
            return Convert.ChangeType(Marshal.PtrToStructure(handle, sexprecType), sexprecType);
        }

        /// <summary>
        /// Gets all value names.
        /// </summary>
        /// <returns>The names of attributes</returns>
        public string[] GetAttributeNames()
        {
            int length = GetFunction<Rf_length>()(sexp.attrib);
            var names = new string[length];
            IntPtr pointer = sexp.attrib;
            var sexprecType = engine.GetSEXPRECType();
            for (var index = 0; index < length; index++)
            {
                dynamic node = Convert.ChangeType(Marshal.PtrToStructure(pointer, sexprecType), sexprecType);
                var attribute = Convert.ChangeType(Marshal.PtrToStructure(node.listsxp.tagval, sexprecType), sexprecType);
                IntPtr name = attribute.symsxp.pname;
                names[index] = new InternalString(Engine, name);
                pointer = node.listsxp.cdrval;
            }
            return names;
        }

        /// <summary>
        /// Gets the value of the specified name.
        /// </summary>
        /// <param name="attributeName">The name of attribute.</param>
        /// <returns>The attribute.</returns>
        public SymbolicExpression GetAttribute(string attributeName)
        {
            ArgumentNullException.ThrowIfNull(attributeName);
            if (attributeName == string.Empty)
            {
                throw new ArgumentException();
            }

            var installedName = GetFunction<Rf_install>()(attributeName);
            var attribute = GetFunction<Rf_getAttrib>()(handle, installedName);
            return Engine.EqualsRNilValue(attribute) ? null : new SymbolicExpression(Engine, attribute);
        }

        internal SymbolicExpression GetAttribute(SymbolicExpression symbol)
        {
            ArgumentNullException.ThrowIfNull(symbol);
            if (symbol.Type != SymbolicExpressionType.Symbol)
            {
                throw new ArgumentException();
            }

            var attribute = GetFunction<Rf_getAttrib>()(handle, symbol.handle);
            return Engine.EqualsRNilValue(attribute) ? null : new SymbolicExpression(Engine, attribute);
        }

        /// <summary>
        /// Sets the new value to the attribute of the specified name.
        /// </summary>
        /// <param name="attributeName">The name of attribute.</param>
        /// <param name="value">The value</param>
        public void SetAttribute(string attributeName, SymbolicExpression value)
        {
            ArgumentNullException.ThrowIfNull(attributeName);
            if (attributeName == string.Empty)
            {
                throw new ArgumentException();
            }

            value ??= Engine.NilValue;

            var installedName = GetFunction<Rf_install>()(attributeName);
            GetFunction<Rf_setAttrib>()(handle, installedName, value.handle);
        }

        internal void SetAttribute(SymbolicExpression symbol, SymbolicExpression value)
        {
            ArgumentNullException.ThrowIfNull(symbol);
            if (symbol.Type != SymbolicExpressionType.Symbol)
            {
                throw new ArgumentException();
            }

            value ??= Engine.NilValue;

            GetFunction<Rf_setAttrib>()(handle, symbol.handle, value.handle);
        }

        /// <summary>
        /// Protects the expression from R's garbage collector.
        /// </summary>
        /// <seealso cref="Unpreserve"/>
        public void Preserve()
        {
            if (IsInvalid || isProtected) return;
            if (Engine.EnableLock)
            {
                lock (lockObject) { GetFunction<R_PreserveObject>()(handle); }
            }
            else
                GetFunction<R_PreserveObject>()(handle);
            isProtected = true;
        }

        /// <summary>
        /// Stops protection.
        /// </summary>
        /// <seealso cref="Preserve"/>
        public void Unpreserve()
        {
            if (IsInvalid || !IsProtected) return;
            if (Engine.EnableLock)
            {
                lock (lockObject) { GetFunction<R_ReleaseObject>()(handle); ; }
            }
            else
                GetFunction<R_ReleaseObject>()(handle);
            isProtected = false;
        }

        /// <summary>
        /// Release the handle on the symbolic expression, i.e. tells R to decrement the reference count to the expression in unmanaged memory
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            if (IsProtected)
            {
                Unpreserve();
            }
            return true;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return handle.GetHashCode();
        }

        /// <summary>
        /// Test the equality of this object with another. If this object is also a SymbolicExpression and points to the same R expression, returns true.
        /// </summary>
        /// <param name="obj">Other object to test for equality</param>
        /// <returns>Returns true if pointing to the same R expression in memory.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SymbolicExpression);
        }

        /// <summary>
        /// Experimental
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="sexp"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SymbolicExpression op_Dynamic<K>(SymbolicExpression sexp, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Experimental
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="sexp"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void op_DynamicAssignment<K>(SymbolicExpression sexp, string name, dynamic value)
        {
        }
    }
}

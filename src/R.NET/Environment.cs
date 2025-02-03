using RDotNet.Internals;
using RDotNet.Utilities;
using System;
using System.Runtime.InteropServices;

namespace RDotNet
{
    /// <summary>
    /// An environment object.
    /// </summary>
    public class REnvironment : SymbolicExpression
    {
        /// <summary>
        /// Creates an environment object.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="pointer">The pointer to an environment.</param>
        protected internal REnvironment(REngine engine, IntPtr pointer)
            : base(engine, pointer)
        { }

        /// <summary>
        /// Creates a new environment object.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="parent">The parent environment.</param>
        public REnvironment(REngine engine, REnvironment parent)
            : base(engine, engine.GetFunction<Rf_NewEnvironment>()(engine.NilValue.DangerousGetHandle(), engine.NilValue.DangerousGetHandle(), parent.handle))
        { }

        /// <summary>
        /// Gets the parental environment.
        /// </summary>
        public REnvironment Parent
        {
            get
            {
                dynamic sexp = GetInternalStructure();
                IntPtr parent = sexp.envsxp.enclos;
                return Engine.EqualsRNilValue(parent) ? null : new REnvironment(Engine, parent);
            }
        }

        /// <summary>
        /// Gets a symbol defined in this environment.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The symbol.</returns>
        public SymbolicExpression GetSymbol(string name)
        {
            CheckNameArg(name);

            var installedName = GetFunction<Rf_install>()(name);
            var value = GetFunction<Rf_findVar>()(installedName, handle);
            if (Engine.CheckUnbound(value))
            {
                throw new EvaluationException($"Error: object '{name}' not found");
            }

            return GetSymbol(value);
        }

        /// <summary>
        /// Tries to get a symbol defined in this environment.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="symbol">The symbol if it was found.</param>
        /// <returns>A boolean denoting if the symbol was found or not.</returns>
        public bool TryGetSymbol(string name, out SymbolicExpression symbol)
        {
            CheckNameArg(name);

            var installedName = GetFunction<Rf_install>()(name);
            var value = GetFunction<Rf_findVar>()(installedName, handle);
            if (Engine.CheckUnbound(value))
            {
                symbol = null;
                return false;
            }
            symbol = GetSymbol(value);
            return true;
        }

        /// <summary>
        /// Gets a symbol defined in this environment from its IntPtr
        /// </summary>
        /// <param name="value">The int pointer to the symbol.</param>
        /// <returns>The symbol.</returns>
        private SymbolicExpression GetSymbol(IntPtr value)
        {
            var sexprecType = Engine.GetSEXPRECType();
            dynamic sexp = Convert.ChangeType(Marshal.PtrToStructure(value, sexprecType), sexprecType);
            if (sexp.sxpinfo.type == SymbolicExpressionType.Promise)
            {
                value = GetFunction<Rf_eval>()(value, handle);
            }
            return new SymbolicExpression(Engine, value);
        }

        /// <summary>
        /// Defines a symbol in this environment.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expression">The symbol.</param>
        public void SetSymbol(string name, SymbolicExpression expression)
        {
            CheckNameArg(name);

            expression ??= Engine.NilValue;
            if (expression.Engine != Engine)
            {
                throw new ArgumentException();
            }
            var installedName = GetFunction<Rf_install>()(name);
            GetFunction<Rf_defineVar>()(installedName, expression.DangerousGetHandle(), handle);
        }

        /// <summary>
        /// Gets the symbol names defined in this environment.
        /// </summary>
        /// <param name="all">Including special functions or not.</param>
        /// <returns>Symbol names.</returns>
        public string[] GetSymbolNames(bool all = false)
        {
            var symbolNames = new CharacterVector(Engine, GetFunction<R_lsInternal>()(handle, all));
            var length = symbolNames.Length;
            var copy = new string[length];
            symbolNames.CopyTo(copy, length);
            return copy;
        }

        private static void CheckNameArg(string name)
        {
            switch (name)
            {
                case null:
                    throw new ArgumentNullException(nameof(name), "'name' cannot be null");
                case "":
                    throw new ArgumentException("'name' cannot be an empty string");
            }
        }
    }
}

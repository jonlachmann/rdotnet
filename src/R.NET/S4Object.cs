using RDotNet.Diagnostics;
using RDotNet.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RDotNet
{
    /// <summary>
    /// An S4 object
    /// </summary>
    [DebuggerDisplay("SlotCount = {SlotCount}; RObjectType = {Type}")]
    [DebuggerTypeProxy(typeof(S4ObjectDebugView))]
    public class S4Object : SymbolicExpression
    {
        /// <summary>
        /// Function .slotNames
        /// </summary>
        /// <remarks>
        /// slotNames, when used on the class representation object, returns the slot names of
        /// instances of the class, rather than the slot names of the class object itself. '.slotNames' is what we want.
        /// </remarks>
        private static Function _dotSlotNamesFunc;

        /// <summary>
        /// Create a new S4 object
        /// </summary>
        /// <param name="engine">R engine</param>
        /// <param name="pointer">pointer to native S4 SEXP</param>
        protected internal S4Object(REngine engine, IntPtr pointer)
            : base(engine, pointer)
        {
            _dotSlotNamesFunc ??= Engine.Evaluate("invisible(.slotNames)").AsFunction();
        }

        /// <summary>
        /// Gets/sets the value of a slot
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SymbolicExpression this[string name]
        {
            get
            {
                CheckSlotName(name);
                IntPtr slotValue;
                using (var s = new ProtectedPointer(Engine, GetFunction<Rf_mkString>()(InternalString.NativeUtf8FromString(name))))
                {
                    slotValue = GetFunction<R_do_slot>()(DangerousGetHandle(), s);
                }
                return new SymbolicExpression(Engine, slotValue);
            }
            set
            {
                CheckSlotName(name);
                using var s = new ProtectedPointer(Engine, GetFunction<Rf_mkString>()(InternalString.NativeUtf8FromString(name)));
                using (new ProtectedPointer(this))
                {
                    GetFunction<R_do_slot_assign>()(DangerousGetHandle(), s, value.DangerousGetHandle());
                }
            }
        }

        private void CheckSlotName(string name)
        {
            if (!SlotNames.Contains(name))
                throw new ArgumentException($"Invalid slot name '{name}'", nameof(name));
        }

        /// <summary>
        /// Is a slot name valid.
        /// </summary>
        /// <param name="slotName">the name of the slot</param>
        /// <returns>whether a slot name is present in the object</returns>
        public bool HasSlot(string slotName)
        {
            using var s = new ProtectedPointer(Engine, GetFunction<Rf_mkString>()(InternalString.NativeUtf8FromString(slotName)));
            return GetFunction<R_has_slot>()(DangerousGetHandle(), s);
        }

        private string[] _slotNames;

        /// <summary>
        /// Gets the slot names for this object. The values are cached once retrieved the first time.
        /// Note this is equivalent to the function '.slotNames' in R, not 'slotNames'
        /// </summary>
        public string[] SlotNames
        {
            get
            {
                _slotNames ??= _dotSlotNamesFunc.Invoke(this).AsCharacter().ToArray();
                return (string[])_slotNames.Clone();
            }
        }

        /// <summary>
        /// Gets the number of slot names
        /// </summary>
        public int SlotCount => SlotNames.Length;

        /// <summary>
        /// Gets the class representation.
        /// </summary>
        /// <returns>The class representation of the S4 class.</returns>
        public S4Object GetClassDefinition()
        {
            var classSymbol = Engine.GetPredefinedSymbol("R_ClassSymbol");
            var className = GetAttribute(classSymbol).AsCharacter().First();
            var definition = Engine.GetFunction<R_getClassDef>()(className);
            return new S4Object(Engine, definition);
        }

        /// <summary>
        /// Gets slot names and types.
        /// </summary>
        /// <returns>Slot names.</returns>
        public IDictionary<string, string> GetSlotTypes()
        {
            var definition = GetClassDefinition();
            var slots = definition["slots"];
            var namesSymbol = Engine.GetPredefinedSymbol("R_NamesSymbol");
            return slots.GetAttribute(namesSymbol).AsCharacter()
               .Zip(slots.AsCharacter(), (name, type) => new { Name = name, Type = type })
               .ToDictionary(t => t.Name, t => t.Type);
        }
    }
}

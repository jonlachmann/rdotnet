using System.Diagnostics;
using System.Linq;

namespace RDotNet.Diagnostics
{
    [DebuggerDisplay("{Display,nq}")]
    internal class S4ObjectSlotDisplay
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly S4Object s4obj;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string name;

        public S4ObjectSlotDisplay(S4Object obj, string name)
        {
            s4obj = obj;
            this.name = name;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public SymbolicExpression Value => s4obj[name];

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Display
        {
            get
            {
                var slot = Value;
                var names = s4obj.SlotNames;
                if (names == null || !names.Contains(name))
                {
                    return $"NA ({slot.Type})";
                }
                else
                {
                    return $"\"{name}\" ({slot.Type})";
                }
            }
        }
    }
}

using System.Diagnostics;
using System.Linq;

namespace RDotNet.Diagnostics
{
    internal class S4ObjectDebugView
    {
        private readonly S4Object s4obj;

        public S4ObjectDebugView(S4Object obj)
        {
            s4obj = obj;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public S4ObjectSlotDisplay[] Slots
        {
            get
            {
                return s4obj.SlotNames.AsEnumerable()
                   .Select(name => new S4ObjectSlotDisplay(s4obj, name))
                   .ToArray();
            }
        }
    }
}

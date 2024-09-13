using System.Diagnostics;

namespace RDotNet.Diagnostics
{
    internal class VectorDebugView<T>
    {
        private readonly Vector<T> _vector;

        public VectorDebugView(Vector<T> vector)
        {
            _vector = vector;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Value
        {
            get
            {
                var array = new T[_vector.Length];
                _vector.CopyTo(array, array.Length);
                return array;
            }
        }
    }
}

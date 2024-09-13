using System.Diagnostics;

namespace RDotNet.Diagnostics
{
    internal class MatrixDebugView<T>
    {
        private readonly Matrix<T> _matrix;

        public MatrixDebugView(Matrix<T> matrix)
        {
            _matrix = matrix;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[,] Value
        {
            get
            {
                var array = new T[_matrix.RowCount, _matrix.ColumnCount];
                _matrix.CopyTo(array, _matrix.RowCount, _matrix.ColumnCount);
                return array;
            }
        }
    }
}

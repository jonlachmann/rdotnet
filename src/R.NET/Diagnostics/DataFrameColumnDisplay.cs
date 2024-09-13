using System;
using System.Diagnostics;

namespace RDotNet.Diagnostics
{
    [DebuggerDisplay("{Display,nq}")]
    internal class DataFrameColumnDisplay
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DataFrame data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int columnIndex;

        public DataFrameColumnDisplay(DataFrame data, int columnIndex)
        {
            this.data = data;
            this.columnIndex = columnIndex;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Value
        {
            get
            {
                var column = data[columnIndex];
                return column.IsFactor() ? column.AsFactor().GetFactors() : column.ToArray();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Display
        {
            get
            {
                var column = data[columnIndex];
                var names = data.ColumnNames;
                return names?[columnIndex] == null ? $"NA ({column.Type})" : $"\"{names[columnIndex]}\" ({column.Type})";
            }
        }
    }
}

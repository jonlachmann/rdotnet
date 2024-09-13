using RDotNet.Diagnostics;
using RDotNet.Internals;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// A matrix base.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    [DebuggerDisplay("MatrixSize = {RowCount} x {ColumnCount}; RObjectType = {Type}")]
    [DebuggerTypeProxy(typeof(MatrixDebugView<>))]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public abstract class Matrix<T> : SymbolicExpression
    {
        /// <summary>
        /// Creates a new matrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="type">The element type.</param>
        /// <param name="rowCount">The size of row.</param>
        /// <param name="columnCount">The size of column.</param>
        protected Matrix(REngine engine, SymbolicExpressionType type, int rowCount, int columnCount)
            : base(engine, engine.GetFunction<Rf_allocMatrix>()(type, rowCount, columnCount))
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowCount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columnCount);
            var empty = new byte[rowCount * columnCount * DataSize];
            Marshal.Copy(empty, 0, DataPointer, empty.Length);
        }

        /// <summary>
        /// Creates a new matrix with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="type">The element type.</param>
        /// <param name="matrix">The values.</param>
        public Matrix(REngine engine, SymbolicExpressionType type, T[,] matrix)
            : base(engine, engine.GetFunction<Rf_allocMatrix>()(type, matrix.GetLength(0), matrix.GetLength(1)))
        {
            //InitMatrixWithIndexers(matrix, rowCount, columnCount);
            InitMatrixFast(matrix);
        }

        private void InitMatrixFast(T[,] matrix)
        {
            using (new ProtectedPointer(this))
            {
                InitMatrixFastDirect(matrix);
            }
        }

        /// <summary>
        /// Initializes this R matrix, using the values in a rectangular array.
        /// </summary>
        /// <param name="matrix"></param>
        protected abstract void InitMatrixFastDirect(T[,] matrix);

        /// <summary>
        /// Creates a new instance for a matrix.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="coerced">The pointer to a matrix.</param>
        protected Matrix(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="rowIndex">The zero-based row index of the element to get or set.</param>
        /// <param name="columnIndex">The zero-based column index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public abstract T this[int rowIndex, int columnIndex] { get; set; }

        /// <summary>
        /// Gets or sets the element at the specified names.
        /// </summary>
        /// <param name="rowName">The row name of the element to get or set.</param>
        /// <param name="columnName">The column name of the element to get or set.</param>
        /// <returns>The element at the specified names.</returns>
        public virtual T this[string rowName, string columnName]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(rowName);
                ArgumentNullException.ThrowIfNull(columnName);
                var rowNames = RowNames;
                if (rowNames == null)
                {
                    throw new InvalidOperationException();
                }
                var columnNames = ColumnNames;
                if (columnNames == null)
                {
                    throw new InvalidOperationException();
                }
                var rowIndex = Array.IndexOf(rowNames, rowName);
                var columnIndex = Array.IndexOf(columnNames, columnName);
                return this[rowIndex, columnIndex];
            }
            set
            {
                ArgumentNullException.ThrowIfNull(rowName);
                ArgumentNullException.ThrowIfNull(columnName);
                var rowNames = RowNames;
                if (rowNames == null)
                {
                    throw new InvalidOperationException();
                }
                var columnNames = ColumnNames;
                if (columnNames == null)
                {
                    throw new InvalidOperationException();
                }
                var rowIndex = Array.IndexOf(rowNames, rowName);
                var columnIndex = Array.IndexOf(columnNames, columnName);
                this[rowIndex, columnIndex] = value;
            }
        }

        /// <summary>
        /// Gets the row size of elements.
        /// </summary>
        public int RowCount => GetFunction<Rf_nrows>()(handle);

        /// <summary>
        /// Gets the column size of elements.
        /// </summary>
        public int ColumnCount => GetFunction<Rf_ncols>()(handle);

        /// <summary>
        /// Gets the total number of items (rows times columns) in this matrix
        /// </summary>
        public int ItemCount => RowCount * ColumnCount;

        /// <summary>
        /// Gets the names of rows.
        /// </summary>
        public string[] RowNames
        {
            get
            {
                var dimnamesSymbol = Engine.GetPredefinedSymbol("R_DimNamesSymbol");
                var dimnames = GetAttribute(dimnamesSymbol);
                var rowNames = dimnames?.AsList()[0].AsCharacter();
                if (rowNames == null)
                {
                    return null;
                }

                var length = rowNames.Length;
                var result = new string[length];
                rowNames.CopyTo(result, length);
                return result;
            }
        }

        /// <summary>
        /// Gets the names of columns.
        /// </summary>
        public string[] ColumnNames
        {
            get
            {
                var dimnamesSymbol = Engine.GetPredefinedSymbol("R_DimNamesSymbol");
                var dimnames = GetAttribute(dimnamesSymbol);
                var columnNames = dimnames?.AsList()[1].AsCharacter();
                if (columnNames == null)
                {
                    return null;
                }

                var length = columnNames.Length;
                var result = new string[length];
                columnNames.CopyTo(result, length);
                return result;
            }
        }

        /// <summary>
        /// Gets the pointer for the first element.
        /// </summary>
        protected IntPtr DataPointer
        {
            get
            {
                return Engine.Compatibility switch
                {
                    REngine.CompatibilityMode.ALTREP => GetFunction<DATAPTR_OR_NULL>()(DangerousGetHandle()),
                    REngine.CompatibilityMode.PreALTREP => IntPtr.Add(handle,
                        Marshal.SizeOf(typeof(Internals.PreALTREP.VECTOR_SEXPREC))),
                    _ => throw new MemberAccessException(
                        "Unable to translate the DataPointer for this R compatibility mode")
                };
            }
        }

        /// <summary>
        /// Gets the size of an element in byte.
        /// </summary>
        protected abstract int DataSize { get; }

        /// <summary>
        /// Gets the offset for the specified indexes.
        /// </summary>
        /// <param name="rowIndex">The index of row.</param>
        /// <param name="columnIndex">The index of column.</param>
        /// <returns>The offset.</returns>
        protected int GetOffset(int rowIndex, int columnIndex)
        {
            return DataSize * (columnIndex * RowCount + rowIndex);
        }

        /// <summary>
        /// Copies the elements to the specified array.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="rowCount">The row length to copy.</param>
        /// <param name="columnCount">The column length to copy.</param>
        /// <param name="sourceRowIndex">The first row index of the matrix.</param>
        /// <param name="sourceColumnIndex">The first column index of the matrix.</param>
        /// <param name="destinationRowIndex">The first row index of the destination array.</param>
        /// <param name="destinationColumnIndex">The first column index of the destination array.</param>
        public void CopyTo(T[,] destination, int rowCount, int columnCount, int sourceRowIndex = 0, int sourceColumnIndex = 0, int destinationRowIndex = 0, int destinationColumnIndex = 0)
        {
            ArgumentNullException.ThrowIfNull(destination);
            if (rowCount < 0)
            {
                throw new IndexOutOfRangeException("rowCount");
            }
            if (columnCount < 0)
            {
                throw new IndexOutOfRangeException("columnCount");
            }
            if (sourceRowIndex < 0 || RowCount < sourceRowIndex + rowCount)
            {
                throw new IndexOutOfRangeException("sourceRowIndex");
            }
            if (sourceColumnIndex < 0 || ColumnCount < sourceColumnIndex + columnCount)
            {
                throw new IndexOutOfRangeException("sourceColumnIndex");
            }
            if (destinationRowIndex < 0 || destination.GetLength(0) < destinationRowIndex + rowCount)
            {
                throw new IndexOutOfRangeException("destinationRowIndex");
            }
            if (destinationColumnIndex < 0 || destination.GetLength(1) < destinationColumnIndex + columnCount)
            {
                throw new IndexOutOfRangeException("destinationColumnIndex");
            }

            while (--rowCount >= 0)
            {
                var currentSourceRowIndex = sourceRowIndex++;
                var currentDestinationRowIndex = destinationRowIndex++;
                var currentColumnCount = columnCount;
                var currentSourceColumnIndex = sourceColumnIndex;
                var currentDestinationColumnIndex = destinationColumnIndex;
                while (--currentColumnCount >= 0)
                {
                    destination[currentDestinationRowIndex, currentDestinationColumnIndex++] = this[currentSourceRowIndex, currentSourceColumnIndex++];
                }
            }
        }

        /// <summary>
        /// Gets a .NET representation as a two dimensional array of an R matrix
        /// </summary>
        /// <returns></returns>
        public T[,] ToArray()
        {
            using var p = new ProtectedPointer(this);
            return GetArrayFast();
        }

        /// <summary>
        /// Efficient conversion from R matrix representation to the array equivalent in the CLR
        /// </summary>
        /// <returns>Array equivalent</returns>
        protected abstract T[,] GetArrayFast();
    }
}

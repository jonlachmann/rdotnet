using RDotNet.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RDotNet
{
    /// <summary>
    /// Provides extension methods for <see cref="REngine"/>.
    /// </summary>
    public static class REngineExtension
    {
        /// <summary>
        /// Creates a new empty CharacterVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static CharacterVector CreateCharacterVector(this REngine engine, int length)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new CharacterVector(engine, length);
        }

        /// <summary>
        /// Creates a new empty ComplexVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static ComplexVector CreateComplexVector(this REngine engine, int length)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new ComplexVector(engine, length);
        }

        /// <summary>
        /// Creates a new empty IntegerVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static IntegerVector CreateIntegerVector(this REngine engine, int length)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new IntegerVector(engine, length);
        }

        /// <summary>
        /// Creates a new empty LogicalVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static LogicalVector CreateLogicalVector(this REngine engine, int length)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new LogicalVector(engine, length);
        }

        /// <summary>
        /// Creates a new empty NumericVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static NumericVector CreateNumericVector(this REngine engine, int length)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new NumericVector(engine, length);
        }

        /// <summary>
        /// Creates a new empty RawVector with the specified length.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new vector.</returns>
        public static RawVector CreateRawVector(this REngine engine, int length)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new RawVector(engine, length);
        }

        /// <summary>
        /// Creates a new CharacterVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static CharacterVector CreateCharacterVector(this REngine engine, IEnumerable<string> vector)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new CharacterVector(engine, vector);
        }

        /// <summary>
        /// Creates a new ComplexVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static ComplexVector CreateComplexVector(this REngine engine, IEnumerable<Complex> vector)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new ComplexVector(engine, vector);
        }

        /// <summary>
        /// Creates a new IntegerVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static IntegerVector CreateIntegerVector(this REngine engine, IEnumerable<int> vector)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new IntegerVector(engine, vector);
        }

        /// <summary>
        /// Creates a new LogicalVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static LogicalVector CreateLogicalVector(this REngine engine, IEnumerable<bool> vector)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new LogicalVector(engine, vector);
        }

        /// <summary>
        /// Creates a new NumericVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static NumericVector CreateNumericVector(this REngine engine, IEnumerable<double> vector)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new NumericVector(engine, vector);
        }

        /// <summary>
        /// Creates a new RawVector with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="vector">The values.</param>
        /// <returns>The new vector.</returns>
        public static RawVector CreateRawVector(this REngine engine, IEnumerable<byte> vector)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new RawVector(engine, vector);
        }

        /// <summary>
        /// Create a vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static CharacterVector CreateCharacter(this REngine engine, string value)
        {
            return CreateCharacterVector(engine, new[] { value });
        }

        /// <summary>
        /// Create a vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static ComplexVector CreateComplex(this REngine engine, Complex value)
        {
            return CreateComplexVector(engine, new[] { value });
        }

        /// <summary>
        /// Create a vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static LogicalVector CreateLogical(this REngine engine, bool value)
        {
            return CreateLogicalVector(engine, new[] { value });
        }

        /// <summary>
        /// Create a vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static NumericVector CreateNumeric(this REngine engine, double value)
        {
            return CreateNumericVector(engine, new[] { value });
        }

        /// <summary>
        /// Create an integer vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static IntegerVector CreateInteger(this REngine engine, int value)
        {
            return CreateIntegerVector(engine, new[] { value });
        }

        /// <summary>
        /// Create a vector with a single value
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="value">The value</param>
        /// <returns>The new vector.</returns>
        public static RawVector CreateRaw(this REngine engine, byte value)
        {
            return CreateRawVector(engine, new[] { value });
        }

        /// <summary>
        /// Creates a new empty CharacterMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static CharacterMatrix CreateCharacterMatrix(this REngine engine, int rowCount, int columnCount)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new CharacterMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new empty ComplexMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static ComplexMatrix CreateComplexMatrix(this REngine engine, int rowCount, int columnCount)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new ComplexMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new empty IntegerMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static IntegerMatrix CreateIntegerMatrix(this REngine engine, int rowCount, int columnCount)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new IntegerMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new empty LogicalMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static LogicalMatrix CreateLogicalMatrix(this REngine engine, int rowCount, int columnCount)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new LogicalMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new empty NumericMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static NumericMatrix CreateNumericMatrix(this REngine engine, int rowCount, int columnCount)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new NumericMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new empty RawMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <returns>The new matrix.</returns>
        public static RawMatrix CreateRawMatrix(this REngine engine, int rowCount, int columnCount)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new RawMatrix(engine, rowCount, columnCount);
        }

        /// <summary>
        /// Creates a new CharacterMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static CharacterMatrix CreateCharacterMatrix(this REngine engine, string[,] matrix)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new CharacterMatrix(engine, matrix);
        }

        /// <summary>
        /// Creates a new ComplexMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static ComplexMatrix CreateComplexMatrix(this REngine engine, Complex[,] matrix)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new ComplexMatrix(engine, matrix);
        }

        /// <summary>
        /// Creates a new IntegerMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static IntegerMatrix CreateIntegerMatrix(this REngine engine, int[,] matrix)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new IntegerMatrix(engine, matrix);
        }

        /// <summary>
        /// Creates a new LogicalMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static LogicalMatrix CreateLogicalMatrix(this REngine engine, bool[,] matrix)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new LogicalMatrix(engine, matrix);
        }

        /// <summary>
        /// Creates a new NumericMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static NumericMatrix CreateNumericMatrix(this REngine engine, double[,] matrix)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new NumericMatrix(engine, matrix);
        }

        /// <summary>
        /// Creates a new RawMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="matrix">The values.</param>
        /// <returns>The new matrix.</returns>
        public static RawMatrix CreateRawMatrix(this REngine engine, byte[,] matrix)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new RawMatrix(engine, matrix);
        }

        /// <summary>
        /// Create an R data frame from managed arrays and objects.
        /// </summary>
        /// <param name="engine">R engine</param>
        /// <param name="columns">The columns with the values for the data frame. These must be array of supported types (double, string, bool, integer, byte)</param>
        /// <param name="columnNames">Column names. default: null.</param>
        /// <param name="rowNames">Row names. Default null.</param>
        /// <param name="checkRows">Check rows. See data.frame R documentation</param>
        /// <param name="checkNames">See data.frame R documentation</param>
        /// <param name="stringsAsFactors">Should columns of strings be considered as factors (categories). See data.frame R documentation</param>
        /// <returns></returns>
        public static DataFrame CreateDataFrame(this REngine engine, IEnumerable[] columns, string[] columnNames = null,
           string[] rowNames = null, bool checkRows = false, bool checkNames = true, bool stringsAsFactors = true)
        {
            var df = engine.GetSymbol("data.frame").AsFunction();
            var colVectors = ToVectors(engine, columns);
            var namedColArgs = CreateNamedArgs(colVectors, columnNames);
            var args = new List<Tuple<string, SymbolicExpression>>(namedColArgs);
            if (rowNames != null) args.Add(Tuple.Create("row.names", (SymbolicExpression)engine.CreateCharacterVector(rowNames)));
            args.Add(Tuple.Create("check.rows", (SymbolicExpression)engine.CreateLogical(checkRows)));
            args.Add(Tuple.Create("check.names", (SymbolicExpression)engine.CreateLogical(checkNames)));
            args.Add(Tuple.Create("stringsAsFactors", (SymbolicExpression)engine.CreateLogical(stringsAsFactors)));
            var result = df.InvokeNamed(args.ToArray()).AsDataFrame();
            return result;
        }

        private static Tuple<string, SymbolicExpression>[] CreateNamedArgs(SymbolicExpression[] colVectors, string[] columnNames)
        {
            if (columnNames != null && colVectors.Length != columnNames.Length)
                throw new ArgumentException("when not null, the number of column names must match the number of SEXP", nameof(columnNames));
            return colVectors.Select((t, i) => Tuple.Create(columnNames != null ? columnNames[i] : "", t)).ToArray();
        }

        internal static SymbolicExpression[] ToVectors(REngine engine, IEnumerable[] columns)
        {
            return Array.ConvertAll(columns, x => ToVector(engine, x));
        }

        internal static SymbolicExpression ToVector(REngine engine, IEnumerable values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values), "values to transform to an R vector must not be null");
            var ints = values as IEnumerable<int>;
            var chars = values as IEnumerable<string>;
            var cplxs = values as IEnumerable<Complex>;
            var logicals = values as IEnumerable<bool>;
            var nums = values as IEnumerable<double>;
            var raws = values as IEnumerable<byte>;
            var sexpVec = values as SymbolicExpression;

            if (sexpVec != null && sexpVec.IsVector())
                return sexpVec;
            if (ints != null)
                return engine.CreateIntegerVector(ints);
            if (chars != null)
                return engine.CreateCharacterVector(chars);
            if (cplxs != null)
                return engine.CreateComplexVector(cplxs);
            if (logicals != null)
                return engine.CreateLogicalVector(logicals);
            if (nums != null)
                return engine.CreateNumericVector(nums);
            if (raws != null)
                return engine.CreateRawVector(raws);
            throw new NotSupportedException($"Cannot convert type {values.GetType()} to an R vector");
        }

        /// <summary>
        /// Creates a new environment.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="parent">The parent environment.</param>
        /// <returns>The newly created environment.</returns>
        public static REnvironment CreateEnvironment(this REngine engine, REnvironment parent)
        {
            ArgumentNullException.ThrowIfNull(engine);
            ArgumentNullException.ThrowIfNull(parent);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            return new REnvironment(engine, parent);
        }

        /// <summary>
        /// Creates a new isolated environment.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <returns>The newly created isolated environment.</returns>
        public static REnvironment CreateIsolatedEnvironment(this REngine engine)
        {
            ArgumentNullException.ThrowIfNull(engine);
            if (!engine.IsRunning)
            {
                throw new ArgumentException();
            }
            var pointer = engine.GetFunction<Rf_allocSExp>()(SymbolicExpressionType.Environment);
            return new REnvironment(engine, pointer);
        }
    }
}

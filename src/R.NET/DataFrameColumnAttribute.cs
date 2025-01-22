using System;
using RDotNet.Utilities;

namespace RDotNet
{
    /// <summary>
    /// Represents a column of certain data frames.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DataFrameColumnAttribute : Attribute
    {
        private static readonly string[] Empty = Array.Empty<string>();

        private readonly int index;

        /// <summary>
        /// Gets the index.
        /// </summary>
        public int Index => index;

        private string name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (index < 0 && value == null)
                {
                    throw new ArgumentNullException("value", "Name must not be null when Index is not defined.");
                }
                name = value;
            }
        }

        /// <summary>
        /// Initializes a new instance by name.
        /// </summary>
        /// <param name="name">The name.</param>
        public DataFrameColumnAttribute(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            this.name = name;
            index = -1;
        }

        /// <summary>
        /// Initializes a new instance by index.
        /// </summary>
        /// <param name="index">The index.</param>
        public DataFrameColumnAttribute(int index)
        {
            ArgumentValidation.ThrowIfNegative(index);
            name = null;
            this.index = index;
        }

        internal int GetIndex(string[] names)
        {
            return Index >= 0 ? Index : Array.IndexOf(names ?? Empty, Name);
        }
    }
}

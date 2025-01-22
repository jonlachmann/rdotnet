using System;

namespace RDotNet.Utilities
{
    public static class ArgumentValidation
    {
        public static void ThrowIfGreaterThan<T>(T value, T maxValue, string paramName = null)
            where T : IComparable<T>
        {
#if NET7_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, maxValue, paramName);
#else
            if (value.CompareTo(maxValue) > 0)
            {
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} must not be greater than {maxValue}.");
            }
#endif
        }
        public static void ThrowIfNegativeOrZero<T>(T value, string paramName = null)
#if NET7_0_OR_GREATER
            where T : System.Numerics.INumberBase<T>
#else
        where T : struct, IComparable<T>
#endif
        {
#if NET7_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
#else
            if (value.CompareTo(default(T)) <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be positive and non-zero.");
            }
#endif
        }
        public static void ThrowIfNegative<T>(T value, string paramName = null)
#if NET7_0_OR_GREATER
            where T : System.Numerics.INumberBase<T>
#else
        where T : struct, IComparable<T>
#endif
        {
#if NET7_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
#else
            if (value.CompareTo(default(T)) < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} must not be negative.");
            }
#endif
        }
    }
}

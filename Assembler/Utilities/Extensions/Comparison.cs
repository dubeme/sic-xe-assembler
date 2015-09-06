using System;

namespace SIC.Assembler.Utilities.Extensions
{
    public static class Comparison
    {
        public static bool IsEqual<T>(this T caller, object callee) where T : IComparable
        {
            return caller.CompareTo(callee) == 0;
        }

        public static bool IsGreaterThan<T>(this T caller, object callee) where T : IComparable
        {
            return caller.CompareTo(callee) > 0;
        }

        public static bool IsGreaterThanOrEqual<T>(this T caller, object callee) where T : IComparable
        {
            return caller.IsGreaterThan(callee) || caller.IsEqual(callee);
        }

        public static bool IsLessThan<T>(this T caller, object callee) where T : IComparable
        {
            return caller.CompareTo(callee) < 0;
        }

        public static bool IsLessThanOrEqual<T>(this T caller, object callee) where T : IComparable
        {
            return caller.IsLessThan(callee) || caller.IsEqual(callee);
        }

        public static bool IsNotEqual<T>(this T caller, object callee) where T : IComparable
        {
            return !caller.IsEqual(callee);
        }
    }
}
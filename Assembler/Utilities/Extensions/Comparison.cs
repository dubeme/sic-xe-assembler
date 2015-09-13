using System;

namespace SIC.Assembler.Utilities.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class Comparison
    {
        /// <summary>
        /// Determines whether the specified callee is equal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="caller">The caller.</param>
        /// <param name="callee">The callee.</param>
        /// <returns></returns>
        public static bool IsEqual<T>(this T caller, object callee) where T : IComparable
        {
            return caller.CompareTo(callee) == 0;
        }

        /// <summary>
        /// Determines whether [is greater than] [the specified callee].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="caller">The caller.</param>
        /// <param name="callee">The callee.</param>
        /// <returns></returns>
        public static bool IsGreaterThan<T>(this T caller, object callee) where T : IComparable
        {
            return caller.CompareTo(callee) > 0;
        }

        /// <summary>
        /// Determines whether [is greater than or equal] [the specified callee].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="caller">The caller.</param>
        /// <param name="callee">The callee.</param>
        /// <returns></returns>
        public static bool IsGreaterThanOrEqual<T>(this T caller, object callee) where T : IComparable
        {
            return caller.IsGreaterThan(callee) || caller.IsEqual(callee);
        }

        /// <summary>
        /// Determines whether [is less than] [the specified callee].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="caller">The caller.</param>
        /// <param name="callee">The callee.</param>
        /// <returns></returns>
        public static bool IsLessThan<T>(this T caller, object callee) where T : IComparable
        {
            return caller.CompareTo(callee) < 0;
        }

        /// <summary>
        /// Determines whether [is less than or equal] [the specified callee].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="caller">The caller.</param>
        /// <param name="callee">The callee.</param>
        /// <returns></returns>
        public static bool IsLessThanOrEqual<T>(this T caller, object callee) where T : IComparable
        {
            return caller.IsLessThan(callee) || caller.IsEqual(callee);
        }

        /// <summary>
        /// Determines whether [is not equal] [the specified callee].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="caller">The caller.</param>
        /// <param name="callee">The callee.</param>
        /// <returns></returns>
        public static bool IsNotEqual<T>(this T caller, object callee) where T : IComparable
        {
            return !caller.IsEqual(callee);
        }
    }
}
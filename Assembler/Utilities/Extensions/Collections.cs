using System;
using System.Collections.Generic;
using System.Linq;

namespace SIC.Assembler.Utilities.Extensions
{
    public static class Collections
    {
        public static void ForEach<V>(this IEnumerable<V> dict, Action<V> action)
        {
            if (action != null)
            {
                foreach (var item in dict)
                {
                    action(item);
                }
            }
        }
    }
}
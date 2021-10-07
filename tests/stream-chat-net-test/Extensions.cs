using System;
using System.Collections.Generic;
using System.Linq;

namespace StreamChatTests
{
    internal static class Extensions
    {
        internal static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> input)
        {
            if (input == null) return Enumerable.Empty<T>();
            return input;
        }

        internal static IEnumerable<T> Yield<T>(this T one)
        {
            yield return one;
        }

        internal static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if ((items == null) || (action == null)) return; // do nothing
            foreach (var item in items)
                action(item);
        }

        internal static int SafeCount<T>(this IEnumerable<T> list, int nullCountAs = 0)
        {
            if (list == null) return nullCountAs;
            return list.Count();
        }
    }
}

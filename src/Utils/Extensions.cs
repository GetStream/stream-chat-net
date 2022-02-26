using System;
using System.Collections.Generic;

namespace StreamChat.Utils
{
    internal static class Extensions
    {
        internal static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
                return;

            foreach (var item in items)
                action(item);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StreamChat
{
    internal static class Extensions
    {
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

        internal static bool IsBuiltInType(this Type type)
        {
#if NETCORE
            return
                type.GetTypeInfo().IsValueType ||
                type.GetTypeInfo().IsPrimitive ||
                new Type[] {
                typeof(String),
                typeof(Decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object;
#else
            return
                type.IsValueType ||
                type.IsPrimitive ||
                new Type[] {
                typeof(String),
                typeof(Decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object;
#endif
        }
    }
}

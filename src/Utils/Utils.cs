using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace StreamChat.Utils
{
    public static class Utils
    {
        /// <summary>
        /// Return enum value string representation respecting optional <see cref="EnumMemberAttribute"/> that overrides string representation
        /// </summary>
        public static string ToEnumMemberString<T>(this T value)
            where T : Enum
        {
            return typeof(T)
                .GetTypeInfo()
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == value.ToString())
                ?.GetCustomAttribute<EnumMemberAttribute>(false)
                ?.Value ?? value.ToString();
        }
    }
}
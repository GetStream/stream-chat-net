using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace StreamChat.Utils
{
    /// <summary>
    /// A JsonConverter that serializes enums as strings, respecting EnumMember attributes.
    /// </summary>
    public class EnumMemberStringEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var enumType = value.GetType();
            var enumValue = value.ToString();
            var memberInfo = enumType.GetTypeInfo().DeclaredMembers
                .SingleOrDefault(x => x.Name == enumValue);
            
            var enumMemberAttribute = memberInfo?.GetCustomAttribute<EnumMemberAttribute>(false);
            var stringValue = enumMemberAttribute?.Value ?? enumValue;
            
            writer.WriteValue(stringValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                var stringValue = reader.Value.ToString();
                var enumValues = Enum.GetValues(objectType);
                var enumTypeInfo = objectType.GetTypeInfo();
                
                foreach (Enum enumValue in enumValues)
                {
                    var enumValueString = enumValue.ToString();
                    var memberInfo = enumTypeInfo.DeclaredMembers
                        .SingleOrDefault(x => x.Name == enumValueString);
                    
                    var enumMemberAttribute = memberInfo?.GetCustomAttribute<EnumMemberAttribute>(false);
                    var enumMemberString = enumMemberAttribute?.Value ?? enumValueString;
                    
                    if (string.Equals(enumMemberString, stringValue, StringComparison.OrdinalIgnoreCase))
                    {
                        return enumValue;
                    }
                }
                
                // Fallback to standard enum parsing
                return Enum.Parse(objectType, stringValue, true);
            }

            throw new JsonSerializationException($"Unexpected token type: {reader.TokenType}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetTypeInfo().IsEnum;
        }
    }
}

using System;
using System.Reflection;
using Newtonsoft.Json;
using StreamChat.Utils;

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

            var enumValue = (Enum)value;
            writer.WriteValue(enumValue.ToEnumMemberString());
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
                
                foreach (Enum enumValue in enumValues)
                {
                    var enumMemberString = enumValue.ToEnumMemberString();
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

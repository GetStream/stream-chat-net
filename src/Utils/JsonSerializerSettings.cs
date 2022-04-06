using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StreamChat.Utils
{
    internal static class StreamJsonConverter
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                // this handles UserId => user_id conversion for us
                NamingStrategy = new SnakeCaseNamingStrategy(),
            },
            NullValueHandling = NullValueHandling.Ignore,
        };

        internal static string SerializeObject(object obj) => JsonConvert.SerializeObject(obj, Settings);
        internal static T DeserializeObject<T>(string json) => JsonConvert.DeserializeObject<T>(json, Settings);
    }
}
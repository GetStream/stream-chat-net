using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat.Models
{
    public abstract class CustomDataBase
    {
        [JsonExtensionData]
        private IDictionary<string, JToken> _data = new Dictionary<string, JToken>();

        /// <summary>
        /// Gets a custom data value.
        /// </summary>
        public T GetData<T>(string name) => _data.TryGetValue(name, out var val) ? val.ToObject<T>() : default(T);

        /// <summary>
        /// Gets a custom data value. If it doesn't exist, it returns <paramref name="default"/>.
        /// </summary>
        public T GetDataOrDefault<T>(string name, T @default) => _data.TryGetValue(name, out var val) ? val.ToObject<T>() : @default;

        /// <summary>
        /// Sets a custom data value.
        /// </summary>
        public void SetData<T>(string name, T data) => _data[name] = JValue.FromObject(data);
    }
}
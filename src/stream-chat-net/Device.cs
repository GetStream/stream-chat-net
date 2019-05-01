using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public class PushProvider
    {
        public const string APN = "apn";
        public const string Firebase = "firebase";
    }

    public class Device
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "push_provider")]
        public string PushProvider { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserID { get; set; }

        public Device() { }
    }
}

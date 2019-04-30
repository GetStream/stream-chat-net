using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{

    public class Device
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "push_provider")]
        public string PushProvider { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserID { get; set; }

        public Device() { }


        public static readonly string APN = "apn";
        public static readonly string Firebase = "firebase";
    }
}

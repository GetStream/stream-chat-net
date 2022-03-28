using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class Device
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "push_provider")]
        public string PushProvider { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "push_provider_name")]
        public string PushProviderName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "disabled")]
        public bool? Disabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "disabled_reason")]
        public string DisabledReason { get; set; }
    }

    public class GetDevicesResponse : ApiResponse
    {
        public List<Device> Devices { get; set; }
    }
}

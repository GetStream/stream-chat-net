using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat.Models
{
    public class Mute
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("target")]
        public User Target { get; set; }
    }

    public class MuteResponse : ApiResponse
    {
        [JsonProperty("own_user")]
        public OwnUser OwnUser { get; set; }

        [JsonProperty("mute")]
        public Mute Mute { get; set; }
    }

    public class ChannelMute
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "channel")]
        public Channel Channel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "expires")]
        public DateTimeOffset? Expires { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class UserMute
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "target")]
        public User Target { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "expires")]
        public DateTimeOffset? Expires { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}

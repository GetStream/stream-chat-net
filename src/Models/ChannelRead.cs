using System;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class ChannelRead
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "last_read")]
        public DateTimeOffset? LastRead { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "unread_messages")]
        public int? UnreadMessages { get; set; }
    }
}

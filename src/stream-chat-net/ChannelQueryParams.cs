using System;
using Newtonsoft.Json;

namespace StreamChat
{
    public class ChannelQueryParams
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "watch")]
        public bool Watch { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "state")]
        public bool State { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "presence")]
        public bool Presence { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "messages")]
        public MessagePaginationParams MessagesPagination { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "members")]
        public PaginationParams MembersPagination { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "watchers")]
        public PaginationParams WatchersPagination { get; set; }

        public ChannelQueryParams(bool watch = false, bool state = false, bool presence = false)
        {
            Watch = watch;
            State = state;
            Presence = presence;
        }
    }
}

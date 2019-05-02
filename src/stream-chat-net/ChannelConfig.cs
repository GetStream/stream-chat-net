using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat
{
    public struct Autmod
    {
        public const string Disabled = "disabled";
        public const string Simple = "simple";
        public const string AI = "ai";
    }

    public class ChannelConfig
    {
        public ChannelConfig() { }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "typing_events")]
        public bool TypingEvents { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "read_events")]
        public bool ReadEvents { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "connect_events")]
        public bool ConnectEvents { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "search")]
        public bool Search { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reactions")]
        public bool Reactions { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "replies")]
        public bool Replies { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mutes")]
        public bool Mutes { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message_retention")]
        public string MessageRetention { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "max_message_length")]
        public int MaxMessageLength { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod")]
        public string Automod { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "commands")]
        public List<Command> Commands { get; internal set; }
    }
}

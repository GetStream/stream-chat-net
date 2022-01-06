using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public abstract class ChannelConfigBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "typing_events")]
        public bool TypingEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "read_events")]
        public bool ReadEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "connect_events")]
        public bool ConnectEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "search")]
        public bool Search { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reactions")]
        public bool Reactions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "replies")]
        public bool Replies { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mutes")]
        public bool Mutes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message_retention")]
        public string MessageRetention { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "max_message_length")]
        public int MaxMessageLength { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod")]
        public string Automod { get; set; }
    }

    public class ChannelConfig : ChannelConfigBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "commands")]
        public List<string> Commands { get; set; }
    }

    public class ChannelConfigWithInfo : ChannelConfigBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "commands")]
        public List<Command> Commands { get; set; }
    }

    public class ConfigOverridesRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "typing_events")]
        public bool? TypingEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reactions")]
        public bool? Reactions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "replies")]
        public bool? Replies { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "uploads")]
        public bool? Uploads { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "url_enrichment")]
        public bool? UrlEnrichment { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "max_message_length")]
        public int? MaxMessageLength { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocklist")]
        public string Blocklist { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocklist_behavior")]
        public ModerationBehaviour BlocklistBehavior { get; set; }
    }
}

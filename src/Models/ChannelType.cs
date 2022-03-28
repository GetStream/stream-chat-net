using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamChat.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Automod
    {
        None,

        [EnumMember(Value = "disabled")]
        Disabled,

        [EnumMember(Value = "simple")]
        Simple,

        [EnumMember(Value = "ai")]
        AI,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ModerationBehaviour
    {
        Unknown,

        [EnumMember(Value = "flag")]
        Flag,

        [EnumMember(Value = "block")]
        Block,
    }

    public abstract class ChannelTypeRequestBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "typing_events")]
        public bool? TypingEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "read_events")]
        public bool? ReadEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "connect_events")]
        public bool? ConnectEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "search")]
        public bool? Search { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reactions")]
        public bool? Reactions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "replies")]
        public bool? Replies { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reminders")]
        public bool? Reminders { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "uploads")]
        public bool? Uploads { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mutes")]
        public bool? Mutes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message_retention")]
        public string MessageRetention { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "max_message_length")]
        public int? MaxMessageLength { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod")]
        public Automod? Automod { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod_behavior")]
        public ModerationBehaviour? AutomodBehavior { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod_thresholds")]
        public Dictionary<string, ModerationBehaviour> AutomodThresholds { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "url_enrichment")]
        public bool? UrlEnrichment { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "custom_events")]
        public bool? CustomEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "push_notifications")]
        public bool? PushNotifications { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocklist")]
        public string Blocklist { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocklist_behavior")]
        public ModerationBehaviour? BlocklistBehavior { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "permissions")]
        [Obsolete("Use V2 Permissions APIs instead. " +
            "See https://getstream.io/chat/docs/dotnet-csharp/migrating_from_legacy/?language=csharp")]
        public List<ChannelTypePermission> Permissions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "grants")]
        public Dictionary<string, List<string>> Grants { get; set; }
    }

    public class ChannelTypeWithCommandsRequest : ChannelTypeRequestBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "commands")]
        public List<Command> Commands { get; set; }
    }

    public class ChannelTypeWithStringCommandsRequest : ChannelTypeRequestBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "commands")]
        public List<string> Commands { get; set; }
    }

    public abstract class ChannelTypeResponseBase : ApiResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "typing_events")]
        public bool? TypingEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "read_events")]
        public bool? ReadEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "connect_events")]
        public bool? ConnectEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "search")]
        public bool? Search { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reactions")]
        public bool? Reactions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "replies")]
        public bool? Replies { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "uploads")]
        public bool? Uploads { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mutes")]
        public bool? Mutes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message_retention")]
        public string MessageRetention { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "max_message_length")]
        public int? MaxMessageLength { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod")]
        public Automod? Automod { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod_behavior")]
        public ModerationBehaviour? AutomodBehavior { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod_thresholds")]
        public Dictionary<string, ModerationBehaviour> AutomodThresholds { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "url_enrichment")]
        public bool? UrlEnrichment { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "custom_events")]
        public bool? CustomEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "push_notifications")]
        public bool? PushNotifications { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocklist")]
        public string Blocklist { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocklist_behavior")]
        public ModerationBehaviour? BlocklistBehavior { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "permissions")]
        [Obsolete("Use V2 Permissions APIs instead. " +
            "See https://getstream.io/chat/docs/dotnet-csharp/migrating_from_legacy/?language=csharp")]
        public List<ChannelTypePermission> Permissions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "grants")]
        public Dictionary<string, List<string>> Grants { get; set; }
    }

    public class ChannelTypeWithStringCommandsResponse : ChannelTypeResponseBase
    {
        public List<string> Commands { get; set; }
    }

    public class ChannelTypeWithCommandsResponse : ChannelTypeResponseBase
    {
        public List<Command> Commands { get; set; }
    }

    public class ListChannelTypesResponse : ApiResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "channel_types")]
        public Dictionary<string, ChannelTypeWithCommandsResponse> ChannelTypes { get; set; }
    }
}
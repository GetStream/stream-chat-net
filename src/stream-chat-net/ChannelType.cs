using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamChat
{
    public enum Autmod
    {
        Unknown,
        [EnumMember(Value = "disabled")]
        Disabled,
        [EnumMember(Value = "simple")]
        Simple,
        [EnumMember(Value = "ai")]
        AI,
    }

    public enum AutmodBehavior
    {
        Uknown,
        [EnumMember(Value = "flag")]
        Flag,
        [EnumMember(Value = "block")]
        Block,
    }

    public enum Threshold
    {
        Uknown,
        [EnumMember(Value = "flag")]
        Flag,
        [EnumMember(Value = "block")]
        Block,
    }

    public enum BlocklistBehavior
    {
        Uknown,
        [EnumMember(Value = "flag")]
        Flag,
        [EnumMember(Value = "block")]
        Block,
    }

    public abstract class ChannelTypeBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; internal set; }

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
        [JsonConverter(typeof(StringEnumConverter))]
        public Autmod? Automod { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod_behavior")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AutmodBehavior? AutmodBehavior { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod_thresholds")]
        public Dictionary<string, Threshold> AutmodThresholds { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "url_enrichment")]
        public bool? UrlEnrichment { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "custom_events")]
        public bool? CustomEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "push_notifications")]
        public bool? PushNotifications { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocklist")]
        public string Blocklist { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocklist_behavior")]
        [JsonConverter(typeof(StringEnumConverter))]
        public BlocklistBehavior? BlocklistBehavior { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "permissions")]
        [Obsolete("Use V2 Permissions APIs instead. " +
            "See https://getstream.io/chat/docs/dotnet-csharp/migrating_from_legacy/?language=csharp")]
        public List<ChannelTypePermission> Permissions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "grants")]
        public Dictionary<string, List<string>> Grants { get; set; }
    }

    public class ChannelTypeWithCommands : ChannelTypeBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "commands")]
        public List<Command> Commands { get; set; }
    }

    public class ChannelTypeWithStringCommands : ChannelTypeBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "commands")]
        public List<string> Commands { get; set; }
    }
}

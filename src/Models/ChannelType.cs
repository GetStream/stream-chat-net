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
        public string Name { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool? TypingEvents { get; set; }
        public bool? ReadEvents { get; set; }
        public bool? ConnectEvents { get; set; }
        public bool? Search { get; set; }
        public bool? Reactions { get; set; }
        public bool? Replies { get; set; }
        public bool? Reminders { get; set; }
        public bool? Uploads { get; set; }
        public bool? MarkMessagesPending { get; set; }
        public bool? Mutes { get; set; }
        public string MessageRetention { get; set; }
        public int? MaxMessageLength { get; set; }
        public Automod? Automod { get; set; }
        public ModerationBehaviour? AutomodBehavior { get; set; }
        public Dictionary<string, ModerationBehaviour> AutomodThresholds { get; set; }
        public bool? UrlEnrichment { get; set; }
        public bool? CustomEvents { get; set; }
        public bool? PushNotifications { get; set; }
        public string Blocklist { get; set; }
        public ModerationBehaviour? BlocklistBehavior { get; set; }

        [Obsolete("Use V2 Permissions APIs instead. " +
            "See https://getstream.io/chat/docs/dotnet-csharp/migrating_from_legacy/?language=csharp")]
        public List<ChannelTypePermission> Permissions { get; set; }
        public Dictionary<string, List<string>> Grants { get; set; }
    }

    public class ChannelTypeWithCommandsRequest : ChannelTypeRequestBase
    {
        public List<Command> Commands { get; set; }
    }

    public class ChannelTypeWithStringCommandsRequest : ChannelTypeRequestBase
    {
        public List<string> Commands { get; set; }
    }

    public abstract class ChannelTypeResponseBase : ApiResponse
    {
        public string Name { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool? TypingEvents { get; set; }
        public bool? ReadEvents { get; set; }
        public bool? ConnectEvents { get; set; }
        public bool? Search { get; set; }
        public bool? Reactions { get; set; }
        public bool? Replies { get; set; }
        public bool? Uploads { get; set; }
        public bool? Mutes { get; set; }
        public string MessageRetention { get; set; }
        public int? MaxMessageLength { get; set; }
        public Automod? Automod { get; set; }
        public ModerationBehaviour? AutomodBehavior { get; set; }
        public Dictionary<string, ModerationBehaviour> AutomodThresholds { get; set; }
        public bool? UrlEnrichment { get; set; }
        public bool? CustomEvents { get; set; }
        public bool? PushNotifications { get; set; }
        public string Blocklist { get; set; }
        public ModerationBehaviour? BlocklistBehavior { get; set; }

        [Obsolete("Use V2 Permissions APIs instead. " +
            "See https://getstream.io/chat/docs/dotnet-csharp/migrating_from_legacy/?language=csharp")]
        public List<ChannelTypePermission> Permissions { get; set; }
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
        public Dictionary<string, ChannelTypeWithCommandsResponse> ChannelTypes { get; set; }
    }
}
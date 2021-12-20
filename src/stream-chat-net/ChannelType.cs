using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat
{
    public class ChannelType : ChannelConfigWithInfo
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "permissions")]
        [Obsolete("Use V2 Permissions APIs instead. " +
            "See https://getstream.io/chat/docs/dotnet-csharp/migrating_from_legacy/?language=csharp")]
        public List<ChannelTypePermission> Permissions { get; set; }
    }

    public class ChannelTypeInput : ChannelType
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "commands")]
        public new List<string> Commands { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "max_message_length")]
        public new int? MaxMessageLength { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod")]
        public new string Automod { get; set; }
    }

    public class ChannelTypeOutput : ChannelType
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "commands")]
        public new List<string> Commands { get; set; }
    }

    public class ChannelTypeInfo : ChannelType
    {
    }
}

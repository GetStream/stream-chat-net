using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat
{
    public class ChannelType : ChannelConfigWithInfo
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "permissions")]
        public List<Permission> Permissions { get; set; }
    }

    public class ChannelTypeInput : ChannelType
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "commands")]
        public new List<string> Commands { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "max_message_length")]
        public new int? MaxMessageLength { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automod")]
        public new string Automod { get; set; }

        public ChannelTypeInput() { }
    }

    public class ChannelTypeOutput : ChannelType
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "commands")]
        public new List<string> Commands { get; set; }

        public ChannelTypeOutput() { }
    }

    public class ChannelTypeInfo : ChannelType
    {
        public ChannelTypeInfo() { }
    }
}

using System;
using System.Collections.Generic;

namespace StreamChat.Models
{
    public abstract class ChannelConfigBase
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string Name { get; set; }
        public bool TypingEvents { get; set; }
        public bool ReadEvents { get; set; }
        public bool ConnectEvents { get; set; }
        public bool Search { get; set; }
        public bool Reactions { get; set; }
        public bool Replies { get; set; }
        public bool Mutes { get; set; }
        public bool CountMessages { get; set; }
        public string MessageRetention { get; set; }
        public int MaxMessageLength { get; set; }
        public string Automod { get; set; }
        public Dictionary<string, List<string>> Grants { get; set; }
    }

    public class ChannelConfig : ChannelConfigBase
    {
        public List<string> Commands { get; set; }
    }

    public class ChannelConfigWithInfo : ChannelConfigBase
    {
        public List<Command> Commands { get; set; }
    }

    public class ConfigOverridesRequest
    {
        public bool? TypingEvents { get; set; }
        public bool? Reactions { get; set; }
        public bool? Replies { get; set; }
        public bool? Uploads { get; set; }
        public bool? UrlEnrichment { get; set; }
        public int? MaxMessageLength { get; set; }
        public string Blocklist { get; set; }
        public ModerationBehaviour BlocklistBehavior { get; set; }
        public List<string> Commands { get; set; }
        public bool? UserMessageReminders { get; set; }
        public bool? SharedLocations { get; set; }
        public bool? CountMessages { get; set; }
    }
}

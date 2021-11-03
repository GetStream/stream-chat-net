using System;
using Newtonsoft.Json;


namespace StreamChat
{
    public struct Commands
    {
        public const string All = "all";
        public const string FunSet = "fun_set";
        public const string ModerationSet = "moderation_set";
        public const string Giphy = "giphy";
        public const string Imgur = "imgur";
        public const string Ban = "ban";
        public const string Mute = "mute";
    }

    public class Command
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "args")]
        public string Args { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "set")]
        public string Set { get; set; }

        public Command() { }
    }
}

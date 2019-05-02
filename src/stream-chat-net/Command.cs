using System;
using Newtonsoft.Json;


namespace StreamChat
{
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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public static class Commands
    {
        public const string All = "all";
        public const string FunSet = "fun_set";
        public const string ModerationSet = "moderation_set";
        public const string Giphy = "giphy";
        public const string Ban = "ban";
        public const string Mute = "mute";
    }

    public abstract class CommandBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "args")]
        public string Args { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "set")]
        public string Set { get; set; }
    }

    public class Command : CommandBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }
    }

    public class CommandCreateRequest : Command
    {
    }

    public class CommandGetResponse : ApiResponse
    {
        public string Description { get; set; }

        public string Args { get; set; }

        public string Set { get; set; }

        public string Name { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty(PropertyName = "updated_at")]
        public string UpdatedAt { get; set; }
    }

    public class CommandUpdateRequest : CommandBase
    {
    }

    public class CommandResponse : ApiResponse
    {
        public Command Command { get; set; }
    }

    public class ListCommandsResponse : ApiResponse
    {
        public List<Command> Commands { get; set; }
    }
}

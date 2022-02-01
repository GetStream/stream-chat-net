using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public static class Resource
    {
        public const string ResourceAny = "*";
        public const string CreateChannel = "CreateChannel";
        public const string ReadChannel = "ReadChannel";
        public const string UpdateChannelRoles = "UpdateChannelRoles";
        public const string UpdateChannelMembers = "UpdateChannelMembers";
        public const string UpdateChannel = "UpdateChannel";
        public const string UpdateUser = "UpdateUser";
        public const string UpdateUserRole = "UpdateUserRole";
        public const string DeleteChannel = "DeleteChannel";
        public const string CreateMessage = "CreateMessage";
        public const string UpdateMessage = "UpdateMessage";
        public const string DeleteMessage = "DeleteMessage";
        public const string RunMessageAction = "RunMessageAction";
        public const string MuteUser = "MuteUser";
        public const string BanUser = "BanUser";
        public const string EditUser = "EditUser";
        public const string UploadAttachment = "UploadAttachment";
        public const string DeleteAttachment = "DeleteAttachment";
        public const string UseCommands = "UseCommands";
        public const string AddLinks = "AddLinks";
    }

    [Obsolete("Use V2 Permissions APIs instead. " +
            "See https://getstream.io/chat/docs/dotnet-csharp/migrating_from_legacy/?language=csharp")]
    public class ChannelTypePermission
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "resources")]
        public List<string> Resources { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "roles")]
        public List<string> Roles { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "owner")]
        public bool Owner { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "priority")]
        public int Priority { get; set; }
    }
}

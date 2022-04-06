using System;
using System.Collections.Generic;

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
        public string Action { get; set; }
        public string Name { get; set; }
        public List<string> Resources { get; set; }
        public List<string> Roles { get; set; }
        public bool Owner { get; set; }
        public int Priority { get; set; }
    }
}

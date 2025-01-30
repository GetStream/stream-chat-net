using System;

namespace StreamChat.Models
{
    public static class ChannelRole
    {
        public const string Member = "member";
        public const string Moderator = "moderator";
        public const string Admin = "admin";
        public const string Owner = "owner";
    }

    public class ChannelMember : CustomDataBase
    {
        public string UserId { get; set; }
        public UserRequest User { get; set; }
        public bool? IsModerator { get; set; }
        public bool? Invited { get; set; }
        public DateTimeOffset? InviteAcceptedAt { get; set; }
        public DateTimeOffset? InviteRejectedAt { get; set; }
        public string Role { get; set; }
        public string ChannelRole { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool? Banned { get; set; }
        public DateTimeOffset? BanExpires { get; set; }
        public bool? ShadowBanned { get; set; }
        public bool? NotificationsMuted { get; set; }
        public DateTimeOffset? PinnedAt { get; set; }
        public DateTimeOffset? ArchivedAt { get; set; }
    }

    public class ChannelMemberResponse : ApiResponse
    {
        public ChannelMember ChannelMember { get; set; }
    }
}
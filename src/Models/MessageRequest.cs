using System;
using System.Collections.Generic;

namespace StreamChat.Models
{
    public enum MessageRequestType
    {
        [System.Runtime.Serialization.EnumMember(Value = @"regular")]
        Regular = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"ephemeral")]
        Ephemeral = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"error")]
        Error = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"reply")]
        Reply = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"system")]
        System = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"deleted")]
        Deleted = 5,
    }
    public class MessageRequest : CustomDataBase
    {
        public string Id { get; set; }
        public MessageRequest Type { get; set; }
        public string Text { get; set; }
        public string Mml { get; set; }
        public UserRequest User { get; set; }
        public string ParentId { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
        public bool? ShownInChannel { get; set; }
        public IEnumerable<string> MentionedUsers { get; set; }
        public string UserId { get; set; }
        public string HTML { get; set; }
        public Dictionary<string, int> ReactionScores { get; set; }
        public string QuotedMessageId { get; set; }
        public string Cid { get; set; }
        public bool? Silent { get; set; }
        public bool? Pinned { get; set; }
        public DateTimeOffset? PinExpires { get; set; }
        public UserRequest PinnedBy { get; set; }
        public DateTimeOffset? PinnedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamChat.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageRequestType
    {
        [EnumMember(Value = @"regular")]
        Regular = 0,

        [EnumMember(Value = @"system")]
        System = 1,
    }

    public class MessageRequest : CustomDataBase
    {
        public string Id { get; set; }
        public MessageRequestType Type { get; set; }
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

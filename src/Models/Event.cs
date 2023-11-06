using System;

namespace StreamChat.Models
{
    /// <summary>
    /// Event model for custom event requests
    /// </summary>
    public class Event : CustomDataBase
    {
        public string Type { get; set; }
        public string ConnectionId { get; set; }
        public string Cid { get; set; }
        public string ChannelId { get; set; }
        public string ChannelType { get; set; }
        public MessageRequest Message { get; set; }
        public Reaction Reaction { get; set; }
        public ChannelRequest Channel { get; set; }
        public ChannelMember Member { get; set; }
        public UserRequest User { get; set; }
        public string UserId { get; set; }
        public OwnUser Me { get; set; }
        public int? WatcherCount { get; set; }
        public string Reason { get; set; }
        public User CreatedBy { get; set; }
        public bool? AutoModeration { get; set; }
        public ModerationScore AutomoderationScores { get; set; }
        public string ParentId { get; set; }
        public string Team { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
    
    /// <summary>
    /// Event model for received events
    /// </summary>
    public class EventResponse : CustomDataBase
    {
        public string Type { get; set; }
        public string ConnectionId { get; set; }
        public string Cid { get; set; }
        public string ChannelId { get; set; }
        public string ChannelType { get; set; }
        public Message Message { get; set; }
        public Reaction Reaction { get; set; }
        public Channel Channel { get; set; }
        public ChannelMember Member { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public OwnUser Me { get; set; }
        public int? WatcherCount { get; set; }
        public string Reason { get; set; }
        public User CreatedBy { get; set; }
        public bool? AutoModeration { get; set; }
        public ModerationScore AutomoderationScores { get; set; }
        public string ParentId { get; set; }
        public string Team { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }

    public class UserCustomEvent : CustomDataBase
    {
        public string Type { get; set; }
    }

    public class SendEventResponse : ApiResponse
    {
        public Event Event { get; set; }
    }

    public class ModerationScore
    {
        public int Toxic { get; set; }
        public int Explicit { get; set; }
        public int Spam { get; set; }
    }
}

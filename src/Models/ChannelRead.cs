using System;

namespace StreamChat.Models
{
    public class ChannelRead
    {
        public User User { get; set; }
        public DateTimeOffset? LastRead { get; set; }
        public int? UnreadMessages { get; set; }
        public DateTimeOffset? LastDeliveredAt { get; set; }
        public string LastDeliveredMessageId { get; set; }
    }
}

using System;

namespace StreamChat.Models
{
    public class Mute
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public User User { get; set; }
        public User Target { get; set; }
    }

    public class MuteResponse : ApiResponse
    {
        public OwnUser OwnUser { get; set; }
        public Mute Mute { get; set; }
    }

    public class ChannelMute
    {
        public User User { get; set; }
        public Channel Channel { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class UserMute
    {
        public User User { get; set; }
        public User Target { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}

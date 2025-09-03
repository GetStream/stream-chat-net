using System.Collections.Generic;

namespace StreamChat.Models
{
    /// <summary>
    /// Options for marking messages as delivered
    /// </summary>
    public class MarkDeliveredOptions
    {
        /// <summary>
        /// Mapping of channel IDs to message IDs that have been delivered
        /// </summary>
        public Dictionary<string, string> ChannelDeliveredMessage { get; set; }

        /// <summary>
        /// Optional client ID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Optional connection ID
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// Optional user object
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Optional user ID
        /// </summary>
        public string UserId { get; set; }
    }
}
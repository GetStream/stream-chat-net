using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    /// <summary>
    /// Options for marking messages as delivered
    /// </summary>
    public class MarkDeliveredOptions
    {
        /// <summary>
        /// List of delivered message confirmations containing channel CID and message ID
        /// </summary>
        [JsonProperty("latest_delivered_messages")]
        public List<DeliveredMessageConfirmation> LatestDeliveredMessages { get; set; }

        /// <summary>
        /// Optional user object
        /// </summary>
        [JsonProperty("user")]
        public UserRequest User { get; set; }

        /// <summary>
        /// Optional user ID
        /// </summary>
        [JsonProperty("user_id")]
        public string UserID { get; set; }
    }
}
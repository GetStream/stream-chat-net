using Newtonsoft.Json;

namespace StreamChat.Models
{
    /// <summary>
    /// Represents a delivered message confirmation containing channel CID and message ID
    /// </summary>
    public class DeliveredMessageConfirmation
    {
        /// <summary>
        /// The channel CID (channel type:channel id)
        /// </summary>
        [JsonProperty("cid")]
        public string ChannelCID { get; set; }

        /// <summary>
        /// The message ID
        /// </summary>
        [JsonProperty("id")]
        public string MessageID { get; set; }
    }
}

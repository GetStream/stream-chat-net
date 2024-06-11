using System;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class BlockUserResponse : ApiResponse
    {
        public string BlockedByUserID { get; set; }
        public string BlockedUserID { get; set; }
        public string CreatedAt { get; set; }
    }

    public class GetBlockedUsersResponse : ApiResponse
    {
        [JsonProperty("blocks")]
        public Blocks[] Blocks { get; set; }
    }

    public class Blocks
    {
        [JsonProperty("user")]
        public User BlockedByUser { get; set; }

        [JsonProperty("user_id")]
        public string BlockedByUserID { get; set; }

        [JsonProperty("blocked_user")]
        public User BlockedUser { get; set; }

        [JsonProperty("blocked_user_id")]
        public string BlockedUserID { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
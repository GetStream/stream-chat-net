using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class ExportedUser : ApiResponse
    {
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("messages")]
        public List<Message> Messages { get; set; }

        [JsonProperty("reactions")]
        public List<Reaction> Reactions { get; set; }
    }
}

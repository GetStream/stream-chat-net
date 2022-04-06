using System.Collections.Generic;

namespace StreamChat.Models
{
    public class ExportedUser : ApiResponse
    {
        public User User { get; set; }
        public List<Message> Messages { get; set; }
        public List<Reaction> Reactions { get; set; }
    }
}

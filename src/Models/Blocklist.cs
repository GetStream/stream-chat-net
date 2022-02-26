using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public abstract class BlocklistBase
    {
        public string Name { get; set; }
        public IEnumerable<string> Words { get; set; }
    }

    public class Blocklist : BlocklistBase
    {
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class BlocklistCreateRequest : BlocklistBase
    {
    }

    public class GetBlocklistResponse : ApiResponse
    {
        public Blocklist Blocklist { get; set; }
    }

    public class ListBlocklistsResponse : ApiResponse
    {
        public List<Blocklist> Blocklists { get; set; }
    }
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public abstract class BlocklistBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "words")]
        public IEnumerable<string> Words { get; set; }
    }

    public class Blocklist : BlocklistBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
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
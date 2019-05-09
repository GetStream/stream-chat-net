using System;
using Newtonsoft.Json;

namespace StreamChat
{
    public class PaginationParams
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "limit")]
        public int Limit { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "offset")]
        public int Offset { get; set; }

        public PaginationParams() { }
    }

    public class MessagePaginationParams
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "limit")]
        public int Limit { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "offset")]
        public int Offset { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id_gte")]
        public string IDGTE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id_gt")]
        public string IDGT { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id_lte")]
        public string IDLTE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id_lt")]
        public string IDLT { get; set; }

        public MessagePaginationParams() { }
    }
}

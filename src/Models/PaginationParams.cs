using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class PaginationParams
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
    }

    public class MessagePaginationParams : IQueryParameterConvertible
    {
        public int Limit { get; set; }
        public int Offset { get; set; }

        [JsonProperty("id_gte")]
        public string IDGTE { get; set; }

        [JsonProperty("id_gt")]
        public string IDGT { get; set; }

        [JsonProperty("id_lte")]
        public string IDLTE { get; set; }

        [JsonProperty("id_lt")]
        public string IDLT { get; set; }

        public static MessagePaginationParams Default
        {
            get => new MessagePaginationParams { Limit = 20 };
        }

        public List<KeyValuePair<string, string>> ToQueryParameters()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("limit", Limit.ToString()),
                new KeyValuePair<string, string>("offset", Offset.ToString()),
                new KeyValuePair<string, string>("id_gte", IDGTE ?? string.Empty),
                new KeyValuePair<string, string>("id_gt", IDGT ?? string.Empty),
                new KeyValuePair<string, string>("id_lte", IDLTE ?? string.Empty),
                new KeyValuePair<string, string>("id_lt", IDLT ?? string.Empty),
            };
        }
    }
}

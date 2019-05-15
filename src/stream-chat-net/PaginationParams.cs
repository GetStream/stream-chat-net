using System;
using Newtonsoft.Json;
using StreamChat.Rest;

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

        [JsonIgnore]
        public static MessagePaginationParams Default
        {
            get
            {
                return new MessagePaginationParams()
                {
                    Limit = 20
                };
            }
        }

        public MessagePaginationParams() { }

        internal void Apply(RestRequest request)
        {
            request.AddQueryParameter("limit", this.Limit.ToString());
            request.AddQueryParameter("offset", this.Offset.ToString());
            request.AddQueryParameter("id_gte", this.IDGTE ?? "");
            request.AddQueryParameter("id_gt", this.IDGT ?? "");
            request.AddQueryParameter("id_lte", this.IDLTE ?? "");
            request.AddQueryParameter("id_lt", this.IDLT ?? "");
        }
    }
}

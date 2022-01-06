using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class ChannelGetRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "connection_id")]
        public string ConnectionId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "data")]
        public ChannelRequest Data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "watch")]
        public bool? Watch { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "state")]
        public bool? State { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "presence")]
        public bool? Presence { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "messages")]
        public MessagePaginationParams MessagesPagination { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "members")]
        public PaginationParams MembersPagination { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "watchers")]
        public PaginationParams WatchersPagination { get; set; }

        public static ChannelGetRequest WithoutWatching() => new ChannelGetRequest { Watch = false, State = false, Presence = false };
    }

    public class QueryMembersRequest : IQueryParameterConvertible
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "members")]
        public ChannelMember Members { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "filter_conditions")]
        public Dictionary<string, object> FilterConditions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "sort")]
        public IEnumerable<SortParameter> Sorts { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "limit")]
        public int? Limit { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "offset")]
        public int? Offset { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id_gte")]
        public string UserIdGte { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id_gt")]
        public string UserIdGt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id_lte")]
        public string UserIdLte { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id_lt")]
        public string UserIdLt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at_after_or_equal")]
        public DateTimeOffset? CreatedAtAfterOrEqual { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at_after")]
        public DateTimeOffset? CreatedAtAfter { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at_before_or_equal")]
        public DateTimeOffset? CreatedAtBeforeOrEqual { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at_before")]
        public DateTimeOffset? CreatedAtBefore { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public UserRequest User { get; set; }
        public List<KeyValuePair<string, string>> ToQueryParameters()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("payload", JsonConvert.SerializeObject(this)),
            };
        }
    }
}

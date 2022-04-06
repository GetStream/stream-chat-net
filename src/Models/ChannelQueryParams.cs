using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using StreamChat.Utils;

namespace StreamChat.Models
{
    public class ChannelGetRequest
    {
        public string ConnectionId { get; set; }
        public ChannelRequest Data { get; set; }
        public bool? Watch { get; set; }
        public bool? State { get; set; }
        public bool? Presence { get; set; }

        [JsonProperty("messages")]
        public MessagePaginationParams MessagesPagination { get; set; }

        [JsonProperty("members")]
        public PaginationParams MembersPagination { get; set; }

        [JsonProperty("watchers")]
        public PaginationParams WatchersPagination { get; set; }

        public static ChannelGetRequest WithoutWatching() => new ChannelGetRequest { Watch = false, State = false, Presence = false };
    }

    public class QueryMembersRequest : IQueryParameterConvertible
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public ChannelMember Members { get; set; }
        public Dictionary<string, object> FilterConditions { get; set; }

        [JsonProperty("sort")]
        public IEnumerable<SortParameter> Sorts { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public string UserIdGte { get; set; }
        public string UserIdGt { get; set; }
        public string UserIdLte { get; set; }
        public string UserIdLt { get; set; }
        public DateTimeOffset? CreatedAtAfterOrEqual { get; set; }
        public DateTimeOffset? CreatedAtAfter { get; set; }
        public DateTimeOffset? CreatedAtBeforeOrEqual { get; set; }
        public DateTimeOffset? CreatedAtBefore { get; set; }
        public string UserId { get; set; }
        public UserRequest User { get; set; }

        public List<KeyValuePair<string, string>> ToQueryParameters()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("payload", StreamJsonConverter.SerializeObject(this)),
            };
        }
    }
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public enum SortDirection
    {
        None,
        Ascending = 1,
        Descending = -1,
    }

    public class SortParameter
    {
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        [JsonProperty(PropertyName = "direction")]
        public SortDirection Direction { get; set; } = SortDirection.Ascending;
    }

    public class QueryUserOptions
    {
        private const int DefaultOffset = 0;
        private const int DefaultLimit = 20;

        public int Offset { get; set; } = DefaultOffset;
        public int Limit { get; set; } = DefaultLimit;
        public bool Presence { get; set; } = false;
        public List<SortParameter> Sort { get; set; } = new List<SortParameter>();
        public Dictionary<string, object> Filter { get; set; } = new Dictionary<string, object>();

        public QueryUserOptions WithOffset(int offset)
        {
            Offset = offset;
            return this;
        }

        public QueryUserOptions WithLimit(int limit)
        {
            Limit = limit;
            return this;
        }

        public QueryUserOptions WithPresence()
        {
            Presence = true;
            return this;
        }

        public QueryUserOptions WithSortBy(SortParameter param)
        {
            Sort.Add(param);
            return this;
        }

        public QueryUserOptions WithFilter(Dictionary<string, object> filter)
        {
            Filter = filter;
            return this;
        }

        public static QueryUserOptions Default
        {
            get => new QueryUserOptions
            {
                Offset = DefaultOffset,
                Limit = DefaultLimit,
            };
        }
    }

    public class QueryChannelsOptions
    {
        private const int DefaultOffset = 0;
        private const int DefaultLimit = 20;

        [JsonProperty("offset")]
        public int Offset { get; set; } = DefaultOffset;

        [JsonProperty("limit")]
        public int Limit { get; set; } = DefaultLimit;

        [JsonProperty("presence")]
        public bool Presence { get; set; }

        [JsonProperty("state")]
        public bool State { get; set; }

        [JsonProperty("watch")]
        public bool Watch { get; set; }

        [JsonProperty("recovery")]
        public bool Recovery { get; set; }

        [JsonProperty("sort", NullValueHandling = NullValueHandling.Ignore)]
        public List<SortParameter> Sort { get; set; }

        [JsonProperty("filter_conditions")]
        public Dictionary<string, object> Filter { get; set; } = new Dictionary<string, object>();

        [JsonProperty("last_message_ids", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> LastMessageIds { get; set; }

        public QueryChannelsOptions WithOffset(int offset)
        {
            Offset = offset;
            return this;
        }

        public QueryChannelsOptions WithLimit(int limit)
        {
            Limit = limit;
            return this;
        }

        public QueryChannelsOptions WithPresence()
        {
            Presence = true;
            return this;
        }

        public QueryChannelsOptions WithState()
        {
            State = true;
            return this;
        }

        public QueryChannelsOptions WithWatch()
        {
            Watch = true;
            return this;
        }

        public QueryChannelsOptions WithRecovery()
        {
            Recovery = true;
            return this;
        }

        public QueryChannelsOptions WithSortBy(SortParameter param)
        {
            if (Sort == null)
                Sort = new List<SortParameter>();

            Sort.Add(param);
            return this;
        }

        public QueryChannelsOptions WithFilter(Dictionary<string, object> filter)
        {
            Filter = filter;
            return this;
        }

        public QueryChannelsOptions WithLastMessageIDs(Dictionary<string, string> msgIds)
        {
            LastMessageIds = msgIds;
            return this;
        }

        public static QueryChannelsOptions Default
        {
            get => new QueryChannelsOptions
            {
                Offset = DefaultOffset,
                Limit = DefaultLimit,
            };
        }
    }

    public class SearchOptions : IQueryParameterConvertible
    {
        private const int DefaultOffset = 0;
        private const int DefaultLimit = 20;

        private int _offset = DefaultOffset;
        private int _limit = DefaultLimit;
        private string _next = null;
        private string _query = null;
        private List<SortParameter> _sort = new List<SortParameter>();
        private Dictionary<string, object> _filter = new Dictionary<string, object>();
        private Dictionary<string, object> _message_filter_conditions = new Dictionary<string, object>();

        public SearchOptions WithOffset(int offset)
        {
            _offset = offset;
            return this;
        }

        public SearchOptions WithLimit(int limit)
        {
            _limit = limit;
            return this;
        }

        public SearchOptions WithSortBy(SortParameter param)
        {
            _sort.Add(param);
            return this;
        }

        public SearchOptions WithFilter(Dictionary<string, object> filter)
        {
            _filter = filter;
            return this;
        }

        public SearchOptions WithQuery(string query)
        {
            _query = query;
            return this;
        }

        public SearchOptions WithMessageFilterConditions(Dictionary<string, object> filter)
        {
            _message_filter_conditions = filter;
            return this;
        }

        public SearchOptions WithNext(string next)
        {
            _next = next;
            return this;
        }

        public static SearchOptions Default
        {
            get => new SearchOptions
            {
                _offset = DefaultOffset,
                _limit = DefaultLimit,
            };
        }

        public List<KeyValuePair<string, string>> ToQueryParameters()
        {
            if (_offset > 0 && (_sort.Count > 0 || !string.IsNullOrEmpty(_next)))
            {
                throw new ArgumentException("offset", "Cannot use offset with sort or next parameters");
            }

            if (!string.IsNullOrEmpty(_query) && _message_filter_conditions.Count > 0)
            {
                throw new ArgumentException("query", "Cannot specify both query and message filter conditions");
            }

            if (string.IsNullOrEmpty(_query) && _message_filter_conditions.Count == 0)
            {
                throw new ArgumentException("query", "Must specify one of query and message filter conditions");
            }

            var payload = new
            {
                limit = _limit,
                filter_conditions = _filter,
                sort = _sort,
                offset = _offset,
                next = _next,
                query = _query,
                message_filter_conditions = _message_filter_conditions,
            };

            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("payload", JsonConvert.SerializeObject(payload)),
            };
        }
    }
}

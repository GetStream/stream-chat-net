using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using StreamChat.Utils;

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
        public string Field { get; set; }
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

        public int Offset { get; set; } = DefaultOffset;
        public int Limit { get; set; } = DefaultLimit;
        public int MemberLimit { get; set; }
        public int MessageLimit { get; set; }
        public bool Presence { get; set; }
        public bool State { get; set; }
        public bool Watch { get; set; }
        public bool Recovery { get; set; }
        public List<SortParameter> Sort { get; set; } = new List<SortParameter>();
        [JsonProperty("filter_conditions")]
        public Dictionary<string, object> Filter { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, string> LastMessageIds { get; set; }
        public string UserId { get; set; }

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
                new KeyValuePair<string, string>("payload", StreamJsonConverter.SerializeObject(payload)),
            };
        }
    }

    public class QueryThreadsOptions
    {
        private const int DefaultOffset = 0;
        private const int DefaultLimit = 10;

        public int Offset { get; set; } = DefaultOffset;
        public int Limit { get; set; } = DefaultLimit;
        public List<SortParameter> Sort { get; set; } = new List<SortParameter>();
        public Dictionary<string, object> Filter { get; set; } = new Dictionary<string, object>();
        public string UserId { get; set; }

        public QueryThreadsOptions WithOffset(int offset)
        {
            Offset = offset;
            return this;
        }

        public QueryThreadsOptions WithLimit(int limit)
        {
            Limit = limit;
            return this;
        }

        public QueryThreadsOptions WithSortBy(SortParameter param)
        {
            Sort.Add(param);
            return this;
        }

        public QueryThreadsOptions WithFilter(Dictionary<string, object> filter)
        {
            Filter = filter;
            return this;
        }

        public QueryThreadsOptions WithUserId(string userId)
        {
            UserId = userId;
            return this;
        }

        public static QueryThreadsOptions Default
        {
            get => new QueryThreadsOptions
            {
                Offset = DefaultOffset,
                Limit = DefaultLimit,
            };
        }
    }
}

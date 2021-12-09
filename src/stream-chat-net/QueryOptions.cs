using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using StreamChat.Rest;

namespace StreamChat
{
    public enum SortDirection
    {
        Ascending = 1,
        Descending = -1
    }

    public class SortParameter
    {
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        [JsonProperty(PropertyName = "direction")]
        public SortDirection Direction { get; set; }        
    }

    public class QueryUserOptions
    {
        const int DefaultOffset = 0;
        const int DefaultLimit = 20;

        int _offset = DefaultOffset;
        int _limit = DefaultLimit;
        bool _presence = false;
        List<SortParameter> _sort = new List<SortParameter>();
        Dictionary<string, object> _filter = new Dictionary<string, object>();

        public QueryUserOptions WithOffset(int offset)
        {
            _offset = offset;
            return this;
        }

        public QueryUserOptions WithLimit(int limit)
        {
            _limit = limit;
            return this;
        }

        public QueryUserOptions WithPresence()
        {
            _presence = true;
            return this;
        }

        public QueryUserOptions WithSortBy(SortParameter param)
        {
            _sort.Add(param);
            return this;
        }

        public QueryUserOptions WithFilter(Dictionary<string, object> filter)
        {
            _filter = filter;
            return this;
        }

        public static QueryUserOptions Default
        {
            get
            {
                return new QueryUserOptions()
                {
                    _offset = DefaultOffset,
                    _limit = DefaultLimit
                };
            }
        }

        internal void Apply(RestRequest request)
        {
            var payload = new
            {
                offset = _offset,
                limit = _limit,
                presence = _presence,
                filter_conditions = _filter,
                sort = _sort,
            };
            request.AddQueryParameter("payload", JsonConvert.SerializeObject(payload));
        }
    }

    public class QueryChannelsOptions
    {
        const int DefaultOffset = 0;
        const int DefaultLimit = 20;

        int _offset = DefaultOffset;
        int _limit = DefaultLimit;

        bool _presence = false;
        bool _state = false;
        bool _watch = false;
        bool _recovery = false;


        List<SortParameter> _sort = new List<SortParameter>();
        Dictionary<string, object> _filter = new Dictionary<string, object>();
        Dictionary<string, string> _lastMessageIDs = new Dictionary<string, string>();

        public QueryChannelsOptions WithOffset(int offset)
        {
            _offset = offset;
            return this;
        }

        public QueryChannelsOptions WithLimit(int limit)
        {
            _limit = limit;
            return this;
        }

        public QueryChannelsOptions WithPresence()
        {
            _presence = true;
            return this;
        }

        public QueryChannelsOptions WithState()
        {
            _state = true;
            return this;
        }

        public QueryChannelsOptions WithWatch()
        {
            _watch = true;
            return this;
        }

        public QueryChannelsOptions WithRecovery()
        {
            _recovery = true;
            return this;
        }

        public QueryChannelsOptions WithSortBy(SortParameter param)
        {
            _sort.Add(param);
            return this;
        }

        public QueryChannelsOptions WithFilter(Dictionary<string, object> filter)
        {
            _filter = filter;
            return this;
        }

        public QueryChannelsOptions WithLastMessageIDs(Dictionary<string, string> msgIDS)
        {
            _lastMessageIDs = msgIDS;
            return this;
        }

        public static QueryChannelsOptions Default
        {
            get
            {
                return new QueryChannelsOptions()
                {
                    _offset = DefaultOffset,
                    _limit = DefaultLimit
                };
            }
        }

        internal void Apply(RestRequest request)
        {
            var payload = new
            {
                offset = _offset,
                limit = _limit,
                presence = _presence,
                watch = _watch,
                state = _state,
                recovery = _recovery,
                last_message_ids = _lastMessageIDs,
                filter_conditions = _filter,
                sort = _sort,
            };
            request.AddQueryParameter("payload", JsonConvert.SerializeObject(payload));
        }
    }

    public class SearchOptions
    {
        const int DefaultOffset = 0;
        const int DefaultLimit = 20;

        int _offset = DefaultOffset;
        int _limit = DefaultLimit;
        string _next = null;
        string _query = null;
        List<SortParameter> _sort = new List<SortParameter>();
        Dictionary<string, object> _filter = new Dictionary<string, object>();
        Dictionary<string, object> _message_filter_conditions = new Dictionary<string, object>();

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
            get
            {
                return new SearchOptions()
                {
                    _offset = DefaultOffset,
                    _limit = DefaultLimit
                };
            }
        }

        internal void Apply(RestRequest request)
        {
            if (_offset > 0 && (_sort.Count > 0 || !String.IsNullOrEmpty(_next)))
            {
                throw new ArgumentException("offset", "Cannot use offset with sort or next parameters");
            }

            if (!String.IsNullOrEmpty(_query) && _message_filter_conditions.Count > 0)
            {
                throw new ArgumentException("query", "Cannot specify both query and message filter conditions");
            }

            if (String.IsNullOrEmpty(_query) && _message_filter_conditions.Count == 0)
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
            request.AddQueryParameter("payload", JsonConvert.SerializeObject(payload));
        }
    }
}

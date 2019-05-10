using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public SortParameter() { }
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
}

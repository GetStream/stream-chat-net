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
}

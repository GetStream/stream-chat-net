using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class ThreadClient : ClientBase, IThreadClient
    {
        internal ThreadClient(IRestClient client) : base(client)
        {
        }

        public async Task<QueryThreadsResponse> QueryThreadsAsync(QueryThreadsOptions options)
        {
            var payload = new
            {
                offset = options.Offset,
                limit = options.Limit,
                filter = options.Filter,
                sort = options.Sort,
                user_id = options.UserId,
            };

            return await ExecuteRequestAsync<QueryThreadsResponse>("threads",
                HttpMethod.POST,
                HttpStatusCode.Created,
                body: payload);
        }
    }
}

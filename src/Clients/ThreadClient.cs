using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class ThreadClient : ClientBase
    {
        public ThreadClient(IRestClient client) : base(client)
        {
        }

        private static string Endpoint() => "threads";

        public async Task<QueryThreadsResponse> QueryThreads(QueryThreadsOptions opts)
        {
            var payload = new
            {
                offset = opts.Offset,
                limit = opts.Limit,
                filter = opts.Filter,
                sort = opts.Sort,
            };

            return await ExecuteRequestAsync<QueryThreadsResponse>(Endpoint(),
                HttpMethod.POST,
                HttpStatusCode.OK,
                body: payload);
        }
    }
}

using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class BlocklistClient : ClientBase, IBlocklistClient
    {
        internal BlocklistClient(IRestClient client) : base(client)
        {
        }

        public async Task<GetBlocklistResponse> GetAsync(string name)
            => await ExecuteRequestAsync<GetBlocklistResponse>($"blocklists/{name}",
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<ApiResponse> CreateAsync(BlocklistCreateRequest request)
            => await ExecuteRequestAsync<ApiResponse>("blocklists",
                HttpMethod.POST,
                HttpStatusCode.Created,
                request);

        public async Task<ApiResponse> DeleteAsync(string name)
            => await ExecuteRequestAsync<ApiResponse>($"blocklists/{name}",
                HttpMethod.DELETE,
                HttpStatusCode.OK);

        public async Task<ApiResponse> UpdateAsync(string name, IEnumerable<string> words)
            => await ExecuteRequestAsync<ApiResponse>($"blocklists/{name}",
                HttpMethod.PUT,
                HttpStatusCode.Created,
                new { words });

        public async Task<ListBlocklistsResponse> ListAsync()
            => await ExecuteRequestAsync<ListBlocklistsResponse>("blocklists",
                HttpMethod.GET,
                HttpStatusCode.OK);
    }
}
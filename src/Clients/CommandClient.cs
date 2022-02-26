using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class CommandClient : ClientBase, ICommandClient
    {
        internal CommandClient(IRestClient client) : base(client)
        {
        }

        public async Task<CommandGetResponse> GetAsync(string name)
            => await ExecuteRequestAsync<CommandGetResponse>($"commands/{name}",
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<CommandResponse> CreateAsync(CommandCreateRequest request)
            => await ExecuteRequestAsync<CommandResponse>("commands",
                HttpMethod.POST,
                HttpStatusCode.Created,
                request);

        public async Task<ApiResponse> DeleteAsync(string name)
            => await ExecuteRequestAsync<ApiResponse>($"commands/{name}",
                HttpMethod.DELETE,
                HttpStatusCode.OK);

        public async Task<CommandResponse> UpdateAsync(string name, CommandUpdateRequest request)
            => await ExecuteRequestAsync<CommandResponse>($"commands/{name}",
                HttpMethod.PUT,
                HttpStatusCode.Created,
                request);

        public async Task<ListCommandsResponse> ListAsync()
            => await ExecuteRequestAsync<ListCommandsResponse>("commands",
                HttpMethod.GET,
                HttpStatusCode.OK);
    }
}
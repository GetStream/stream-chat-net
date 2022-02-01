using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class ChannelTypeClient : ClientBase, IChannelTypeClient
    {
        internal ChannelTypeClient(IRestClient client) : base(client)
        {
        }

        public async Task<ChannelTypeWithStringCommandsResponse> CreateChannelTypeAsync(ChannelTypeWithStringCommandsRequest channelType)
        {
            if (channelType.Commands == null || channelType.Commands.Count == 0)
            {
                channelType.Commands = new List<string> { Commands.All };
            }

            return await ExecuteRequestAsync<ChannelTypeWithStringCommandsResponse>("channeltypes", HttpMethod.POST, HttpStatusCode.Created, channelType);
        }

        public async Task<ChannelTypeWithCommandsResponse> GetChannelTypeAsync(string type)
            => await ExecuteRequestAsync<ChannelTypeWithCommandsResponse>($"channeltypes/{type}",
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<ListChannelTypesResponse> ListChannelTypesAsync()
            => await ExecuteRequestAsync<ListChannelTypesResponse>("channeltypes",
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<ChannelTypeWithStringCommandsResponse> UpdateChannelTypeAsync(string type, ChannelTypeWithStringCommandsRequest updateReq)
            => await ExecuteRequestAsync<ChannelTypeWithStringCommandsResponse>($"channeltypes/{type}",
                HttpMethod.PUT,
                HttpStatusCode.Created,
                updateReq);

        public async Task<ApiResponse> DeleteChannelTypeAsync(string type)
            => await ExecuteRequestAsync<ApiResponse>($"channeltypes/{type}",
                HttpMethod.DELETE,
                HttpStatusCode.OK);
    }
}

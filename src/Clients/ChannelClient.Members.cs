using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public partial class ChannelClient
    {
        public async Task<ApiResponse> AddMembersAsync(string channelType, string channelId, params string[] userIds)
            => await AddMembersAsync(channelType, channelId, userIds, null, null);

        public async Task<ApiResponse> AddMembersAsync(string channelType, string channelId, IEnumerable<string> userIds, MessageRequest msg, AddMemberOptions options)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new ChannelUpdateRequest
                {
                    AddMembers = userIds,
                    HideHistory = options?.HideHistory,
                    Message = msg,
                });

        public async Task<ApiResponse> RemoveMembersAsync(string channelType, string channelId, IEnumerable<string> userIds, MessageRequest msg = null)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new ChannelUpdateRequest
                {
                    RemoveMembers = userIds,
                    Message = msg,
                });

        public async Task<QueryMembersResponse> QueryMembersAsync(QueryMembersRequest queryMembersRequest)
            => await ExecuteRequestAsync<QueryMembersResponse>("members",
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: queryMembersRequest.ToQueryParameters());

        public async Task<UpdateChannelResponse> AssignRolesAsync(string channelType, string channelId, AssignRoleRequest roleRequest)
            => await ExecuteRequestAsync<UpdateChannelResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.POST,
                HttpStatusCode.Created,
                roleRequest);
    }
}
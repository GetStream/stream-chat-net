using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public partial class ChannelClient
    {
        public async Task<UpdateChannelResponse> AddMembersAsync(string channelType, string channelId, params string[] userIds)
            => await AddMembersAsync(channelType, channelId, userIds, null, null);

        public async Task<UpdateChannelResponse> AddMembersAsync(string channelType, string channelId, string[] userIds, CancellationToken cancellationToken = default)
            => await AddMembersAsync(channelType, channelId, userIds, null, null, cancellationToken);

        public async Task<UpdateChannelResponse> AddMembersAsync(string channelType, string channelId, IEnumerable<string> userIds, MessageRequest msg, AddMemberOptions options, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<UpdateChannelResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new ChannelUpdateRequest
                {
                    AddMembers = userIds,
                    HideHistory = options?.HideHistory,
                    HideHistoryBefore = options?.HideHistoryBefore,
                    Message = msg,
                },
                cancellationToken: cancellationToken);

        public async Task<ApiResponse> RemoveMembersAsync(string channelType, string channelId, IEnumerable<string> userIds, MessageRequest msg = null, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new ChannelUpdateRequest
                {
                    RemoveMembers = userIds,
                    Message = msg,
                },
                cancellationToken: cancellationToken);

        public async Task<ChannelMemberResponse> UpdateMemberPartialAsync(string channelType, string channelId, ChannelMemberPartialRequest channelMemberPartialRequest, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<ChannelMemberResponse>($"channels/{channelType}/{channelId}/member/{channelMemberPartialRequest.UserId}",
                HttpMethod.PATCH,
                HttpStatusCode.OK,
                channelMemberPartialRequest,
                cancellationToken: cancellationToken);

        public async Task<QueryMembersResponse> QueryMembersAsync(QueryMembersRequest queryMembersRequest, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<QueryMembersResponse>("members",
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: queryMembersRequest.ToQueryParameters(),
                cancellationToken: cancellationToken);

        public async Task<UpdateChannelResponse> AssignRolesAsync(string channelType, string channelId, AssignRoleRequest roleRequest, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<UpdateChannelResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.POST,
                HttpStatusCode.Created,
                roleRequest,
                cancellationToken: cancellationToken);

        public async Task<UpdateChannelResponse> InviteAsync(string channelType, string channelId, string userId, MessageRequest msg = null, CancellationToken cancellationToken = default)
            => await InviteAsync(channelType, channelId, new[] { userId }, msg, cancellationToken);

        public async Task<UpdateChannelResponse> InviteAsync(string channelType, string channelId, IEnumerable<string> userIds, CancellationToken cancellationToken = default)
            => await InviteAsync(channelType, channelId, userIds, null, cancellationToken);

        public async Task<UpdateChannelResponse> InviteAsync(string channelType, string channelId, IEnumerable<string> userIds, MessageRequest msg = null, CancellationToken cancellationToken = default)
            => await UpdateAsync(channelType, channelId, new ChannelUpdateRequest
            {
                Invites = userIds,
                Message = msg,
            }, cancellationToken);

        public async Task<UpdateChannelResponse> AcceptInviteAsync(string channelType, string channelId, string userId, CancellationToken cancellationToken = default)
            => await UpdateAsync(channelType, channelId, new ChannelUpdateRequest
            {
                AcceptInvite = true,
                UserId = userId,
            }, cancellationToken);

        public async Task<UpdateChannelResponse> RejectInviteAsync(string channelType, string channelId, string userId, CancellationToken cancellationToken = default)
            => await UpdateAsync(channelType, channelId, new ChannelUpdateRequest
            {
                RejectInvite = true,
                UserId = userId,
            }, cancellationToken);
    }
}
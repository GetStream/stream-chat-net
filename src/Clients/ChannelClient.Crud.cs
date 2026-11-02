using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public partial class ChannelClient
    {
        public async Task<ChannelGetResponse> GetOrCreateAsync(string channelType, string channelId, string createdBy,
            params string[] members)
            => await GetOrCreateAsync(channelType,
                channelId,
                new ChannelGetRequest
                {
                    Data = new ChannelRequest
                    {
                        CreatedBy = new UserRequest { Id = createdBy },
                        Members = members.Select(x => new ChannelMember { UserId = x }),
                    },
                });

        public async Task<ChannelGetResponse> GetOrCreateAsync(string channelType, string channelId, string createdBy,
            string[] members, CancellationToken cancellationToken = default)
            => await GetOrCreateAsync(channelType,
                channelId,
                new ChannelGetRequest
                {
                    Data = new ChannelRequest
                    {
                        CreatedBy = new UserRequest { Id = createdBy },
                        Members = members.Select(x => new ChannelMember { UserId = x }),
                    },
                },
                cancellationToken: cancellationToken);

        public async Task<ChannelGetResponse> GetOrCreateAsync(string channelType, string channelId,
            ChannelGetRequest channelRequest, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<ChannelGetResponse>($"channels/{channelType}/{channelId}/query",
                HttpMethod.POST,
                HttpStatusCode.Created,
                channelRequest,
                cancellationToken: cancellationToken);

        public async Task<ChannelGetResponse> GetOrCreateAsync(string channelType, ChannelGetRequest channelRequest, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<ChannelGetResponse>($"channels/{channelType}/query",
                HttpMethod.POST,
                HttpStatusCode.Created,
                channelRequest,
                cancellationToken: cancellationToken);

        public async Task<UpdateChannelResponse> UpdateAsync(string channelType, string channelId,
            ChannelUpdateRequest updateRequest, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<UpdateChannelResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.POST,
                HttpStatusCode.Created,
                updateRequest,
                cancellationToken: cancellationToken);

        public async Task<PartialUpdateChannelResponse> PartialUpdateAsync(string channelType, string channelId,
            PartialUpdateChannelRequest partialUpdateChannelRequest, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<PartialUpdateChannelResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.PATCH,
                HttpStatusCode.OK,
                partialUpdateChannelRequest,
                cancellationToken: cancellationToken);

        public async Task<ApiResponse> DeleteAsync(string channelType, string channelId, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.DELETE,
                HttpStatusCode.OK,
                cancellationToken: cancellationToken);

        public async Task<TruncateResponse> TruncateAsync(string channelType, string channelId, CancellationToken cancellationToken = default)
            => await TruncateAsync(channelType, channelId, null, cancellationToken);

        public async Task<TruncateResponse> TruncateAsync(string channelType, string channelId,
            TruncateOptions truncateOptions, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<TruncateResponse>($"channels/{channelType}/{channelId}/truncate",
                HttpMethod.POST,
                HttpStatusCode.Created,
                truncateOptions,
                cancellationToken: cancellationToken);

        public Task<ChannelMemberResponse> PinAsync(string channelType, string channelId, string userId, CancellationToken cancellationToken = default)
            => UpdateMemberPartialAsync(channelType, channelId, new ChannelMemberPartialRequest
            {
                UserId = userId,
                Set = new Dictionary<string, object>()
                {
                    { "pinned", true },
                },
            }, cancellationToken);

        public Task<ChannelMemberResponse> UnpinAsync(string channelType, string channelId, string userId, CancellationToken cancellationToken = default)
            => UpdateMemberPartialAsync(channelType, channelId, new ChannelMemberPartialRequest
            {
                UserId = userId,
                Set = new Dictionary<string, object>()
                {
                    { "pinned", false },
                },
            }, cancellationToken);

        public Task<ChannelMemberResponse> ArchiveAsync(string channelType, string channelId, string userId, CancellationToken cancellationToken = default)
            => UpdateMemberPartialAsync(channelType, channelId, new ChannelMemberPartialRequest
            {
                UserId = userId,
                Set = new Dictionary<string, object>()
                {
                    { "archived", true },
                },
            }, cancellationToken);

        public Task<ChannelMemberResponse> UnarchiveAsync(string channelType, string channelId, string userId, CancellationToken cancellationToken = default)
            => UpdateMemberPartialAsync(channelType, channelId, new ChannelMemberPartialRequest
            {
                UserId = userId,
                Set = new Dictionary<string, object>()
                {
                    { "archived", false },
                },
            }, cancellationToken);
    }
}
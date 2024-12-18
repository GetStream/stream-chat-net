using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public async Task<ChannelGetResponse> GetOrCreateAsync(string channelType, string channelId,
            ChannelGetRequest channelRequest)
            => await ExecuteRequestAsync<ChannelGetResponse>($"channels/{channelType}/{channelId}/query",
                HttpMethod.POST,
                HttpStatusCode.Created,
                channelRequest);

        public async Task<ChannelGetResponse> GetOrCreateAsync(string channelType, ChannelGetRequest channelRequest)
            => await ExecuteRequestAsync<ChannelGetResponse>($"channels/{channelType}/query",
                HttpMethod.POST,
                HttpStatusCode.Created,
                channelRequest);

        public async Task<UpdateChannelResponse> UpdateAsync(string channelType, string channelId,
            ChannelUpdateRequest updateRequest)
            => await ExecuteRequestAsync<UpdateChannelResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.POST,
                HttpStatusCode.Created,
                updateRequest);

        public async Task<PartialUpdateChannelResponse> PartialUpdateAsync(string channelType, string channelId,
            PartialUpdateChannelRequest partialUpdateChannelRequest)
            => await ExecuteRequestAsync<PartialUpdateChannelResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.PATCH,
                HttpStatusCode.OK,
                partialUpdateChannelRequest);

        public async Task<ApiResponse> DeleteAsync(string channelType, string channelId)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.DELETE,
                HttpStatusCode.OK);

        public async Task<TruncateResponse> TruncateAsync(string channelType, string channelId)
            => await TruncateAsync(channelType, channelId, null);

        public async Task<TruncateResponse> TruncateAsync(string channelType, string channelId,
            TruncateOptions truncateOptions)
            => await ExecuteRequestAsync<TruncateResponse>($"channels/{channelType}/{channelId}/truncate",
                HttpMethod.POST,
                HttpStatusCode.Created,
                truncateOptions);

        public Task<ChannelMemberResponse> PinAsync(string channelType, string channelId, string userId)
            => UpdateMemberPartialAsync(channelType, channelId, new ChannelMemberPartialRequest
            {
                UserId = userId, Set = new Dictionary<string, object>()
                {
                    { "pinned", true },
                },
            });

        public Task<ChannelMemberResponse> UnpinAsync(string channelType, string channelId, string userId)
            => UpdateMemberPartialAsync(channelType, channelId, new ChannelMemberPartialRequest
            {
                UserId = userId, Set = new Dictionary<string, object>()
                {
                    { "pinned", false },
                },
            });

        public Task<ChannelMemberResponse> ArchiveAsync(string channelType, string channelId, string userId)
            => UpdateMemberPartialAsync(channelType, channelId, new ChannelMemberPartialRequest
            {
                UserId = userId, Set = new Dictionary<string, object>()
                {
                    { "archived", true },
                },
            });

        public Task<ChannelMemberResponse> UnarchiveAsync(string channelType, string channelId, string userId)
            => UpdateMemberPartialAsync(channelType, channelId, new ChannelMemberPartialRequest
            {
                UserId = userId, Set = new Dictionary<string, object>()
                {
                    { "archived", false },
                },
            });
    }
}
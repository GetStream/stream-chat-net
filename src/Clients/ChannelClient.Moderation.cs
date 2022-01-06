using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public partial class ChannelClient
    {
        public async Task<ApiResponse> AddModeratorsAsync(string channelType, string channelId, IEnumerable<string> userIds)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new { add_moderators = userIds });

        public async Task<ApiResponse> DemoteModeratorsAsync(string channelType, string channelId, IEnumerable<string> userIds)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new { demote_moderators = userIds });

        public async Task<ChannelMuteResponse> MuteChannelAsync(ChannelMuteRequest request)
            => await ExecuteRequestAsync<ChannelMuteResponse>("moderation/mute/channel",
                HttpMethod.POST,
                HttpStatusCode.Created,
                request);

        public async Task<ChannelUnmuteResponse> UnmuteChannelAsync(ChannelUnmuteRequest request)
            => await ExecuteRequestAsync<ChannelUnmuteResponse>("moderation/unmute/channel",
                HttpMethod.POST,
                HttpStatusCode.Created,
                request);
    }
}
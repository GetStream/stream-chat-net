using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    /// <summary>
    /// Convenience methods for updating a channel's filter tags.
    /// </summary>
    public partial class ChannelClient
    {
        /// <summary>
        /// Adds one or more filter tags to the channel.
        /// </summary>
        public Task<UpdateChannelResponse> AddFilterTagsAsync(string channelType, string channelId,
            params string[] filterTags) => AddFilterTagsAsync(channelType, channelId, (IEnumerable<string>)filterTags);

        /// <summary>
        /// Adds filter tags to the channel.
        /// </summary>
        public async Task<UpdateChannelResponse> AddFilterTagsAsync(string channelType, string channelId,
            IEnumerable<string> filterTags, MessageRequest msg = null) => await ExecuteRequestAsync<UpdateChannelResponse>(
            $"channels/{channelType}/{channelId}",
            HttpMethod.POST,
            HttpStatusCode.Created,
            new ChannelUpdateRequest
            {
                AddFilterTags = filterTags,
                Message = msg,
            });

        /// <summary>
        /// Removes one or more filter tags from the channel.
        /// </summary>
        public Task<ApiResponse> RemoveFilterTagsAsync(string channelType, string channelId,
            params string[] filterTags) => RemoveFilterTagsAsync(channelType, channelId, (IEnumerable<string>)filterTags);

        /// <summary>
        /// Removes filter tags from the channel.
        /// </summary>
        public async Task<ApiResponse> RemoveFilterTagsAsync(string channelType, string channelId,
            IEnumerable<string> filterTags, MessageRequest msg = null) => await ExecuteRequestAsync<ApiResponse>(
            $"channels/{channelType}/{channelId}",
            HttpMethod.POST,
            HttpStatusCode.Created,
            new ChannelUpdateRequest
            {
                RemoveFilterTags = filterTags,
                Message = msg,
            });
    }
}

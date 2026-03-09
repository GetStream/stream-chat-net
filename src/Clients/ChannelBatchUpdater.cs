using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// Provides convenience methods for batch channel operations.
    /// </summary>
    public class ChannelBatchUpdater
    {
        private readonly IChannelClient _client;

        public ChannelBatchUpdater(IChannelClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Adds members to channels matching the filter.
        /// </summary>
        public async Task<AsyncOperationResponse> AddMembersAsync(ChannelsBatchFilters filter, IEnumerable<ChannelBatchMemberRequest> members)
        {
            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.AddMembers,
                Filter = filter,
                Members = members?.ToList(),
            };
            return await _client.UpdateChannelsBatchAsync(options);
        }

        /// <summary>
        /// Removes members from channels matching the filter.
        /// </summary>
        public async Task<AsyncOperationResponse> RemoveMembersAsync(ChannelsBatchFilters filter, IEnumerable<ChannelBatchMemberRequest> members)
        {
            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.RemoveMembers,
                Filter = filter,
                Members = members?.ToList(),
            };
            return await _client.UpdateChannelsBatchAsync(options);
        }

        /// <summary>
        /// Invites members to channels matching the filter.
        /// </summary>
        public async Task<AsyncOperationResponse> InviteMembersAsync(ChannelsBatchFilters filter, IEnumerable<ChannelBatchMemberRequest> members)
        {
            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.InviteMembers,
                Filter = filter,
                Members = members?.ToList(),
            };
            return await _client.UpdateChannelsBatchAsync(options);
        }

        /// <summary>
        /// Assigns roles to members in channels matching the filter.
        /// </summary>
        public async Task<AsyncOperationResponse> AssignRolesAsync(ChannelsBatchFilters filter, IEnumerable<ChannelBatchMemberRequest> members)
        {
            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.AssignRoles,
                Filter = filter,
                Members = members?.ToList(),
            };
            return await _client.UpdateChannelsBatchAsync(options);
        }

        /// <summary>
        /// Adds moderators to channels matching the filter.
        /// </summary>
        public async Task<AsyncOperationResponse> AddModeratorsAsync(ChannelsBatchFilters filter, IEnumerable<ChannelBatchMemberRequest> members)
        {
            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.AddModerators,
                Filter = filter,
                Members = members?.ToList(),
            };
            return await _client.UpdateChannelsBatchAsync(options);
        }

        /// <summary>
        /// Removes moderator role from members in channels matching the filter.
        /// </summary>
        public async Task<AsyncOperationResponse> DemoteModeratorsAsync(ChannelsBatchFilters filter, IEnumerable<ChannelBatchMemberRequest> members)
        {
            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.DemoteModerators,
                Filter = filter,
                Members = members?.ToList(),
            };
            return await _client.UpdateChannelsBatchAsync(options);
        }

        /// <summary>
        /// Hides channels matching the filter for the specified members.
        /// </summary>
        public async Task<AsyncOperationResponse> HideAsync(ChannelsBatchFilters filter, IEnumerable<ChannelBatchMemberRequest> members)
        {
            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.Hide,
                Filter = filter,
                Members = members?.ToList(),
            };
            return await _client.UpdateChannelsBatchAsync(options);
        }

        /// <summary>
        /// Shows channels matching the filter for the specified members.
        /// </summary>
        public async Task<AsyncOperationResponse> ShowAsync(ChannelsBatchFilters filter, IEnumerable<ChannelBatchMemberRequest> members)
        {
            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.Show,
                Filter = filter,
                Members = members?.ToList(),
            };
            return await _client.UpdateChannelsBatchAsync(options);
        }

        /// <summary>
        /// Archives channels matching the filter for the specified members.
        /// </summary>
        public async Task<AsyncOperationResponse> ArchiveAsync(ChannelsBatchFilters filter, IEnumerable<ChannelBatchMemberRequest> members)
        {
            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.Archive,
                Filter = filter,
                Members = members?.ToList(),
            };
            return await _client.UpdateChannelsBatchAsync(options);
        }

        /// <summary>
        /// Unarchives channels matching the filter for the specified members.
        /// </summary>
        public async Task<AsyncOperationResponse> UnarchiveAsync(ChannelsBatchFilters filter, IEnumerable<ChannelBatchMemberRequest> members)
        {
            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.Unarchive,
                Filter = filter,
                Members = members?.ToList(),
            };
            return await _client.UpdateChannelsBatchAsync(options);
        }

        /// <summary>
        /// Updates data on channels matching the filter.
        /// </summary>
        public async Task<AsyncOperationResponse> UpdateDataAsync(ChannelsBatchFilters filter, ChannelDataUpdate data)
        {
            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.UpdateData,
                Filter = filter,
                Data = data,
            };
            return await _client.UpdateChannelsBatchAsync(options);
        }

    }
}

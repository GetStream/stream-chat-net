using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// ChannelBatchUpdater - A class that provides convenience methods for batch channel operations
    /// </summary>
    public class ChannelBatchUpdater
    {
        private readonly ChannelClient _client;

        internal ChannelBatchUpdater(ChannelClient client)
        {
            _client = client;
        }

        // Member operations

        /// <summary>
        /// addMembers - Add members to channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <param name="members">Members to add (list of user IDs or list of member objects with user_id and channel_role)</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> AddMembersAsync(
            Dictionary<string, object> filter, object members)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "addMembers",
                Filter = filter,
                Members = members,
            });
        }

        /// <summary>
        /// removeMembers - Remove members from channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <param name="members">Member IDs to remove</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> RemoveMembersAsync(
            Dictionary<string, object> filter, List<string> members)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "removeMembers",
                Filter = filter,
                Members = members,
            });
        }

        /// <summary>
        /// inviteMembers - Invite members to channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <param name="members">Members to invite (list of user IDs or list of member objects with user_id and channel_role)</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> InviteMembersAsync(
            Dictionary<string, object> filter, object members)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "invites",
                Filter = filter,
                Members = members,
            });
        }

        /// <summary>
        /// addModerators - Add moderators to channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <param name="members">Member IDs to promote to moderator</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> AddModeratorsAsync(
            Dictionary<string, object> filter, List<string> members)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "addModerators",
                Filter = filter,
                Members = members,
            });
        }

        /// <summary>
        /// demoteModerators - Remove moderator role from members in channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <param name="members">Member IDs to demote</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> DemoteModeratorsAsync(
            Dictionary<string, object> filter, List<string> members)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "demoteModerators",
                Filter = filter,
                Members = members,
            });
        }

        /// <summary>
        /// assignRoles - Assign roles to members in channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <param name="members">Members with role assignments (list of objects with user_id and channel_role)</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> AssignRolesAsync(
            Dictionary<string, object> filter, List<Dictionary<string, object>> members)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "assignRoles",
                Filter = filter,
                Members = members,
            });
        }

        // Visibility operations

        /// <summary>
        /// hide - Hide channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> HideAsync(Dictionary<string, object> filter)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "hide",
                Filter = filter,
            });
        }

        /// <summary>
        /// show - Show channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> ShowAsync(Dictionary<string, object> filter)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "show",
                Filter = filter,
            });
        }

        /// <summary>
        /// archive - Archive channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> ArchiveAsync(Dictionary<string, object> filter)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "archive",
                Filter = filter,
            });
        }

        /// <summary>
        /// unarchive - Unarchive channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> UnarchiveAsync(Dictionary<string, object> filter)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "unarchive",
                Filter = filter,
            });
        }

        // Data operations

        /// <summary>
        /// updateData - Update data on channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <param name="data">Data to update (frozen, disabled, custom, team, config_overrides, auto_translation_enabled, auto_translation_language)</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> UpdateDataAsync(
            Dictionary<string, object> filter, Dictionary<string, object> data)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "updateData",
                Filter = filter,
                Data = data,
            });
        }

        /// <summary>
        /// addFilterTags - Add filter tags to channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <param name="tags">Tags to add</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> AddFilterTagsAsync(
            Dictionary<string, object> filter, List<string> tags)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "addFilterTags",
                Filter = filter,
                FilterTagsUpdate = tags,
            });
        }

        /// <summary>
        /// removeFilterTags - Remove filter tags from channels matching the filter
        /// </summary>
        /// <param name="filter">Filter to select channels</param>
        /// <param name="tags">Tags to remove</param>
        /// <returns>The server response</returns>
        public async Task<UpdateChannelsBatchResponse> RemoveFilterTagsAsync(
            Dictionary<string, object> filter, List<string> tags)
        {
            return await _client.UpdateChannelsBatchAsync(new UpdateChannelsBatchOptions
            {
                Operation = "removeFilterTags",
                Filter = filter,
                FilterTagsUpdate = tags,
            });
        }
    }
}


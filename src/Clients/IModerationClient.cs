using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to access moderation endpoints of a Stream Chat application.
    /// </summary>
    public interface IModerationClient
    {
        /// <summary>
        /// Check content for moderation.
        /// </summary>
        /// <param name="entityType">Type of entity to be checked E.g., stream:user, stream:chat:v1:message, or any custom string. Predefined values are listed in <see cref="ModerationEntityTypes"/></param>
        /// <param name="entityId">ID of the entity to be checked. This is mainly for tracking purposes</param>
        /// <param name="entityCreatorId">ID of the entity creator</param>
        /// <param name="moderationPayload">Content to be checked for moderation. E.g., texts, images, videos</param>
        /// <param name="configKey">Configuration key for moderation</param>
        /// <param name="options">Additional options for moderation check</param>
        Task<ModerationCheckResponse> CheckAsync(
            string entityType,
            string entityId,
            string entityCreatorId,
            ModerationPayload moderationPayload,
            string configKey,
            ModerationCheckOptions options = null);

        /// <summary>
        /// Experimental: Check user profile for moderation.
        /// This will not create any review queue items for the user profile.
        /// You can use this to check whether to allow certain user profile to be created or not.
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="profile">User profile data containing username and/or profile image</param>
        Task<ModerationCheckResponse> CheckUserProfileAsync(string userId, UserProfileCheckRequest profile);
    }
}
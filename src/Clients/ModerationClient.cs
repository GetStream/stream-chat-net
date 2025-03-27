using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class ModerationClient : ClientBase, IModerationClient
    {
        internal ModerationClient(IRestClient client) : base(client)
        {
        }

        public async Task<ModerationCheckResponse> CheckAsync(
            string entityType,
            string entityId,
            string entityCreatorId,
            ModerationPayload moderationPayload,
            string configKey,
            ModerationCheckOptions options = null)
        {
            var request = new
            {
                entity_type = entityType,
                entity_id = entityId,
                entity_creator_id = entityCreatorId,
                moderation_payload = moderationPayload,
                config_key = configKey,
                options,
            };

            return await ExecuteRequestAsync<ModerationCheckResponse>(
                "api/v2/moderation/check",
                HttpMethod.POST,
                HttpStatusCode.Created,
                request);
        }

        public async Task<ModerationCheckResponse> CheckUserProfileAsync(string userId, UserProfileCheckRequest profile)
        {
            if (string.IsNullOrEmpty(profile?.Username) && string.IsNullOrEmpty(profile?.Image))
            {
                throw new ArgumentException("Either username or image must be provided", nameof(profile));
            }

            var payload = new ModerationPayload
            {
                Texts = !string.IsNullOrEmpty(profile.Username) ? new[] { profile.Username }.ToList() : null,
                Images = !string.IsNullOrEmpty(profile.Image) ? new[] { profile.Image }.ToList() : null,
            };

            return await CheckAsync(
                ModerationEntityTypes.UserProfile,
                userId,
                userId,
                payload,
                "user_profile:default",
                new ModerationCheckOptions
                {
                    ForceSync = true,
                    TestMode = true,
                });
        }
    }
}
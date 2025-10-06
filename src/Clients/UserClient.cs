using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;
using StreamChat.Utils;

namespace StreamChat.Clients
{
    public class UserClient : ClientBase, IUserClient
    {
        private static readonly DateTimeOffset _epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private readonly IJwtGeneratorClient _jwtClient;
        private readonly string _apiSecret;

        internal UserClient(IRestClient client, IJwtGeneratorClient jwtClient, string apiSecret) : base(client)
        {
            _jwtClient = jwtClient;
            _apiSecret = apiSecret;
        }

        private static string Endpoint(string id = null) => id == null ? "users" : $"users/{id}";

        public string CreateToken(string userId) => CreateToken(userId, null, null);

        public string CreateToken(string userId, DateTimeOffset? expiration = null, DateTimeOffset? issuedAt = null)
        {
            var payload = new Dictionary<string, object> { { "user_id", userId } };

            if (expiration.HasValue)
                payload["exp"] = (int)expiration.Value.Subtract(_epoch).TotalSeconds;

            if (issuedAt.HasValue)
                payload["iat"] = (int)issuedAt.Value.Subtract(_epoch).TotalSeconds;

            return _jwtClient.GenerateJwt(payload, _apiSecret);
        }

        public async Task<UpsertResponse> UpsertManyAsync(IEnumerable<UserRequest> users)
            => await ExecuteRequestAsync<UpsertResponse>(Endpoint(),
                HttpMethod.POST,
                HttpStatusCode.Created,
                body: new
                {
                    users = users.ToDictionary(u => u.Id, u => u),
                });

        public async Task<UpsertResponse> UpsertAsync(UserRequest user)
            => await UpsertManyAsync(new[] { user });

        public async Task<CreateGuestResponse> CreateGuestAsync(UserRequest user)
            => await ExecuteRequestAsync<CreateGuestResponse>("guest",
                HttpMethod.POST,
                HttpStatusCode.Created,
                body: new { user = user });

        public async Task<UpsertResponse> UpdateManyPartialAsync(IEnumerable<UserPartialRequest> updates)
            => await ExecuteRequestAsync<UpsertResponse>(Endpoint(),
                HttpMethod.PATCH,
                HttpStatusCode.OK,
                new { users = updates });

        public async Task<UpsertResponse> UpdatePartialAsync(UserPartialRequest update)
            => await UpdateManyPartialAsync(new[] { update });

        public async Task<GenericUserResponse> DeleteAsync(string id, bool markMessagesDeleted = false, bool hardDelete = false, bool deleteConversations = false)
            => await ExecuteRequestAsync<GenericUserResponse>(Endpoint(id),
                HttpMethod.DELETE,
                HttpStatusCode.OK,
                body: null,
                queryParams: new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("mark_messages_deleted", markMessagesDeleted.ToString().ToLowerInvariant()),
                    new KeyValuePair<string, string>("hard_delete", hardDelete.ToString().ToLowerInvariant()),
                    new KeyValuePair<string, string>("delete_conversation_channels", deleteConversations.ToString().ToLowerInvariant()),
                });

        public async Task<GenericTaskIdResponse> DeleteManyAsync(DeleteUsersRequest req)
            => await ExecuteRequestAsync<GenericTaskIdResponse>(Endpoint("delete"),
                HttpMethod.POST,
                HttpStatusCode.Created,
                req);

        public async Task<GenericUserResponse> DeactivateAsync(string id, bool markMessagesDeleted = false, string createdById = null)
            => await ExecuteRequestAsync<GenericUserResponse>(Endpoint(id) + "/deactivate",
                HttpMethod.POST,
                HttpStatusCode.Created,
                body: new
                {
                    mark_messages_deleted = markMessagesDeleted,
                    created_by_id = createdById,
                });

        public async Task<GenericUserResponse> ReactivateAsync(string id, bool restoreMessages = false, string name = null, string createdById = null)
            => await ExecuteRequestAsync<GenericUserResponse>(Endpoint(id) + "/reactivate",
                HttpMethod.POST,
                HttpStatusCode.Created,
                body: new
                {
                    restore_messages = restoreMessages,
                    name = name,
                    created_by_id = createdById,
                });

        public async Task<ExportedUser> ExportAsync(string userId)
            => await ExecuteRequestAsync<ExportedUser>(Endpoint(userId) + "/export",
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<GenericTaskIdResponse> ExportUsersAsync(IEnumerable<string> userIds)
            => await ExecuteRequestAsync<GenericTaskIdResponse>("export/users",
                HttpMethod.POST,
                HttpStatusCode.Created,
                body: new
                {
                    user_ids = userIds,
                });

        public async Task<ApiResponse> ShadowBanAsync(ShadowBanRequest shadowBanRequest)
            => await BanAsync(shadowBanRequest.ToBanRequest());

        public async Task<ApiResponse> RemoveShadowBanAsync(ShadowBanRequest shadowBanRequest)
            => await UnbanAsync(shadowBanRequest.ToBanRequest());

        public async Task<ApiResponse> BanAsync(BanRequest banRequest)
            => await ExecuteRequestAsync<ApiResponse>("moderation/ban",
                HttpMethod.POST,
                HttpStatusCode.Created,
                banRequest);

        public async Task<ApiResponse> UnbanAsync(BanRequest banRequest)
            => await ExecuteRequestAsync<ApiResponse>("moderation/ban",
                HttpMethod.DELETE,
                HttpStatusCode.OK,
                queryParams: new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("target_user_id", banRequest.TargetUserId),
                    new KeyValuePair<string, string>("type", banRequest.Type),
                    new KeyValuePair<string, string>("id", banRequest.Id),
                });
        public async Task<QueryBannedUsersResponse> QueryBannedUsersAsync(QueryBannedUsersRequest request)
            => await ExecuteRequestAsync<QueryBannedUsersResponse>("query_banned_users",
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: request.ToQueryParameters());

        public async Task<MuteResponse> MuteAsync(string targetId, string id)
            => await ExecuteRequestAsync<MuteResponse>("moderation/mute",
                    HttpMethod.POST,
                    HttpStatusCode.Created,
                    body: new
                    {
                        target_id = targetId,
                        user_id = id,
                    });

        public async Task<BlockUserResponse> BlockUserAsync(string targetId, string userID)
            => await ExecuteRequestAsync<BlockUserResponse>("users/block",
                    HttpMethod.POST,
                    HttpStatusCode.Created,
                    body: new
                    {
                        blocked_user_id = targetId,
                        user_id = userID,
                    });
        public async Task<ApiResponse> UnblockUserAsync(string targetId, string userID)
            => await ExecuteRequestAsync<BlockUserResponse>("users/unblock",
                    HttpMethod.POST,
                    HttpStatusCode.Created,
                    body: new
                    {
                        blocked_user_id = targetId,
                        user_id = userID,
                    });
        public async Task<GetBlockedUsersResponse> GetBlockedUsersAsync(string userID)
            => await ExecuteRequestAsync<GetBlockedUsersResponse>("users/block",
                    HttpMethod.GET,
                    HttpStatusCode.OK,
                    queryParams: new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("user_id", userID),
                    });

        public async Task<ApiResponse> UnmuteAsync(string targetId, string id)
            => await ExecuteRequestAsync<ApiResponse>("moderation/unmute",
                    HttpMethod.POST,
                    HttpStatusCode.Created,
                    body: new
                    {
                        target_id = targetId,
                        user_id = id,
                    });

        public async Task<ApiResponse> MarkAllReadAsync(string id)
            => await ExecuteRequestAsync<ApiResponse>("channels/read",
                HttpMethod.POST,
                HttpStatusCode.Created,
                body: new
                {
                    user = new UserRequest { Id = id },
                });

        public async Task<UpsertResponse> RevokeUserTokenAsync(string userId, DateTimeOffset? issuedBefore)
            => await UpdatePartialAsync(new UserPartialRequest
            {
                Id = userId,
                Set = new Dictionary<string, object>
                    {
                        { "revoke_tokens_issued_before", issuedBefore },
                    },
            });

        public async Task<UpsertResponse> RevokeManyUserTokensAsync(IEnumerable<string> userIds, DateTimeOffset? issuedBefore)
            => await UpdateManyPartialAsync(userIds.Select(id => new UserPartialRequest
            {
                Id = id,
                Set = new Dictionary<string, object>
                    {
                        { "revoke_tokens_issued_before", issuedBefore },
                    },
            }));

        public async Task<QueryUsersResponse> QueryAsync(QueryUserOptions opts)
        {
            var payload = new
            {
                offset = opts.Offset,
                limit = opts.Limit,
                presence = opts.Presence,
                filter_conditions = opts.Filter,
                sort = opts.Sort,
            };

            return await ExecuteRequestAsync<QueryUsersResponse>(Endpoint(),
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("payload", StreamJsonConverter.SerializeObject(payload)),
                });
        }

        public async Task<SharedLocationResponse> UpdateUserLiveLocationAsync(string userID, SharedLocationRequest location)
        {
            if (string.IsNullOrEmpty(userID))
                throw new ArgumentException("User ID cannot be empty", nameof(userID));

            return await ExecuteRequestAsync<SharedLocationResponse>("users/live_locations",
                HttpMethod.PUT,
                HttpStatusCode.Created,
                location,
                queryParams: new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user_id", userID),
                });
        }

        public async Task<ActiveLiveLocationsResponse> GetUserActiveLiveLocationsAsync(string userID)
        {
            if (string.IsNullOrEmpty(userID))
                throw new ArgumentException("User ID cannot be empty", nameof(userID));

            return await ExecuteRequestAsync<ActiveLiveLocationsResponse>("users/live_locations",
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user_id", userID),
                });
        }

        public async Task<EventResponse> MarkDeliveredAsync(MarkDeliveredOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.LatestDeliveredMessages == null || options.LatestDeliveredMessages.Count == 0)
                throw new ArgumentException("LatestDeliveredMessages must not be empty", nameof(options));

            if (options.User == null && string.IsNullOrEmpty(options.UserID))
                throw new ArgumentException("Either User or UserID must be provided", nameof(options));

            var queryParams = new List<KeyValuePair<string, string>>();
            if (options.User == null)
            {
                queryParams.Add(new KeyValuePair<string, string>("user_id", options.UserID));
            }
            else
            {
                queryParams.Add(new KeyValuePair<string, string>("user_id", options.User.ID));
            }

            return await ExecuteRequestAsync<EventResponse>("channels/delivered",
                HttpMethod.POST,
                HttpStatusCode.Created,
                options,
                queryParams: queryParams);
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamChat.Rest;

namespace StreamChat
{
    public class Users : IUsers
    {
        readonly Client _client;

        internal Users(Client client)
        {
            _client = client;
        }

        public async Task<IEnumerable<User>> UpsertMany(IEnumerable<User> users)
        {
            var usersDict = new JObject();
            users.ForEach(u => usersDict.Add(new JProperty(u.ID, u.ToJObject())));
            var payload = new JObject(new JProperty("users", usersDict));

            var request = this._client.BuildAppRequest(Users.Endpoint(), HttpMethod.POST);
            request.SetJsonBody(payload.ToString());

            var response = await this._client.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return Users.GetResults(response.Content);
            throw StreamChatException.FromResponse(response);
        }

        public async Task<User> Upsert(User user)
        {
            var users = await this.UpsertMany(new User[] { user });
            return users.FirstOrDefault();
        }

        public async Task<IEnumerable<User>> UpdateManyPartial(IEnumerable<UserPartialRequest> updates)
        {
            var updatesArr = new JArray();
            updates.ForEach(u => updatesArr.Add(u.ToJObject()));

            var payload = new JObject(new JProperty("users", updatesArr));

            var request = this._client.BuildAppRequest(Users.Endpoint(), HttpMethod.PATCH);
            request.SetJsonBody(payload.ToString());
            var response = await this._client.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Users.GetResults(response.Content);

            throw StreamChatException.FromResponse(response);
        }

        public async Task<User> UpdatePartial(UserPartialRequest update)
        {
            var user = await this.UpdateManyPartial(new UserPartialRequest[] { update });
            return user.FirstOrDefault();
        }

        public async Task<User> Delete(string id, bool markMessagesDeleted = false, bool hardDelete = false, bool deleteConversations = false)
        {
            var request = this._client.BuildAppRequest(Users.Endpoint(id), HttpMethod.DELETE);
            request.AddQueryParameter("mark_messages_deleted", markMessagesDeleted.ToString().ToLower());
            request.AddQueryParameter("hard_delete", hardDelete.ToString().ToLower());
            request.AddQueryParameter("delete_conversation_channels", deleteConversations.ToString().ToLower());

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Users.GetUserFromResponse(response.Content);

            throw StreamChatException.FromResponse(response);
        }

        public async Task<User> Deactivate(string id, bool markMessagesDeleted = false, string createdById = "")
        {
            var request = this._client.BuildAppRequest(Users.Endpoint(id) + "/deactivate", HttpMethod.POST);
            var payload = new
            {
                mark_messages_deleted = markMessagesDeleted,
                created_by_id = createdById,
            };
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return Users.GetUserFromResponse(response.Content);

            throw StreamChatException.FromResponse(response);
        }

        public async Task<User> Reactivate(string id, bool restoreMessages = false, string name = "", string createdById = "")
        {
            var request = this._client.BuildAppRequest(Users.Endpoint(id) + "/reactivate", HttpMethod.POST);
            var payload = new
            {
                restore_messages = restoreMessages,
                name = name,
                created_by_id = createdById,
            };
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return Users.GetUserFromResponse(response.Content);

            throw StreamChatException.FromResponse(response);
        }

        public async Task<ExportedUser> Export(string id)
        {
            var request = this._client.BuildAppRequest(Users.Endpoint(id) + "/export", HttpMethod.GET);
            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return ExportedUser.FromJObject(JObject.Parse(response.Content));

            throw StreamChatException.FromResponse(response);
        }

        public async Task Ban(string targetUserID, string id, string reason, int timeoutMinutes = 0)
        {
            await this.Ban(targetUserID, id, reason, null, timeoutMinutes);
        }

        public async Task Ban(string targetUserID, string id, string reason, Channel channel, int timeoutMinutes = 0)
        {
            var payload = new Dictionary<string, object>()
            {
                {"target_user_id", targetUserID},
                {"reason", reason},
                {"timeout", timeoutMinutes},
                {"user_id", id},
            };
            if (channel != null)
            {
                payload["type"] = channel.Type;
                payload["id"] = channel.ID;
            }
            var request = this._client.BuildAppRequest("moderation/ban", HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                throw StreamChatException.FromResponse(response);
        }

        public async Task Unban(string targetUserID, Channel channel = null)
        {
            var request = this._client.BuildAppRequest("moderation/ban", HttpMethod.DELETE);
            request.AddQueryParameter("target_user_id", targetUserID);
            if (channel != null)
            {
                request.AddQueryParameter("type", channel.Type);
                request.AddQueryParameter("id", channel.ID);
            }

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw StreamChatException.FromResponse(response);
        }

        public async Task<MuteResponse> Mute(string targetID, string id)
        {
            var payload = new
            {
                target_id = targetID,
                user_id = id
            };
            var request = this._client.BuildAppRequest("moderation/mute", HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return MuteResponse.FromJObject(JObject.Parse(response.Content));
            throw StreamChatException.FromResponse(response);
        }

        public async Task Unmute(string targetID, string id)
        {
            var payload = new
            {
                target_id = targetID,
                user_id = id
            };
            var request = this._client.BuildAppRequest("moderation/unmute", HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                throw StreamChatException.FromResponse(response);
        }

        public async Task MarkAllRead(string id)
        {
            var payload = new
            {
                user = new
                {
                    id = id
                }
            };
            var request = this._client.BuildAppRequest("channels/read", HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                throw StreamChatException.FromResponse(response);
        }

        public async Task<IEnumerable<User>> Query(QueryUserOptions opts)
        {
            var request = this._client.BuildAppRequest(Users.Endpoint(), HttpMethod.GET);
            opts.Apply(request);

            var response = await this._client.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Users.GetResults(response.Content);
            throw StreamChatException.FromResponse(response);
        }

        private static string Endpoint(string id = null)
        {
            return id == null ? "users" : string.Format("users/{0}", id);
        }

        private static IEnumerable<User> GetResults(string content)
        {
            var root = JObject.Parse(content);
            var usersProp = root.Property("users").Value;

            IEnumerable<JToken> users;
            if (usersProp.Type == JTokenType.Array)
            {
                users = usersProp as JArray;
            }
            else
            {
                var usersDict = usersProp as JObject;
                users = usersDict.Values();
            }
            foreach (var user in users)
            {
                var userObj = user as JObject;
                yield return User.FromJObject(userObj);
            }
        }

        private static User GetUserFromResponse(string content)
        {
            var root = JObject.Parse(content);
            var user = root.Property("user").Value as JObject;
            return User.FromJObject(user);
        }
    }
}

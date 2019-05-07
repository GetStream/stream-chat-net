using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamChat.Rest;

namespace StreamChat
{
    public class Users
    {
        readonly Client _client;

        internal Users(Client client)
        {
            _client = client;
        }

        public async Task<IEnumerable<User>> UpdateMany(IEnumerable<User> users)
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

        public async Task<User> Update(User user)
        {
            var users = await this.UpdateMany(new User[] { user });
            return users.FirstOrDefault();
        }

        public async Task<User> Delete(string id, bool markMessagesDeleted = false, bool hardDelete = false)
        {
            var request = this._client.BuildAppRequest(Users.Endpoint(id), HttpMethod.DELETE);
            request.AddQueryParameter("mark_messages_deleted", markMessagesDeleted.ToString().ToLower());
            request.AddQueryParameter("hard_delete", hardDelete.ToString().ToLower());

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Users.GetUserFromResponse(response.Content);

            throw StreamChatException.FromResponse(response);
        }

        public async Task<User> Deactivate(string id, bool markMessagesDeleted = false)
        {
            var request = this._client.BuildAppRequest(Users.Endpoint(id) + "/deactivate", HttpMethod.POST);
            var payload = new
            {
                mark_messages_deleted = markMessagesDeleted,
            };
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return Users.GetUserFromResponse(response.Content);

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

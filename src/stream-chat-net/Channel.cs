using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamChat.Rest;

namespace StreamChat
{
    public class Channel : CustomDataBase, IChannel
    {
        public string Type { get; internal set; }
        public string ID { get; internal set; }

        private string Endpoint
        {
            get
            {
                if (this.ID == null)
                    throw new ArgumentNullException("channel id", "Channel ID must be set");
                return string.Format("channels/{0}/{1}", this.Type, this.ID);
            }
        }

        readonly Client _client;

        internal Channel(Client client, string type, string id = "", GenericData data = null)
        {
            _client = client;
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException("channel type", "Channel type can't be empty");
            this.Type = type;
            this.ID = id;
            if (data != null)
                this._data = data;
        }

        public async Task<ChannelState> Create(string createdBy, IEnumerable<string> members = null)
        {
            var dummyUser = new User()
            {
                ID = createdBy
            };
            this.SetData("created_by", dummyUser);
            if (members != null)
                this.SetData("members", members);
            return await this.Query(new ChannelQueryParams());
        }

        public async Task<Message> SendMessage(MessageInput msg, string userID, bool skipPush = false)
        {
            if (msg.User == null)
            {
                msg.User = new User();
            }
            msg.User.ID = userID;

            var payload = new JObject(new JProperty("message", msg.ToJObject()));
            if (skipPush) 
            {
                payload.Add("skip_push", true);
            }


            var request = this._client.BuildAppRequest(this.Endpoint + "/message", HttpMethod.POST);
            request.SetJsonBody(payload.ToString());

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var respObj = JObject.Parse(response.Content);
                var msgObj = respObj.Property("message").Value as JObject;
                return Message.FromJObject(msgObj);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<Message> SendMessage(MessageInput msg, string userID, string parentID, bool skipPush = false)
        {
            msg.ParentID = parentID;
            return await this.SendMessage(msg, userID, skipPush);
        }

        public async Task<Event> SendEvent(Event evt, string userID)
        {
            if (evt.User == null)
            {
                evt.User = new User();
            }
            evt.User.ID = userID;

            var payload = new JObject(new JProperty("event", evt.ToJObject()));

            var request = this._client.BuildAppRequest(this.Endpoint + "/event", HttpMethod.POST);
            request.SetJsonBody(payload.ToString());

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var respObj = JObject.Parse(response.Content);
                var evtObj = respObj.Property("event").Value as JObject;
                return Event.FromJObject(evtObj);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<ReactionResponse> SendReaction(string messageID, Reaction reaction, string userID, bool skipPush = false)
        {
            if (reaction.User == null)
            {
                reaction.User = new User();
            }
            reaction.User.ID = userID;

            var payload = new JObject(new JProperty("reaction", reaction.ToJObject()));
            if (skipPush) 
            {
                payload.Add("skip_push", true);
            }

            var endpoint = string.Format("messages/{0}/reaction", messageID);
            var request = this._client.BuildAppRequest(endpoint, HttpMethod.POST);
            request.SetJsonBody(payload.ToString());

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var respObj = JObject.Parse(response.Content);
                return ReactionResponse.FromJObject(respObj);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<ReactionResponse> DeleteReaction(string messageID, string reactionType, string userID)
        {
            var endpoint = string.Format("messages/{0}/reaction/{1}", messageID, reactionType);
            var request = this._client.BuildAppRequest(endpoint, HttpMethod.DELETE);
            request.AddQueryParameter("user_id", userID);

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var respObj = JObject.Parse(response.Content);
                return ReactionResponse.FromJObject(respObj);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<List<Reaction>> GetReactions(string messageID, int offset = 0, int limit = 50)
        {
            var endpoint = string.Format("messages/{0}/reactions", messageID);
            var request = this._client.BuildAppRequest(endpoint, HttpMethod.GET);
            request.AddQueryParameter("offset", offset.ToString());
            request.AddQueryParameter("limit", limit.ToString());

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var respObj = JObject.Parse(response.Content);
                var reactionObjs = respObj.Property("reactions").Value as JArray;

                var result = new List<Reaction>();
                foreach (var reactionTok in reactionObjs)
                {
                    var r = reactionTok as JObject;
                    result.Add(Reaction.FromJObject(r));
                }
                return result;
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<ChannelState> Query(ChannelQueryParams queryParams)
        {
            var payload = JObject.FromObject(queryParams);
            payload.Add("data", this._data.ToJObject());

            string tpl = "channels/{0}{1}/query";
            string idStr = this.ID == null ? "" : "/" + this.ID;
            string endpoint = string.Format(tpl, this.Type, idStr);

            var request = this._client.BuildAppRequest(endpoint, HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var stateObj = JObject.Parse(response.Content);
                stateObj.Remove("duration");
                var chanState = ChannelState.FromJObject(stateObj);
                if (string.IsNullOrEmpty(this.ID))
                    this.ID = chanState.Channel.ID;
                return chanState;
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<UpdateChannelResponse> Update(GenericData customData, MessageInput msg = null, bool skipPush = false)
        {
            var payload = new JObject();
            payload.Add(new JProperty("data", customData.ToJObject()));
            if (msg != null)
            {
                payload.Add(new JProperty("message", msg.ToJObject()));

                if (skipPush) 
                    payload.Add("skip_push", true);
            }

            var request = this._client.BuildAppRequest(this.Endpoint, HttpMethod.POST);
            request.SetJsonBody(payload.ToString());

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var respObj = JObject.Parse(response.Content);
                return UpdateChannelResponse.FromJObject(respObj);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task Delete()
        {
            var request = this._client.BuildAppRequest(this.Endpoint, HttpMethod.DELETE);
            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw StreamChatException.FromResponse(response);
        }

        public async Task Truncate()
        {
            var request = this._client.BuildAppRequest(this.Endpoint + "/truncate", HttpMethod.POST);
            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                throw StreamChatException.FromResponse(response);
        }

        public async Task AddMembers(IEnumerable<string> userIDs, MessageInput msg = null)
        {
            var payload = new JObject(new JProperty("add_members", userIDs));
            if (msg != null)
            {
                if (msg.User != null)
                {
                    msg.User = new User()
                    {
                        ID = msg.User.ID
                    };
                }
                payload.Add("message", msg.ToJObject());
            }

            var request = this._client.BuildAppRequest(this.Endpoint, HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                throw StreamChatException.FromResponse(response);
        }

        public async Task RemoveMembers(IEnumerable<string> userIDs, MessageInput msg = null)
        {
            var payload = new JObject(new JProperty("remove_members", userIDs));
            if (msg != null)
            {
                if (msg.User != null)
                {
                    msg.User = new User()
                    {
                        ID = msg.User.ID
                    };
                }
                payload.Add("message", msg.ToJObject());
            }

            var request = this._client.BuildAppRequest(this.Endpoint, HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                throw StreamChatException.FromResponse(response);
        }

        public async Task AddModerators(IEnumerable<string> userIDs)
        {
            var payload = new
            {
                add_moderators = userIDs
            };
            var request = this._client.BuildAppRequest(this.Endpoint, HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                throw StreamChatException.FromResponse(response);
        }

        public async Task DemoteModerators(IEnumerable<string> userIDs)
        {
            var payload = new
            {
                demote_moderators = userIDs
            };
            var request = this._client.BuildAppRequest(this.Endpoint, HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                throw StreamChatException.FromResponse(response);
        }

        public async Task<Event> MarkRead(string userID, string messageID = "")
        {
            var payload = new
            {
                user = new
                {
                    id = userID
                },
                message_id = messageID
            };
            var request = this._client.BuildAppRequest(this.Endpoint + "/read", HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var respObj = JObject.Parse(response.Content);
                var evtObj = respObj.Property("event").Value as JObject;
                return Event.FromJObject(evtObj);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<List<Message>> GetReplies(string parentID, MessagePaginationParams pagination)
        {
            var endpoint = string.Format("messages/{0}/replies", parentID);
            var request = this._client.BuildAppRequest(endpoint, HttpMethod.GET);
            pagination.Apply(request);

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var respObj = JObject.Parse(response.Content);
                var msgs = respObj.Property("messages").Value as JArray;
                var result = new List<Message>();

                foreach (var msg in msgs)
                {
                    var msgObj = msg as JObject;
                    result.Add(Message.FromJObject(msgObj));
                }
                return result;
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task BanUser(string targetID, string id, string reason, int timeoutMinutes = 0)
        {
            await this._client.Users.Ban(targetID, id, reason, this, timeoutMinutes);
        }

        public async Task UnbanUser(string targetID)
        {
            await this._client.Users.Unban(targetID, this);
        }

        public async Task Hide(string userID, bool clearHistory = false)
        {
            var request = this._client.BuildAppRequest(this.Endpoint + "/hide", HttpMethod.POST);
            var payload = new JObject();
            payload.Add("user_id", userID);
            payload.Add("clear_history", clearHistory);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                throw StreamChatException.FromResponse(response);
            }
        }

        public async Task Show(string userID)
        {
            var request = this._client.BuildAppRequest(this.Endpoint + "/show", HttpMethod.POST);
            var payload = new JObject(new JProperty("user_id", userID));
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                throw StreamChatException.FromResponse(response);
            }
        }
    }
}

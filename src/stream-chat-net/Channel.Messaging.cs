using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamChat.Rest;

namespace StreamChat
{
    public partial class Channel
    {
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
    }
}
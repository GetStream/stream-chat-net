using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamChat.Rest;

namespace StreamChat
{
    public class Channel : CustomDataBase
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

        internal Channel(Client client, string type, string id = null, GenericData data = null)
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

        public async Task<Message> SendMessage(Message msg, string userID)
        {
            if (msg.User == null)
            {
                msg.User = new User();
            }
            msg.User.ID = userID;

            var payload = new JObject(new JProperty("message", msg.ToJObject()));


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
                if (this.ID == null)
                    this.ID = chanState.Channel.ID;
                return chanState;
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task AddMembers(IEnumerable<string> userIDs)
        {
            var payload = new
            {
                add_members = userIDs
            };
            var request = this._client.BuildAppRequest(this.Endpoint, HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                throw StreamChatException.FromResponse(response);
        }

        public async Task RemoveMembers(IEnumerable<string> userIDs)
        {
            var payload = new
            {
                remove_members = userIDs
            };
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
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamChat.Rest;

namespace StreamChat
{
    public partial class Channel
    {
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
                this._data = customData;
                var respObj = JObject.Parse(response.Content);
                return UpdateChannelResponse.FromJObject(respObj);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<PartialUpdateChannelResponse> PartialUpdate(PartialUpdateChannelRequest partialUpdateChannelRequest)
        {
            var request = this._client.BuildAppRequest(this.Endpoint, HttpMethod.PATCH);
            request.SetJsonBody(partialUpdateChannelRequest.ToJObject().ToString());

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if(partialUpdateChannelRequest.Unset != null)
                {
                    partialUpdateChannelRequest.Unset.ForEach(x => this._data.RemoveData(x));
                }
                if (partialUpdateChannelRequest.Set != null)
                {
                    partialUpdateChannelRequest.Set.ForEach(x => this._data.SetData(x.Key, x.Value));
                }

                var respObj = JObject.Parse(response.Content);
                return PartialUpdateChannelResponse.FromJObject(respObj);
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

        public async Task<TruncateResponse> Truncate()
        {
            return await Truncate(null);
        }

        public async Task<TruncateResponse> Truncate(TruncateOptions truncateOptions)
        {
            var request = this._client.BuildAppRequest(this.Endpoint + "/truncate", HttpMethod.POST);
            if (truncateOptions != null)
            {
                request.SetJsonBody(truncateOptions.ToJObject().ToString());
            }
            
            var response = await this._client.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var respObj = JObject.Parse(response.Content);
                return TruncateResponse.FromJObject(respObj);
            }
            throw StreamChatException.FromResponse(response);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamChat.Rest;

namespace StreamChat
{
    public partial class Channel
    {
        public async Task AddMembers(IEnumerable<string> userIDs, MessageInput msg = null, AddMemberOptions options = null)
        {
            var payload = new JObject(new JProperty("add_members", userIDs));

            if (options != null)
            {
                payload.Merge(JObject.FromObject(options));
            }

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

        public async Task<UpdateChannelResponse> AssignRoles(AssignRoleRequest roleRequest)
        {
            var request = this._client.BuildAppRequest(this.Endpoint, HttpMethod.POST);
            request.SetJsonBody(roleRequest.ToJObject().ToString());

            var response = await this._client.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                throw StreamChatException.FromResponse(response);

            var respObj = JObject.Parse(response.Content);
            return UpdateChannelResponse.FromJObject(respObj);
        }

    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StreamChat.Rest;

namespace StreamChat
{
    public partial class Channel
    {
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


        public async Task BanUser(string targetID, string id, string reason, int timeoutMinutes = 0)
        {
            await this._client.Users.Ban(targetID, id, reason, this, timeoutMinutes);
        }

        public async Task UnbanUser(string targetID)
        {
            await this._client.Users.Unban(targetID, this);
        }

    }
}
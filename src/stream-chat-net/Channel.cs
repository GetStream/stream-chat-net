using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamChat.Rest;

namespace StreamChat
{
    public partial class Channel : CustomDataBase, IChannel
    {
        public string Type { get; private set; }
        public string ID { get; private set; }

        private string Endpoint
        {
            get
            {
                if (this.ID == null)
                    throw new ArgumentNullException("channel id", "Channel ID must be set");
                
                return string.Format("channels/{0}/{1}", this.Type, this.ID);
            }
        }

        private readonly Client _client;

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

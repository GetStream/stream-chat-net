using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamChat.Rest;
using HttpMethod = StreamChat.Rest.HttpMethod;

namespace StreamChat
{
    public class Client : IClient
    {
        private static readonly string Version = "0.27.0";
        private readonly Uri BaseUrl = new Uri("https://chat.stream-io-api.com");
        private static readonly HttpClient DefaultHttpClient = new HttpClient();
        private static readonly object JWTHeader = new
        {
            typ = "JWT",
            alg = "HS256"
        };
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly ClientOptions _options;
        private readonly RestClient _client;
        private readonly string _apiSecret;
        private readonly string _apiKey;
        private readonly string _token;

        public Client(string apiKey, string apiSecret) : this(null, null, apiKey, apiSecret)
        {
        }

        public Client(string apiKey, string apiSecret, ClientOptions opts) : this(opts, null, apiKey, apiSecret)
        {
        }

        public Client(string apiKey, string apiSecret, HttpClient httpClient) : this(null, httpClient, apiKey, apiSecret)
        {
        }

        public Client(string apiKey, string apiSecret, HttpClient httpClient, ClientOptions opts) : this(opts, httpClient, apiKey, apiSecret)
        {
        }

        private Client(ClientOptions opts, HttpClient httpClient, string apiKey, string apiSecret)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(nameof(apiKey), "Must have an apiKey");
            if (string.IsNullOrWhiteSpace(apiSecret))
                throw new ArgumentNullException(nameof(apiSecret), "Must have an apiSecret");

            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _options = opts ?? ClientOptions.Default;
            httpClient = httpClient ?? DefaultHttpClient;
            _client = new RestClient(httpClient, BaseUrl, TimeSpan.FromMilliseconds(_options.Timeout));
            var payload = new Dictionary<string, object>
            {
                {"server",  true}
            };
            _token = this.GenerateJwt(payload);
        }

        public IUsers Users
        {
            get
            {
                return new Users(this);
            }
        }

        public string CreateToken(string userId, DateTime? expiration = null)
        {
            var payload = new Dictionary<string, object>
            {
                {"user_id", userId}
            };
            if (expiration.HasValue)
            {
                payload["exp"] = (Int32)(expiration.Value.ToUniversalTime().Subtract(epoch).TotalSeconds);
            }
            return this.GenerateJwt(payload);
        }

        public async Task UpdateAppSettings(AppSettings settings)
        {
            var request = BuildAppRequest("app", HttpMethod.PATCH);
            request.SetJsonBody(JsonConvert.SerializeObject(settings));

            var response = await this.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw StreamChatException.FromResponse(response);
        }

        public async Task<AppSettingsWithDetails> GetAppSettings()
        {
            var request = BuildAppRequest("app", HttpMethod.GET);

            var response = await this.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var obj = JObject.Parse(response.Content);
                return (AppSettingsWithDetails)obj.Property("app").Value.ToObject(typeof(AppSettingsWithDetails));
            }

            throw StreamChatException.FromResponse(response);
        }

        public async Task<ChannelTypeOutput> CreateChannelType(ChannelTypeInput channelType)
        {
            if (channelType.Commands == null || channelType.Commands.Count == 0)
            {
                channelType.Commands = new List<string>() { Commands.All };
            }
            var request = BuildAppRequest("channeltypes", HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(channelType));

            var response = await this.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return JsonConvert.DeserializeObject<ChannelTypeOutput>(response.Content);

            throw StreamChatException.FromResponse(response);
        }

        public async Task<ChannelTypeInfo> GetChannelType(string type)
        {
            var endpoint = string.Format("channeltypes/{0}", type);
            var request = BuildAppRequest(endpoint, HttpMethod.GET);

            var response = await this.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<ChannelTypeInfo>(response.Content);

            throw StreamChatException.FromResponse(response);
        }

        public async Task<Dictionary<string, ChannelTypeInfo>> ListChannelTypes()
        {
            var request = BuildAppRequest("channeltypes", HttpMethod.GET);
            var response = await this.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var obj = JObject.Parse(response.Content);
                return (Dictionary<string, ChannelTypeInfo>)obj.Property("channel_types").Value.ToObject(typeof(Dictionary<string, ChannelTypeInfo>));
            }

            throw StreamChatException.FromResponse(response);
        }

        public async Task<ChannelTypeOutput> UpdateChannelType(string type, ChannelTypeInput channelType)
        {
            var payload = JObject.FromObject(channelType);
            payload.Remove("name");

            var endpoint = string.Format("channeltypes/{0}", type);
            var request = BuildAppRequest(endpoint, HttpMethod.PUT);
            request.SetJsonBody(payload.ToString());

            var response = await this.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return JsonConvert.DeserializeObject<ChannelTypeOutput>(response.Content);

            throw StreamChatException.FromResponse(response);
        }

        public async Task DeleteChannelType(string type)
        {
            var endpoint = string.Format("channeltypes/{0}", type);
            var request = BuildAppRequest(endpoint, HttpMethod.DELETE);

            var response = await this.MakeRequest(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw StreamChatException.FromResponse(response);
        }

        public async Task AddDevice(Device d)
        {
            var request = BuildAppRequest("devices", HttpMethod.POST);
            request.SetJsonBody(JsonConvert.SerializeObject(d));

            var response = await this.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                throw StreamChatException.FromResponse(response);
        }

        public async Task<List<Device>> GetDevices(string userID)
        {
            var request = BuildAppRequest("devices", HttpMethod.GET);
            request.AddQueryParameter("user_id", userID);
            var response = await this.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var obj = JObject.Parse(response.Content);
                return (List<Device>)obj.Property("devices").Value.ToObject(typeof(List<Device>));
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task RemoveDevice(string deviceID, string userID)
        {
            var request = BuildAppRequest("devices", HttpMethod.DELETE);
            request.AddQueryParameter("user_id", userID);
            request.AddQueryParameter("id", deviceID);

            var response = await this.MakeRequest(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw StreamChatException.FromResponse(response);
        }

        public async Task<RateLimitsMap> GetRateLimits(GetRateLimitsOptions opts)
        {
            var request = BuildAppRequest("rate_limits", HttpMethod.GET);
            opts.Apply(request);

            var response = await this.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<RateLimitsMap>(response.Content);
            }
            throw StreamChatException.FromResponse(response);
        }

        public IChannel Channel(string channelType, string channelID = "", GenericData data = null)
        {
            return new Channel(this, channelType, channelID, data);
        }

        public async Task<List<ChannelState>> QueryChannels(QueryChannelsOptions opts)
        {
            var request = this.BuildAppRequest("channels", HttpMethod.GET);
            opts.Apply(request);

            var response = await this.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var root = JObject.Parse(response.Content);
                var chans = root.Property("channels").Value as JArray;
                var result = new List<ChannelState>();
                foreach (var chan in chans)
                {
                    var chanObj = chan as JObject;
                    result.Add(ChannelState.FromJObject(chanObj));
                }
                return result;
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<MessageSearchResponse> Search(SearchOptions opts)
        {
            var request = this.BuildAppRequest("search", HttpMethod.GET);
            opts.Apply(request);

            var response = await this.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<MessageSearchResponse>(response.Content);
            }
            throw StreamChatException.FromResponse(response);
        }


        public async Task<Message> UpdateMessage(MessageInput msg)
        {
            if (string.IsNullOrEmpty(msg.ID))
                throw new ArgumentException("message.id must be set");
            var payload = new JObject(new JProperty("message", msg.ToJObject()));

            var endpoint = string.Format("messages/{0}", msg.ID);
            var request = this.BuildAppRequest(endpoint, HttpMethod.POST);
            request.SetJsonBody(payload.ToString());

            var response = await this.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var respObj = JObject.Parse(response.Content);
                var msgObj = respObj.Property("message").Value as JObject;
                return Message.FromJObject(msgObj);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<Message> DeleteMessage(string messageID, bool hardDelete = false)
        {
            var endpoint = string.Format("messages/{0}", messageID);
            var request = this.BuildAppRequest(endpoint, HttpMethod.DELETE);
            if (hardDelete)
            {
                request.AddQueryParameter("hard", "true");
            }

            var response = await this.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var respObj = JObject.Parse(response.Content);
                var msgObj = respObj.Property("message").Value as JObject;
                return Message.FromJObject(msgObj);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<Message> GetMessage(string messageID)
        {
            var endpoint = string.Format("messages/{0}", messageID);
            var request = this.BuildAppRequest(endpoint, HttpMethod.GET);

            var response = await this.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var respObj = JObject.Parse(response.Content);
                var msgObj = respObj.Property("message").Value as JObject;
                return Message.FromJObject(msgObj);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task FlagUser(string flaggedID, string flaggerID)
        {
            await this.postFlag(flaggedID, flaggerID, "flag", "user");
        }

        public async Task UnflagUser(string flaggedID, string flaggerID)
        {
            await this.postFlag(flaggedID, flaggerID, "unflag", "user");
        }

        public async Task FlagMessage(string flaggedID, string flaggerID)
        {
            await this.postFlag(flaggedID, flaggerID, "flag", "message");
        }

        public async Task UnflagMessage(string flaggedID, string flaggerID)
        {
            await this.postFlag(flaggedID, flaggerID, "unflag", "message");
        }

        private async Task postFlag(string dest, string src, string op, string kind)
        {
            var endpoint = string.Format("moderation/{0}", op);
            var request = this.BuildAppRequest(endpoint, HttpMethod.POST);
            var payload = new JObject();
            if (kind == "user")
            {
                payload.Add("target_user_id", dest);
            }
            else
            {
                payload.Add("target_message_id", dest);
            }
            payload.Add("user_id", src);
            request.SetJsonBody(payload.ToString());

            var response = await this.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                throw StreamChatException.FromResponse(response);
            }
        }

        public async Task<string> ExportChannels(List<ExportChannelRequest> reqs)
        {
            var request = this.BuildAppRequest("export_channels", HttpMethod.POST);
            var payload = new JObject(new JProperty("channels", JArray.FromObject(reqs)));
            request.SetJsonBody(payload.ToString());

            var response = await this.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var respObj = JObject.Parse(response.Content);
                var taskId = respObj.Property("task_id").Value.ToString();
                return taskId;
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<ExportChannelsStatusResponse> GetExportChannelsStatus(string taskId)
        {
            var endpoint = string.Format("export_channels/{0}", taskId);
            var request = this.BuildAppRequest(endpoint, HttpMethod.GET);

            var response = await this.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<ExportChannelsStatusResponse>(response.Content);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<TaskStatus> GetTaskStatus(string taskId)
        {
            var endpoint = string.Format("tasks/{0}", taskId);
            var request = this.BuildAppRequest(endpoint, HttpMethod.GET);

            var response = await this.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<TaskStatus>(response.Content);
            }
            throw StreamChatException.FromResponse(response);
        }

        public async Task<string> DeleteChannels(string[] cids, bool hardDelete = false)
        {
            var endpoint = string.Format("channels/delete");
            var request = this.BuildAppRequest(endpoint, HttpMethod.POST);
            var payload = new
            {
                cids = cids,
                hard_delete = hardDelete,
            };
            request.SetJsonBody(JsonConvert.SerializeObject(payload));

            var response = await this.MakeRequest(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return JObject.Parse(response.Content).Property("task_id").Value.ToString();
            }
            throw StreamChatException.FromResponse(response);
        }

        public bool VerifyWebhook(string requestBody, string xSignature)
        {
            using (var sha = new HMACSHA256(Encoding.UTF8.GetBytes(this._apiSecret)))
            {
                var sigBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(requestBody));
                var sig = BitConverter.ToString(sigBytes).Replace("-", string.Empty).ToLower();
                return sig == xSignature;
            }
        }

        internal RestRequest BuildAppRequest(string path, HttpMethod method)
        {
            return BuildRestRequest(path, method);
        }

        internal Task<RestResponse> MakeRequest(RestRequest request)
        {
            return _client.Execute(request);
        }

        private string GenerateJwt(object payload)
        {
            var segments = new List<string>();

            byte[] headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Client.JWTHeader));
            byte[] payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));

            segments.Add(Base64UrlEncode(headerBytes));
            segments.Add(Base64UrlEncode(payloadBytes));

            var stringToSign = string.Join(".", segments.ToArray());
            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            using (var sha = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret)))
            {
                byte[] signature = sha.ComputeHash(bytesToSign);
                segments.Add(Base64UrlEncode(signature));
            }
            return string.Join(".", segments.ToArray());
        }

        private RestRequest BuildRestRequest(string fullPath, HttpMethod method)
        {
            var request = new RestRequest(fullPath, method);
            request.AddHeader("Authorization", _token);
            request.AddHeader("stream-auth-type", "jwt");
            request.AddHeader("X-Stream-Client", "stream-chat-net-client-" + Version);
            request.AddQueryParameter("api_key", _apiKey);
            return request;
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Trim('=');
        }
    }
}

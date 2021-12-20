using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StreamChat.Rest;

namespace StreamChat
{
    public abstract class ServiceBase
    {
        protected readonly Client _client;

        internal ServiceBase(Client client)
        {
            _client = client;
        }

        protected async Task<T> ExecuteRequestAsync<T>(string relativeUri, HttpMethod method, HttpStatusCode expectedStatusCode, object body = null)
        {
            var req = _client.BuildAppRequest(relativeUri, method);

            if (body != null)
            {
                req.SetJsonBody(JsonConvert.SerializeObject(body));
            }

            var resp = await _client.MakeRequest(req);

            if (resp.StatusCode == expectedStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(resp.Content);
            }
            
            throw StreamChatException.FromResponse(resp);
        }
    }
}
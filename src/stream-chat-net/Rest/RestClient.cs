using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StreamChat.Rest
{
    internal class RestClient
    {
        readonly Uri _baseUrl;
        private TimeSpan _timeout;

        public RestClient(Uri baseUrl, TimeSpan timeout)
        {
            _baseUrl = baseUrl;
            _timeout = timeout;
        }

        private HttpClient BuildClient(RestRequest request)
        {
#if NET45
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
#endif
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.Timeout = _timeout;

            // add request headers
            request.Headers.ForEach(h =>
            {
                client.DefaultRequestHeaders.Add(h.Key, h.Value);
            });

            return client;
        }

        public TimeSpan Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        private async Task<RestResponse> ExecuteGet(Uri url, RestRequest request)
        {
            using (var client = BuildClient(request))
            {
                HttpResponseMessage response = await client.GetAsync(url);
                return await RestResponse.FromResponseMessage(response);
            }
        }

        private async Task<RestResponse> ExecutePost(Uri url, RestRequest request)
        {
            using (var client = BuildClient(request))
            {
                var payload = request.JsonBody ?? "{}";
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
                return await RestResponse.FromResponseMessage(response);
            }
        }

        private async Task<RestResponse> ExecutePut(Uri url, RestRequest request)
        {
            using (var client = BuildClient(request))
            {
                var payload = request.JsonBody ?? "{}";
                HttpResponseMessage response = await client.PutAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
                return await RestResponse.FromResponseMessage(response);
            }
        }

        private async Task<RestResponse> ExecuteDelete(Uri url, RestRequest request)
        {
            using (var client = BuildClient(request))
            {
                HttpResponseMessage response = await client.DeleteAsync(url);
                return await RestResponse.FromResponseMessage(response);
            }
        }

        private Uri BuildUri(RestRequest request)
        {
            var queryString = "";
            request.QueryParameters.ForEach((p) =>
            {
                queryString += (queryString.Length == 0) ? "?" : "&";
                queryString += string.Format("{0}={1}", p.Key, Uri.EscapeDataString(p.Value.ToString()));
            });
            return new Uri(_baseUrl, request.Resource + queryString);
        }

        public Task<RestResponse> Execute(RestRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request", "Request is required");

            Uri url = this.BuildUri(request);

            switch (request.Method)
            {
                case HttpMethod.DELETE:
                    return this.ExecuteDelete(url, request);
                case HttpMethod.POST:
                    return this.ExecutePost(url, request);
                case HttpMethod.PUT:
                    return this.ExecutePut(url, request);
                default:
                    return this.ExecuteGet(url, request);
            }
        }
    }
}

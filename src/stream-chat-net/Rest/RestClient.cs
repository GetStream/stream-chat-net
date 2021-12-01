using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace StreamChat.Rest
{
    internal class RestClient
    {
        private readonly Uri _baseUrl;
        private readonly HttpClient _httpClient;
        private static readonly MediaTypeWithQualityHeaderValue _jsonAcceptHeader = new MediaTypeWithQualityHeaderValue("application/json");
        private static readonly IReadOnlyList<HttpMethod> _methodsWithRequestBody = new List<HttpMethod>
        {
            HttpMethod.POST, HttpMethod.PATCH, HttpMethod.PUT
        };

        public TimeSpan Timeout { get; set; }

        public RestClient(HttpClient httpClient, Uri baseUrl, TimeSpan timeout)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            Timeout = timeout;
#if NET45
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
#endif
        }

        private HttpRequestMessage GenerateRequestMessage(RestRequest req, Uri uri)
        {
            var request = new HttpRequestMessage(req.Method.ToDotnetHttpMethod(), uri);
            request.Headers.Accept.Add(_jsonAcceptHeader);
            req.Headers.ForEach(kvp => request.Headers.Add(kvp.Key, kvp.Value));
            var shouldHaveRequestBody = _methodsWithRequestBody.Contains(req.Method);

            if (shouldHaveRequestBody)
            {
                request.Content = new StringContent(req.JsonBody ?? "{}", Encoding.UTF8, "application/json");
            }

            return request;
        }

        private Uri BuildUriWithQueryString(RestRequest request)
        {
            var queryString = "";
            request.QueryParameters.ForEach((p) =>
            {
                queryString += (queryString.Length == 0) ? "?" : "&";
                queryString += string.Format("{0}={1}", p.Key, Uri.EscapeDataString(p.Value.ToString()));
            });
            return new Uri(_baseUrl, request.Resource + queryString);
        }

        public async Task<RestResponse> Execute(RestRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request is required");

            var url = BuildUriWithQueryString(request);

            var cancelTokeSource = new CancellationTokenSource();
            cancelTokeSource.CancelAfter(Timeout);

            using (cancelTokeSource)
            using (var req = GenerateRequestMessage(request, url))
            {
                var response = await _httpClient.SendAsync(req, cancelTokeSource.Token);
                return await RestResponse.FromResponseMessage(response);
            }
        }
    }
}
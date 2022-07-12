using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Utils;

namespace StreamChat.Rest
{
    internal class RestClient : IRestClient
    {
        private static readonly MediaTypeWithQualityHeaderValue _jsonAcceptHeader = new MediaTypeWithQualityHeaderValue("application/json");
        private static readonly IReadOnlyList<HttpMethod> _methodsWithRequestBody = new List<HttpMethod>
        {
            HttpMethod.POST, HttpMethod.PATCH, HttpMethod.PUT,
        };
        private readonly string _jwt;
        private readonly string _apiKey;
        private readonly string _sdkVersion;
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUrl;
        private readonly TimeSpan _timeout;

        internal RestClient(ClientOptions opts, string jwt, string apiKey, string sdkVersion)
        {
            _httpClient = opts.HttpClient;
            _baseUrl = opts.BaseUrl;
            _jwt = jwt;
            _apiKey = apiKey;
            _sdkVersion = sdkVersion;
            _timeout = opts.Timeout;
#if OLD_TLS_HANDLING
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
#endif
        }

        public RestRequest BuildRestRequest(string fullPath,
            HttpMethod method,
            object body,
            IEnumerable<KeyValuePair<string, string>> queryParams,
            MultipartFormDataContent multipartBody)
        {
            return new RestRequest()
                .SetRelativeUri(fullPath)
                .SetHttpMethod(method)
                .AddHeader("Authorization", _jwt)
                .AddHeader("stream-auth-type", "jwt")
                .AddHeader("X-Stream-Client", $"stream-chat-net-client-{_sdkVersion}")
                .SetJsonBodyIfNotNull(body == null ? null : StreamJsonConverter.SerializeObject(body))
                .SetMultipartBodyIfNotNull(multipartBody)
                .AddQueryParameter("api_key", _apiKey)
                .AddQueryParametersIfNotNull(queryParams);
        }

        public async Task<RestResponse> ExecuteAsync(RestRequest request)
        {
            var uri = BuildUriWithQueryString(request);

            using (var cancelTokenSource = new CancellationTokenSource(_timeout))
            using (var req = GenerateRequestMessage(request, uri))
            {
                var response = await _httpClient.SendAsync(req, cancelTokenSource.Token);
                return await RestResponse.FromResponseMessageAsync(response);
            }
        }

        private HttpRequestMessage GenerateRequestMessage(RestRequest req, Uri uri)
        {
            var request = new HttpRequestMessage(req.Method.ToDotnetHttpMethod(), uri);
            request.Headers.Accept.Add(_jsonAcceptHeader);
            req.Headers.ForEach(kvp => request.Headers.Add(kvp.Key, kvp.Value));
            var shouldHaveRequestBody = _methodsWithRequestBody.Contains(req.Method);

            if (shouldHaveRequestBody)
            {
                if (req.MultipartBody != null)
                    request.Content = req.MultipartBody;
                else
                    request.Content = new StringContent(req.JsonBody ?? "{}", Encoding.UTF8, "application/json");
            }

            return request;
        }

        private Uri BuildUriWithQueryString(RestRequest request)
        {
            var queryStringBuilder = new StringBuilder();

            request.QueryParameters.ForEach(p =>
            {
                if (queryStringBuilder.Length > 0)
                {
                    queryStringBuilder.Append("&");
                }

                queryStringBuilder.Append($"{p.Key}={Uri.EscapeDataString(p.Value)}");
            });

            var uriBuilder = new UriBuilder(_baseUrl)
            {
                Path = request.RelativeUri,
                Query = queryStringBuilder.ToString(),
            };

            return uriBuilder.Uri;
        }
    }
}
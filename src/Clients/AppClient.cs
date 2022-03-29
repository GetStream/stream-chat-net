using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class AppClient : ClientBase, IAppClient
    {
        private readonly string _apiSecret;

        internal AppClient(IRestClient client, string apiSecret) : base(client)
        {
            _apiSecret = apiSecret;
        }

        public async Task<GetAppResponse> GetAppSettingsAsync()
            => await ExecuteRequestAsync<GetAppResponse>("app",
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<ApiResponse> UpdateAppSettingsAsync(AppSettingsRequest settings)
            => await ExecuteRequestAsync<ApiResponse>("app",
                HttpMethod.PATCH,
                HttpStatusCode.OK,
                settings);

        public async Task<RateLimitsMap> GetRateLimitsAsync(GetRateLimitsOptions opts)
            => await ExecuteRequestAsync<RateLimitsMap>("rate_limits",
                    HttpMethod.GET,
                    HttpStatusCode.OK,
                    queryParams: opts?.ToQueryParameters());

        public async Task<AppCheckPushResponse> CheckPushAsync(AppCheckPushRequest checkPushRequest)
            => await ExecuteRequestAsync<AppCheckPushResponse>("check_push",
                    HttpMethod.POST,
                    HttpStatusCode.Created,
                    checkPushRequest);

        public async Task<AppCheckSqsResponse> CheckSqsPushAsync(AppCheckSqsRequest checkSqsRequest)
            => await ExecuteRequestAsync<AppCheckSqsResponse>("check_sqs",
                    HttpMethod.POST,
                    HttpStatusCode.Created,
                    checkSqsRequest);

        public async Task<ApiResponse> RevokeTokensAsync(DateTimeOffset? issuedBefore)
            => await UpdateAppSettingsAsync(new AppSettingsRequest { RevokeTokensIssuedBefore = issuedBefore });

        public async Task<UpsertPushProviderResponse> UpsertPushProviderAsync(PushProviderRequest pushProviderRequest)
            => await ExecuteRequestAsync<UpsertPushProviderResponse>("push_providers",
                    HttpMethod.POST,
                    HttpStatusCode.Created,
                    new Dictionary<string, PushProviderRequest> { { "push_provider", pushProviderRequest } });

        public async Task<ListPushProvidersResponse> ListPushProvidersAsync()
            => await ExecuteRequestAsync<ListPushProvidersResponse>("push_providers",
                    HttpMethod.GET,
                    HttpStatusCode.OK);

        public async Task<ApiResponse> DeletePushProviderAsync(PushProviderType providerType, string name)
            => await ExecuteRequestAsync<ApiResponse>($"push_providers/{providerType}/{name}",
                    HttpMethod.DELETE,
                    HttpStatusCode.OK);

        public bool VerifyWebhook(string requestBody, string xSignature)
        {
            using (var sha = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret)))
            {
                var sigBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(requestBody));
                var sig = BitConverter.ToString(sigBytes).Replace("-", string.Empty).ToLower();
                return sig == xSignature;
            }
        }
    }
}
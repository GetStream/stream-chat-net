using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using StreamChat.Exceptions;
using StreamChat.Models;
using StreamChat.Rest;
using StreamChat.Utils;
using HttpMethod = StreamChat.Rest.HttpMethod;

namespace StreamChat.Clients
{
    public abstract class ClientBase
    {
        private readonly IRestClient _client;

        internal ClientBase(IRestClient client)
        {
            _client = client;
        }

        protected async Task<T> ExecuteRequestAsync<T>(
            string relativeUri,
            HttpMethod method,
            HttpStatusCode expectedStatusCode,
            object body = null,
            IEnumerable<KeyValuePair<string, string>> queryParams = null,
            MultipartFormDataContent multipartBody = null) where T : ApiResponse
        {
            var req = _client.BuildRestRequest(relativeUri, method, body, queryParams, multipartBody);

            var resp = await _client.ExecuteAsync(req);

            if (resp.StatusCode == expectedStatusCode)
            {
                T deserialized;

                try
                {
                    deserialized = StreamJsonConverter.DeserializeObject<T>(resp.Content);
                }
                catch (Exception ex)
                {
                    throw new StreamDeserializationException($"Failure during response deserialization. Type: {typeof(T)}. Response: {resp.Content}", ex);
                }

                return EnrichWithRateLimits(deserialized, resp);
            }

            throw StreamChatException.FromResponse(resp);
        }

        private T EnrichWithRateLimits<T>(T body, RestResponse resp) where T : ApiResponse
        {
            if (resp.TryGetRateLimit(out var rateLimit))
            {
                body.SetRateLimit(rateLimit);
            }

            return body;
        }
    }
}
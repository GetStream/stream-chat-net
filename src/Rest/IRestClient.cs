using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StreamChat.Rest
{
    public interface IRestClient
    {
        RestRequest BuildRestRequest(string fullPath,
            HttpMethod method,
            object body = null,
            IEnumerable<KeyValuePair<string, string>> queryParams = null,
            MultipartFormDataContent multipartBody = null);

        Task<RestResponse> ExecuteAsync(RestRequest request);
    }
}
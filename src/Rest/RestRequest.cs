using System.Collections.Generic;
using System.Net.Http;
using StreamChat.Utils;

namespace StreamChat.Rest
{
    internal class RestRequest
    {
        internal Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
        internal Dictionary<string, string> QueryParameters { get; private set; } = new Dictionary<string, string>();
        internal HttpMethod Method { get; private set; }
        internal string RelativeUri { get; private set; }
        internal string JsonBody { get; private set; }
        internal MultipartFormDataContent MultipartBody { get; private set; }

        internal RestRequest AddHeader(string name, string value)
        {
            Headers[name] = value;

            return this;
        }

        internal RestRequest SetRelativeUri(string uri)
        {
            RelativeUri = uri;

            return this;
        }

        internal RestRequest SetHttpMethod(HttpMethod method)
        {
            Method = method;

            return this;
        }

        internal RestRequest AddQueryParameter(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                QueryParameters.Add(name, value);

            return this;
        }

        internal RestRequest AddQueryParametersIfNotNull(IEnumerable<KeyValuePair<string, string>> queryParams)
        {
            queryParams?.ForEach(kvp => AddQueryParameter(kvp.Key, kvp.Value));

            return this;
        }

        internal RestRequest SetJsonBodyIfNotNull(string json)
        {
            if (json != null)
                JsonBody = json;

            return this;
        }

        internal RestRequest SetMultipartBodyIfNotNull(MultipartFormDataContent content)
        {
            if (content != null)
            {
                MultipartBody = content;
                AddHeader("X-Stream-LogRequestBody", "false");
            }

            return this;
        }
    }
}

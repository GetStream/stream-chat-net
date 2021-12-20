using System.Collections.Generic;

namespace StreamChat.Rest
{
    internal class RestRequest
    {
        private IDictionary<string, string> _headers = new Dictionary<string, string>();
        private IDictionary<string, string> _queryParameters = new Dictionary<string, string>();

        internal RestRequest(string resource, HttpMethod method)
        {
            Method = method;
            Resource = resource;
        }

        public HttpMethod Method { get; private set; }

        public string Resource { get; private set; }

        public string JsonBody { get; private set; }

        public void AddHeader(string name, string value)
        {
            _headers[name] = value;
        }

        public void AddQueryParameter(string name, string value)
        {
            _queryParameters[name] = value;
        }

        public void SetJsonBody(string json)
        {
            JsonBody = json;
        }

        public IEnumerable<KeyValuePair<string, string>> QueryParameters
        {
            get
            {
                return _queryParameters;
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return _headers;
            }
        }

        public bool HasBody
        {
            get { return JsonBody != null; }
        }
    }
}

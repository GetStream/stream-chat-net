using System;

namespace StreamChat.Rest
{
    public enum HttpMethod
    {
        GET,
        POST,
        DELETE,
        PUT,
        PATCH,
    }

    internal static class HttpMethodExtensions
    {
        internal static System.Net.Http.HttpMethod ToDotnetHttpMethod(this HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.GET:
                    return System.Net.Http.HttpMethod.Get;
                case HttpMethod.POST:
                    return System.Net.Http.HttpMethod.Post;
                case HttpMethod.DELETE:
                    return System.Net.Http.HttpMethod.Delete;
                case HttpMethod.PUT:
                    return System.Net.Http.HttpMethod.Put;
                case HttpMethod.PATCH:
                    return new System.Net.Http.HttpMethod("PATCH");
                default:
                    throw new NotImplementedException(method.ToString());
            }
        }
    }
}

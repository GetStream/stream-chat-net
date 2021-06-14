using Newtonsoft.Json;
using StreamChat.Rest;
using System;
using System.Collections.Generic;

namespace StreamChat
{
#if !NETCORE
    [Serializable]
#endif

    public class StreamChatException : Exception
    {
        public int? Code { get; internal set; }
        internal StreamChatException(ExceptionState state)
            : base(message: state.Message)
        {
            this.Code = state.Code;
        }

        internal class ExceptionState
        {
            public int? Code { get; set; }

            public string Message { get; set; }

            public Dictionary<string, string> ExceptionFields { get; set; }

            [Newtonsoft.Json.JsonProperty("status_code")]
            public int HttpStatusCode { get; set; }
        }

        internal static StreamChatException FromResponse(RestResponse response)
        {
            ExceptionState state = null;
            if (!string.IsNullOrWhiteSpace(response.Content))
            {
                state = JsonConvert.DeserializeObject<ExceptionState>(response.Content);
                state.HttpStatusCode = (int)response.StatusCode;
            }
            if (state == null)
            {
                state = new ExceptionState() { Code = null, Message = response.ErrorMessage, HttpStatusCode = (int)response.StatusCode };
            }
            throw new StreamChatException(state);
        }
    }
}

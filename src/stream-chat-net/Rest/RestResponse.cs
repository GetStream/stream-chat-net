using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StreamChat.Rest
{
    internal class RestResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Content { get; set; }

        public string ErrorMessage { get; set; }

        public Exception ErrorException { get; set; }

        internal static async Task<RestResponse> FromResponseMessage(HttpResponseMessage message)
        {
            var response = new RestResponse()
            {
                StatusCode = message.StatusCode
            };

            using(message)
            {
                response.Content = await message.Content.ReadAsStringAsync();
            }    

            return response;
        }
    }
}

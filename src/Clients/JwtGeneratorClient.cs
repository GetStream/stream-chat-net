using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace StreamChat.Clients
{
    internal class JwtGeneratorClient : IJwtGeneratorClient
    {
        private static readonly IReadOnlyDictionary<string, string> _jwtHeader = new Dictionary<string, string>
        {
            { "typ", "JWT" },
            { "alg", "HS256" },
        };
        public string GenerateServerSideJwt(string apiSecret) => GenerateJwt(new { server = true }, apiSecret);

        public string GenerateJwt(object payload, string apiSecret)
        {
            var segments = new List<string>(capacity: 3);

            var headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_jwtHeader));
            var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));

            segments.Add(Base64UrlEncode(headerBytes));
            segments.Add(Base64UrlEncode(payloadBytes));

            var stringToSign = string.Join(".", segments);
            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            using (var sha = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
            {
                var signature = sha.ComputeHash(bytesToSign);
                segments.Add(Base64UrlEncode(signature));
            }

            return string.Join(".", segments);
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Trim('=');
        }
    }
}
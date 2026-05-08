using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using StreamChat.Exceptions;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// Stateless helpers implementing the cross-SDK webhook contract documented at
    /// https://getstream.io/chat/docs/node/webhooks_overview/.
    /// </summary>
    /// <remarks>
    /// The composite functions (<see cref="VerifyAndParseWebhook"/>,
    /// <see cref="VerifyAndParseSqs"/>, <see cref="VerifyAndParseSns"/>) are the
    /// recommended entry points; the primitives they compose are exposed so callers
    /// can build custom flows or run individual steps in isolation.
    /// </remarks>
    public static class WebhookHelpers
    {
        private static readonly byte[] GzipMagic = new byte[] { 0x1f, 0x8b };

        public static byte[] UngzipPayload(byte[] body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (body.Length < 2 || body[0] != GzipMagic[0] || body[1] != GzipMagic[1])
            {
                return body;
            }

            try
            {
                using (var input = new MemoryStream(body))
                using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                using (var output = new MemoryStream())
                {
                    gzip.CopyTo(output);
                    return output.ToArray();
                }
            }
            catch (InvalidDataException ex)
            {
                throw new StreamWebhookSignatureException("failed to decompress gzip payload", ex);
            }
        }

        public static byte[] DecodeSqsPayload(string body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            byte[] decoded;
            try
            {
                decoded = Convert.FromBase64String(body);
            }
            catch (FormatException ex)
            {
                throw new StreamWebhookSignatureException("failed to base64-decode payload", ex);
            }

            return UngzipPayload(decoded);
        }

        public static byte[] DecodeSnsPayload(string message) => DecodeSqsPayload(message);

        public static bool VerifySignature(byte[] body, string signature, string secret)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (signature == null)
            {
                throw new ArgumentNullException(nameof(signature));
            }

            if (secret == null)
            {
                throw new ArgumentNullException(nameof(secret));
            }

            byte[] computed;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                computed = hmac.ComputeHash(body);
            }

            return TryHexToBytes(signature, out var provided)
                && provided.Length == computed.Length
                && FixedTimeEquals(computed, provided);
        }

        public static EventResponse ParseEvent(byte[] payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var json = Encoding.UTF8.GetString(payload);
            try
            {
                return JsonConvert.DeserializeObject<EventResponse>(json);
            }
            catch (JsonException ex)
            {
                throw new StreamWebhookSignatureException($"failed to parse webhook event: {ex.Message}", ex);
            }
        }

        public static EventResponse VerifyAndParseWebhook(byte[] body, string signature, string secret)
            => VerifyAndParseInternal(UngzipPayload(body), signature, secret);

        public static EventResponse VerifyAndParseSqs(string messageBody, string signature, string secret)
            => VerifyAndParseInternal(DecodeSqsPayload(messageBody), signature, secret);

        public static EventResponse VerifyAndParseSns(string message, string signature, string secret)
            => VerifyAndParseInternal(DecodeSnsPayload(message), signature, secret);

        private static EventResponse VerifyAndParseInternal(byte[] payload, string signature, string secret)
        {
            if (!VerifySignature(payload, signature, secret))
            {
                throw new StreamWebhookSignatureException("invalid webhook signature");
            }

            return ParseEvent(payload);
        }

        private static bool TryHexToBytes(string hex, out byte[] result)
        {
            result = null;
            if (hex == null || (hex.Length & 1) != 0)
            {
                return false;
            }

            var buf = new byte[hex.Length / 2];
            for (int i = 0; i < buf.Length; i++)
            {
                int hi = HexNibble(hex[i * 2]);
                int lo = HexNibble(hex[(i * 2) + 1]);
                if (hi < 0 || lo < 0)
                {
                    return false;
                }

                buf[i] = (byte)((hi << 4) | lo);
            }

            result = buf;
            return true;
        }

        private static int HexNibble(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }

            if (c >= 'a' && c <= 'f')
            {
                return c - 'a' + 10;
            }

            if (c >= 'A' && c <= 'F')
            {
                return c - 'A' + 10;
            }

            return -1;
        }

        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
#if NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            return CryptographicOperations.FixedTimeEquals(left, right);
#else
            if (left == null || right == null)
            {
                return false;
            }

            if (left.Length != right.Length)
            {
                return false;
            }

            int diff = 0;
            for (int i = 0; i < left.Length; i++)
            {
                diff |= left[i] ^ right[i];
            }

            return diff == 0;
#endif
        }
    }
}

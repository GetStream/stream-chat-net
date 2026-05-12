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
    /// can build custom flows or run individual steps in isolation. Every failure
    /// mode is reported through <see cref="StreamInvalidWebhookException"/>.
    /// </remarks>
    public static class WebhookHelpers
    {
        private static readonly byte[] GzipMagic = new byte[] { 0x1f, 0x8b };

        /// <summary>
        /// Returns <paramref name="body"/> verbatim when it is not gzipped, or the
        /// inflated bytes when the first two bytes match the RFC 1952 gzip magic
        /// number (<c>0x1F 0x8B</c>). The check is performed on the bytes themselves
        /// so it works regardless of the <c>Content-Encoding</c> header.
        /// </summary>
        /// <param name="body">Raw payload bytes; never <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="body"/> is <c>null</c>.</exception>
        /// <exception cref="StreamInvalidWebhookException">
        /// When the body starts with the gzip magic but cannot be inflated.
        /// </exception>
        public static byte[] GunzipPayload(byte[] body)
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
                throw new StreamInvalidWebhookException(StreamInvalidWebhookException.GzipFailed, ex);
            }
            catch (IOException ex)
            {
                throw new StreamInvalidWebhookException(StreamInvalidWebhookException.GzipFailed, ex);
            }
        }

        /// <summary>
        /// Reverses the SQS firehose envelope: base64-decodes <paramref name="body"/>
        /// and then inflates the result when it is gzipped. The returned bytes are the
        /// raw JSON that Stream signed.
        /// </summary>
        /// <param name="body">SQS message <c>Body</c> string; never <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="body"/> is <c>null</c>.</exception>
        /// <exception cref="StreamInvalidWebhookException">
        /// When the body is not valid base64 or the inner payload is malformed gzip.
        /// </exception>
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
                throw new StreamInvalidWebhookException(StreamInvalidWebhookException.InvalidBase64, ex);
            }

            return GunzipPayload(decoded);
        }

        /// <summary>
        /// Reverses an SNS HTTP notification envelope. When
        /// <paramref name="notificationBody"/> is a JSON envelope
        /// (<c>{"Type":"Notification","Message":"..."}</c>), the inner
        /// <c>Message</c> field is extracted and run through the SQS pipeline
        /// (base64-decode, then gzip-if-magic). When the input is not a JSON
        /// envelope it is treated as the already-extracted <c>Message</c>
        /// string, so call sites that pre-unwrap continue to work.
        /// </summary>
        /// <param name="notificationBody">SNS HTTP POST body, or a pre-extracted Message string.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="notificationBody"/> is <c>null</c>.</exception>
        /// <exception cref="StreamInvalidWebhookException">
        /// When the extracted Message is not valid base64 or the inner payload is malformed gzip.
        /// </exception>
        public static byte[] DecodeSnsPayload(string notificationBody)
        {
            if (notificationBody == null)
            {
                throw new ArgumentNullException(nameof(notificationBody));
            }

            var inner = ExtractSnsMessage(notificationBody);
            return DecodeSqsPayload(inner ?? notificationBody);
        }

        private static string ExtractSnsMessage(string notificationBody)
        {
            var trimmed = notificationBody.TrimStart();
            if (trimmed.Length == 0 || trimmed[0] != '{')
            {
                return null;
            }

            try
            {
                var envelope = JsonConvert.DeserializeObject<SnsEnvelope>(trimmed);
                return envelope?.Message;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private sealed class SnsEnvelope
        {
            [JsonProperty("Message")]
            public string Message { get; set; }
        }

        /// <summary>
        /// Returns <c>true</c> when the hex-encoded HMAC-SHA256 of <paramref name="body"/>
        /// keyed by <paramref name="secret"/> matches <paramref name="signature"/>.
        /// The byte comparison is constant-time.
        /// </summary>
        /// <param name="body">Raw bytes the server signed.</param>
        /// <param name="signature">Hex-encoded HMAC-SHA256 (typically the <c>X-Signature</c> header).</param>
        /// <param name="secret">Stream Chat API secret.</param>
        /// <exception cref="ArgumentNullException">When any argument is <c>null</c>.</exception>
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

        /// <summary>
        /// Deserialises the JSON in <paramref name="payload"/> into an
        /// <see cref="EventResponse"/>. Unknown event types still parse — the
        /// surrounding metadata (e.g. <c>type</c>, <c>cid</c>) is populated and
        /// future-specific fields land in the <c>CustomData</c> bag.
        /// </summary>
        /// <param name="payload">Raw UTF-8 JSON bytes.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="payload"/> is <c>null</c>.</exception>
        /// <exception cref="StreamInvalidWebhookException">When the JSON cannot be parsed.</exception>
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
                throw new StreamInvalidWebhookException(StreamInvalidWebhookException.InvalidJson, ex);
            }
        }

        /// <summary>
        /// Inflates <paramref name="body"/> when gzipped (detected from the body
        /// bytes, independent of any <c>Content-Encoding</c> header), verifies the
        /// HMAC-SHA256 signature in constant time, and returns the parsed event.
        /// </summary>
        /// <param name="body">Raw HTTP request body bytes Stream signed.</param>
        /// <param name="signature">Hex-encoded HMAC-SHA256 from the <c>X-Signature</c> header.</param>
        /// <param name="secret">Stream Chat API secret.</param>
        /// <exception cref="StreamInvalidWebhookException">
        /// When the signature does not match or the gzip / JSON envelope is malformed.
        /// </exception>
        public static EventResponse VerifyAndParseWebhook(byte[] body, string signature, string secret)
            => VerifyAndParseInternal(GunzipPayload(body), signature, secret);

        /// <summary>
        /// Reverses the SQS firehose envelope (base64, then optional gzip),
        /// verifies the HMAC-SHA256 signature against the uncompressed JSON,
        /// and returns the parsed event.
        /// </summary>
        /// <param name="messageBody">SQS message <c>Body</c> string.</param>
        /// <param name="signature">Hex-encoded HMAC-SHA256 from the <c>X-Signature</c> message attribute.</param>
        /// <param name="secret">Stream Chat API secret.</param>
        /// <exception cref="StreamInvalidWebhookException">
        /// When the signature does not match or the base64 / gzip / JSON envelope is malformed.
        /// </exception>
        public static EventResponse VerifyAndParseSqs(string messageBody, string signature, string secret)
            => VerifyAndParseInternal(DecodeSqsPayload(messageBody), signature, secret);

        /// <summary>
        /// Reverses the SNS firehose envelope (base64, then optional gzip),
        /// verifies the HMAC-SHA256 signature against the uncompressed JSON,
        /// and returns the parsed event. The wire format matches SQS;
        /// this overload exists so call sites read naturally.
        /// </summary>
        /// <param name="message">SNS notification <c>Message</c> field.</param>
        /// <param name="signature">Hex-encoded HMAC-SHA256 from the <c>X-Signature</c> message attribute.</param>
        /// <param name="secret">Stream Chat API secret.</param>
        /// <exception cref="StreamInvalidWebhookException">
        /// When the signature does not match or the base64 / gzip / JSON envelope is malformed.
        /// </exception>
        public static EventResponse VerifyAndParseSns(string message, string signature, string secret)
            => VerifyAndParseInternal(DecodeSnsPayload(message), signature, secret);

        private static EventResponse VerifyAndParseInternal(byte[] payload, string signature, string secret)
        {
            if (!VerifySignature(payload, signature, secret))
            {
                throw new StreamInvalidWebhookException(StreamInvalidWebhookException.SignatureMismatch);
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

using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using StreamChat.Exceptions;

namespace StreamChat.Clients
{
    /// <summary>
    /// Reverses the encoding wrappers Stream applies to outbound webhook /
    /// SQS / SNS payloads and verifies the HMAC signature server-side
    /// signs over the inner JSON.
    /// </summary>
    /// <remarks>
    /// Decode order is fixed by the cross-SDK contract: payload encoding
    /// (base64 wrapping) is unwrapped first because SQS / SNS firehose
    /// envelopes wrap the (possibly already gzipped) bytes in base64 to
    /// keep them transport-safe; only after that does <c>Content-Encoding</c>
    /// (gzip) get reversed.
    /// </remarks>
    internal static class WebhookHelpers
    {
        public static byte[] DecompressWebhookBody(byte[] body, string contentEncoding, string payloadEncoding)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            var working = body;

            if (!string.IsNullOrWhiteSpace(payloadEncoding))
            {
                var pe = payloadEncoding.Trim().ToLowerInvariant();
                if (pe == "base64" || pe == "b64")
                {
                    try
                    {
                        var b64Text = Encoding.ASCII.GetString(working);
                        working = Convert.FromBase64String(b64Text);
                    }
                    catch (FormatException ex)
                    {
                        throw new StreamWebhookSignatureException(
                            $"failed to decode webhook payload_encoding=\"{payloadEncoding}\": invalid base64 input",
                            ex);
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        $"unsupported webhook payload_encoding: {payloadEncoding}. This SDK only supports base64.");
                }
            }

            if (!string.IsNullOrWhiteSpace(contentEncoding))
            {
                var ce = contentEncoding.Trim().ToLowerInvariant();
                if (ce == "gzip")
                {
                    try
                    {
                        using (var input = new MemoryStream(working))
                        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                        using (var output = new MemoryStream())
                        {
                            gzip.CopyTo(output);
                            working = output.ToArray();
                        }
                    }
                    catch (InvalidDataException ex)
                    {
                        throw new StreamWebhookSignatureException("failed to decompress webhook body", ex);
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        $"unsupported webhook Content-Encoding: {contentEncoding}. This SDK only supports gzip; set webhook_compression_algorithm to \"gzip\" on the app config.");
                }
            }

            return working;
        }

        public static byte[] VerifyAndDecodeWebhook(string apiSecret, byte[] body, string signature, string contentEncoding, string payloadEncoding)
        {
            if (apiSecret == null)
            {
                throw new ArgumentNullException(nameof(apiSecret));
            }

            if (signature == null)
            {
                throw new ArgumentNullException(nameof(signature));
            }

            var decoded = DecompressWebhookBody(body, contentEncoding, payloadEncoding);

            byte[] computed;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
            {
                computed = hmac.ComputeHash(decoded);
            }

            if (!TryHexToBytes(signature, out var provided) || provided.Length != computed.Length || !FixedTimeEquals(computed, provided))
            {
                throw new StreamWebhookSignatureException("invalid webhook signature");
            }

            return decoded;
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

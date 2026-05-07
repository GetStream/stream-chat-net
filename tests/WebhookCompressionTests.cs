using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Clients;
using StreamChat.Exceptions;

#pragma warning disable SA1310 // Field names should not contain underscore — names mandated by the cross-SDK contract.

namespace StreamChatTests
{
    /// <summary>
    /// Unit tests for the webhook decode + signature-verification helpers
    /// added in <see cref="StreamChat.Clients.IAppClient"/>.
    /// </summary>
    /// <remarks>
    /// These tests do not extend <see cref="TestBase"/> on purpose — they
    /// must run fully offline because they cover SDK-only behavior that does
    /// not touch the Stream API. Tests follow the arrange-act-assert pattern
    /// divided by empty lines.
    /// </remarks>
    [TestFixture]
    public class WebhookCompressionTests
    {
        private const string JSON_BODY = "{\"type\":\"message.new\",\"message\":{\"text\":\"the quick brown fox\"}}";
        private const string API_SECRET = "tsec2";
        private const string API_KEY = "test-api-key";

        private static IAppClient BuildAppClient(string secret = API_SECRET)
            => new StreamClientFactory(API_KEY, secret).GetAppClient();

        private static byte[] Gzip(byte[] input)
        {
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
                {
                    gzip.Write(input, 0, input.Length);
                }

                return output.ToArray();
            }
        }

        private static byte[] Base64Wrap(byte[] input)
            => Encoding.ASCII.GetBytes(Convert.ToBase64String(input));

        private static string HmacHex(string secret, byte[] data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }

        [Test]
        public void VerifyWebhook_ExistingMethod_StillWorks()
        {
            var appClient = BuildAppClient();
            var signature = HmacHex(API_SECRET, Encoding.UTF8.GetBytes(JSON_BODY));

            var ok = appClient.VerifyWebhook(JSON_BODY, signature);
            var bad = appClient.VerifyWebhook(JSON_BODY, new string('0', 64));

            ok.Should().BeTrue();
            bad.Should().BeFalse();
        }

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("   ", "   ")]
        [TestCase(null, "")]
        [TestCase("", null)]
        public void DecompressWebhookBody_PassthroughWhenNoEncoding(string contentEncoding, string payloadEncoding)
        {
            var appClient = BuildAppClient();
            var body = Encoding.UTF8.GetBytes(JSON_BODY);

            var decoded = appClient.DecompressWebhookBody(body, contentEncoding, payloadEncoding);

            decoded.Should().Equal(body);
        }

        [Test]
        public void DecompressWebhookBody_GzipRoundTrip()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var gzipped = Gzip(raw);

            var decoded = appClient.DecompressWebhookBody(gzipped, contentEncoding: "gzip");

            decoded.Should().Equal(raw);
            Encoding.UTF8.GetString(decoded).Should().Be(JSON_BODY);
        }

        [Test]
        public void DecompressWebhookBody_Base64RoundTrip()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(raw);

            var decoded = appClient.DecompressWebhookBody(wrapped, payloadEncoding: "base64");

            decoded.Should().Equal(raw);
        }

        [Test]
        public void DecompressWebhookBody_Base64GzipRoundTrip_SqsSnsShape()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));

            var decoded = appClient.DecompressWebhookBody(wrapped, contentEncoding: "gzip", payloadEncoding: "base64");

            decoded.Should().Equal(raw);
        }

        [TestCase("GZIP", "BASE64")]
        [TestCase("GzIp", "Base64")]
        [TestCase(" gzip ", " b64 ")]
        public void DecompressWebhookBody_CaseInsensitive(string contentEncoding, string payloadEncoding)
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));

            var decoded = appClient.DecompressWebhookBody(wrapped, contentEncoding, payloadEncoding);

            decoded.Should().Equal(raw);
        }

        [TestCase("br")]
        [TestCase("brotli")]
        [TestCase("zstd")]
        [TestCase("deflate")]
        [TestCase("compress")]
        [TestCase("lz4")]
        public void DecompressWebhookBody_RejectsUnsupportedContentEncoding(string contentEncoding)
        {
            var appClient = BuildAppClient();
            var body = Encoding.UTF8.GetBytes(JSON_BODY);

            Action call = () => appClient.DecompressWebhookBody(body, contentEncoding);

            call.Should().Throw<InvalidOperationException>()
                .WithMessage("*unsupported webhook Content-Encoding*gzip*");
        }

        [TestCase("hex")]
        [TestCase("url")]
        [TestCase("binary")]
        public void DecompressWebhookBody_RejectsUnsupportedPayloadEncoding(string payloadEncoding)
        {
            var appClient = BuildAppClient();
            var body = Encoding.UTF8.GetBytes(JSON_BODY);

            Action call = () => appClient.DecompressWebhookBody(body, payloadEncoding: payloadEncoding);

            call.Should().Throw<InvalidOperationException>()
                .WithMessage("*unsupported webhook payload_encoding*base64*");
        }

        [Test]
        public void DecompressWebhookBody_ThrowsOnInvalidGzipBytes()
        {
            var appClient = BuildAppClient();
            var notGzip = Encoding.UTF8.GetBytes(JSON_BODY);

            Action call = () => appClient.DecompressWebhookBody(notGzip, contentEncoding: "gzip");

            call.Should().Throw<StreamWebhookSignatureException>()
                .WithMessage("*failed to decompress webhook body*");
        }

        [Test]
        public void DecompressWebhookBody_ThrowsOnInvalidBase64Input()
        {
            var appClient = BuildAppClient();
            var notBase64 = Encoding.UTF8.GetBytes("@@@-not-base64-@@@");

            Action call = () => appClient.DecompressWebhookBody(notBase64, payloadEncoding: "base64");

            call.Should().Throw<StreamWebhookSignatureException>()
                .WithMessage("*payload_encoding*");
        }

        [Test]
        public void VerifyAndDecodeWebhook_HappyPathPlain()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);

            var decoded = appClient.VerifyAndDecodeWebhook(raw, signature);

            Encoding.UTF8.GetString(decoded).Should().Be(JSON_BODY);
        }

        [Test]
        public void VerifyAndDecodeWebhook_HappyPathGzip()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);
            var gzipped = Gzip(raw);

            var decoded = appClient.VerifyAndDecodeWebhook(gzipped, signature, contentEncoding: "gzip");

            Encoding.UTF8.GetString(decoded).Should().Be(JSON_BODY);
        }

        [Test]
        public void VerifyAndDecodeWebhook_HappyPathBase64Gzip()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);
            var wrapped = Base64Wrap(Gzip(raw));

            var decoded = appClient.VerifyAndDecodeWebhook(wrapped, signature, contentEncoding: "gzip", payloadEncoding: "base64");

            Encoding.UTF8.GetString(decoded).Should().Be(JSON_BODY);
        }

        [Test]
        public void VerifyAndDecodeWebhook_ThrowsOnSignatureMismatch()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var bogus = HmacHex("a-different-secret", raw);

            Action call = () => appClient.VerifyAndDecodeWebhook(raw, bogus);

            call.Should().Throw<StreamWebhookSignatureException>()
                .WithMessage("invalid webhook signature");
        }

        [Test]
        public void VerifyAndDecodeWebhook_RejectsGzipWhenSignatureIsOverCompressedBytes()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var gzipped = Gzip(raw);
            var signatureOverCompressed = HmacHex(API_SECRET, gzipped);

            Action call = () => appClient.VerifyAndDecodeWebhook(gzipped, signatureOverCompressed, contentEncoding: "gzip");

            call.Should().Throw<StreamWebhookSignatureException>()
                .WithMessage("invalid webhook signature");
        }

        [Test]
        public void VerifyAndDecodeWebhook_RejectsBase64GzipWhenSignatureIsOverWrappedBytes()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));
            var signatureOverWrapped = HmacHex(API_SECRET, wrapped);

            Action call = () => appClient.VerifyAndDecodeWebhook(wrapped, signatureOverWrapped, contentEncoding: "gzip", payloadEncoding: "base64");

            call.Should().Throw<StreamWebhookSignatureException>()
                .WithMessage("invalid webhook signature");
        }
    }
}

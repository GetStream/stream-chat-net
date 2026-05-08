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
    /// Unit tests for the webhook verification + parsing helpers added in
    /// <see cref="StreamChat.Clients.IAppClient"/>.
    /// </summary>
    /// <remarks>
    /// These tests do not extend <see cref="TestBase"/> on purpose - they
    /// must run fully offline because they cover SDK-only behaviour that
    /// does not touch the Stream API. Tests follow the arrange-act-assert
    /// pattern divided by empty lines.
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

        private static string Base64Wrap(byte[] input) => Convert.ToBase64String(input);

        private static string HmacHex(string secret, byte[] data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }

        [Test]
        public void VerifyWebhook_BackwardCompatibility_StillWorks()
        {
            var appClient = BuildAppClient();
            var signature = HmacHex(API_SECRET, Encoding.UTF8.GetBytes(JSON_BODY));

            var ok = appClient.VerifyWebhook(JSON_BODY, signature);
            var bad = appClient.VerifyWebhook(JSON_BODY, new string('0', 64));

            ok.Should().BeTrue();
            bad.Should().BeFalse();
        }

        [Test]
        public void VerifyAndParseWebhook_PlainBody()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);

            var ev = appClient.VerifyAndParseWebhook(raw, signature);

            ev.Type.Should().Be("message.new");
            ev.Message.Should().NotBeNull();
            ev.Message.Text.Should().Be("the quick brown fox");
        }

        [Test]
        public void VerifyAndParseWebhook_GzipBody()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);
            var gzipped = Gzip(raw);

            var ev = appClient.VerifyAndParseWebhook(gzipped, signature);

            ev.Type.Should().Be("message.new");
        }

        [Test]
        public void VerifyAndParseWebhook_ThrowsOnSignatureMismatch()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var bogus = HmacHex("a-different-secret", raw);

            Action call = () => appClient.VerifyAndParseWebhook(raw, bogus);

            call.Should().Throw<StreamWebhookSignatureException>()
                .WithMessage("invalid webhook signature");
        }

        [Test]
        public void VerifyAndParseWebhook_RejectsSignatureOverCompressedBytes()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var gzipped = Gzip(raw);
            var sigOverCompressed = HmacHex(API_SECRET, gzipped);

            Action call = () => appClient.VerifyAndParseWebhook(gzipped, sigOverCompressed);

            call.Should().Throw<StreamWebhookSignatureException>()
                .WithMessage("invalid webhook signature");
        }

        [Test]
        public void VerifyAndParseSqs_Base64Only()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);
            var wrapped = Base64Wrap(raw);

            var ev = appClient.VerifyAndParseSqs(wrapped, signature);

            ev.Type.Should().Be("message.new");
        }

        [Test]
        public void VerifyAndParseSqs_Base64PlusGzip()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);
            var wrapped = Base64Wrap(Gzip(raw));

            var ev = appClient.VerifyAndParseSqs(wrapped, signature);

            ev.Type.Should().Be("message.new");
        }

        [Test]
        public void VerifyAndParseSqs_RejectsSignatureOverWrappedBytes()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));
            var sigOverWrapped = HmacHex(API_SECRET, Encoding.ASCII.GetBytes(wrapped));

            Action call = () => appClient.VerifyAndParseSqs(wrapped, sigOverWrapped);

            call.Should().Throw<StreamWebhookSignatureException>()
                .WithMessage("invalid webhook signature");
        }

        [Test]
        public void VerifyAndParseSqs_ThrowsOnInvalidBase64()
        {
            var appClient = BuildAppClient();

            Action call = () => appClient.VerifyAndParseSqs("@@@-not-base64-@@@", "ignored");

            call.Should().Throw<StreamWebhookSignatureException>()
                .WithMessage("*base64-decode*");
        }

        [Test]
        public void VerifyAndParseSns_Base64PlusGzip()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);
            var wrapped = Base64Wrap(Gzip(raw));

            var ev = appClient.VerifyAndParseSns(wrapped, signature);

            ev.Type.Should().Be("message.new");
        }

        [Test]
        public void VerifyAndParseSns_MatchesSqs()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);
            var wrapped = Base64Wrap(Gzip(raw));

            var sns = appClient.VerifyAndParseSns(wrapped, signature);
            var sqs = appClient.VerifyAndParseSqs(wrapped, signature);

            sns.Type.Should().Be(sqs.Type);
            sns.Message.Text.Should().Be(sqs.Message.Text);
        }

        [Test]
        public void UngzipPayload_PassthroughPlainBytes()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);

            var output = WebhookHelpers.UngzipPayload(raw);

            output.Should().Equal(raw);
        }

        [Test]
        public void UngzipPayload_InflatesGzipBytes()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var gzipped = Gzip(raw);

            var output = WebhookHelpers.UngzipPayload(gzipped);

            output.Should().Equal(raw);
        }

        [Test]
        public void UngzipPayload_ThrowsOnInvalidGzipBody()
        {
            // Valid gzip header + deflate flags + bogus payload, so the magic
            // check passes but inflation fails with InvalidDataException.
            var bad = new byte[]
            {
                0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            };

            Action call = () => WebhookHelpers.UngzipPayload(bad);

            call.Should().Throw<StreamWebhookSignatureException>()
                .WithMessage("*decompress gzip*");
        }

        [Test]
        public void DecodeSqsPayload_Base64Only()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Convert.ToBase64String(raw);

            var output = WebhookHelpers.DecodeSqsPayload(wrapped);

            output.Should().Equal(raw);
        }

        [Test]
        public void DecodeSqsPayload_Base64PlusGzip()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Convert.ToBase64String(Gzip(raw));

            var output = WebhookHelpers.DecodeSqsPayload(wrapped);

            output.Should().Equal(raw);
        }

        [Test]
        public void DecodeSnsPayload_AliasesDecodeSqsPayload()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Convert.ToBase64String(Gzip(raw));

            var sns = WebhookHelpers.DecodeSnsPayload(wrapped);
            var sqs = WebhookHelpers.DecodeSqsPayload(wrapped);

            sns.Should().Equal(sqs);
        }

        [Test]
        public void VerifySignature_Matching()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var sig = HmacHex(API_SECRET, raw);

            WebhookHelpers.VerifySignature(raw, sig, API_SECRET).Should().BeTrue();
        }

        [Test]
        public void VerifySignature_Mismatched()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);

            WebhookHelpers.VerifySignature(raw, new string('0', 64), API_SECRET).Should().BeFalse();
        }

        [Test]
        public void ParseEvent_KnownType()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);

            var ev = WebhookHelpers.ParseEvent(raw);

            ev.Type.Should().Be("message.new");
            ev.Message.Text.Should().Be("the quick brown fox");
        }

        [Test]
        public void ParseEvent_UnknownTypeStillParses()
        {
            var raw = Encoding.UTF8.GetBytes("{\"type\":\"a.future.event\"}");

            var ev = WebhookHelpers.ParseEvent(raw);

            ev.Type.Should().Be("a.future.event");
        }

        [Test]
        public void ParseEvent_ThrowsOnMalformedJson()
        {
            var raw = Encoding.UTF8.GetBytes("not json");

            Action call = () => WebhookHelpers.ParseEvent(raw);

            call.Should().Throw<StreamWebhookSignatureException>()
                .WithMessage("*parse webhook event*");
        }
    }
}

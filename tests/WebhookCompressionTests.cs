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

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*signature mismatch*");
        }

        [Test]
        public void VerifyAndParseWebhook_RejectsSignatureOverCompressedBytes()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var gzipped = Gzip(raw);
            var sigOverCompressed = HmacHex(API_SECRET, gzipped);

            Action call = () => appClient.VerifyAndParseWebhook(gzipped, sigOverCompressed);

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*signature mismatch*");
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

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*signature mismatch*");
        }

        [Test]
        public void VerifyAndParseSqs_ThrowsOnInvalidBase64()
        {
            var appClient = BuildAppClient();

            Action call = () => appClient.VerifyAndParseSqs("@@@-not-base64-@@@", "ignored");

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*invalid base64 encoding*");
        }

        [Test]
        public void VerifyAndParseSns_PreExtractedMessage_Base64PlusGzip()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);
            var wrapped = Base64Wrap(Gzip(raw));

            var ev = appClient.VerifyAndParseSns(wrapped, signature);

            ev.Type.Should().Be("message.new");
        }

        [Test]
        public void VerifyAndParseSns_PreExtractedMessage_MatchesSqs()
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
        public void VerifyAndParseSns_FullEnvelope()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);
            var wrapped = Base64Wrap(Gzip(raw));
            var envelope = SnsEnvelope(wrapped);

            var ev = appClient.VerifyAndParseSns(envelope, signature);

            ev.Type.Should().Be("message.new");
        }

        [Test]
        public void VerifyAndParseSns_RejectsSignatureOverEnvelope()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));
            var envelope = SnsEnvelope(wrapped);
            var sigOverEnvelope = HmacHex(API_SECRET, Encoding.UTF8.GetBytes(envelope));

            Action call = () => appClient.VerifyAndParseSns(envelope, sigOverEnvelope);

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*signature mismatch*");
        }

        [Test]
        public void DecodeSnsPayload_UnwrapsFullEnvelope()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));
            var envelope = SnsEnvelope(wrapped);

            var output = WebhookHelpers.DecodeSnsPayload(envelope);

            output.Should().Equal(raw);
        }

        [Test]
        public void DecodeSnsPayload_EnvelopeWithLeadingWhitespace()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));
            var envelope = "\n  " + SnsEnvelope(wrapped);

            var output = WebhookHelpers.DecodeSnsPayload(envelope);

            output.Should().Equal(raw);
        }

        private static string SnsEnvelope(string innerMessage)
            => "{"
                + "\"Type\":\"Notification\","
                + "\"MessageId\":\"22b80b92-fdea-4c2c-8f9d-bdfb0c7bf324\","
                + "\"TopicArn\":\"arn:aws:sns:us-east-1:123456789012:stream-webhooks\","
                + "\"Message\":\"" + innerMessage + "\","
                + "\"Timestamp\":\"2026-05-11T10:00:00.000Z\","
                + "\"SignatureVersion\":\"1\","
                + "\"MessageAttributes\":{\"X-Signature\":{\"Type\":\"String\",\"Value\":\"placeholder\"}}"
                + "}";

        [Test]
        public void GunzipPayload_PassthroughPlainBytes()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);

            var output = WebhookHelpers.GunzipPayload(raw);

            output.Should().Equal(raw);
        }

        [Test]
        public void GunzipPayload_InflatesGzipBytes()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var gzipped = Gzip(raw);

            var output = WebhookHelpers.GunzipPayload(gzipped);

            output.Should().Equal(raw);
        }

        [Test]
        public void GunzipPayload_ThrowsOnInvalidGzipBody()
        {
            // Valid gzip header + deflate flags + bogus payload, so the magic
            // check passes but inflation fails with InvalidDataException.
            var bad = new byte[]
            {
                0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            };

            Action call = () => WebhookHelpers.GunzipPayload(bad);

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*gzip decompression failed*");
        }

        [Test]
        public void GunzipPayload_HelloWorldFixture()
        {
            var gzipped = Convert.FromBase64String("H4sIAGrYAWoAA8tIzcnJL88vykkBAK0g6/kKAAAA");

            var output = WebhookHelpers.GunzipPayload(gzipped);

            output.Should().Equal(Encoding.UTF8.GetBytes("helloworld"));
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
        public void DecodeSqsPayload_HelloWorldBase64Fixture()
        {
            var output = WebhookHelpers.DecodeSqsPayload("aGVsbG93b3JsZA==");

            output.Should().Equal(Encoding.UTF8.GetBytes("helloworld"));
        }

        [Test]
        public void DecodeSqsPayload_HelloWorldBase64GzipFixture()
        {
            var output = WebhookHelpers.DecodeSqsPayload("H4sIAGrYAWoAA8tIzcnJL88vykkBAK0g6/kKAAAA");

            output.Should().Equal(Encoding.UTF8.GetBytes("helloworld"));
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

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*invalid JSON payload*");
        }

        [Test]
        public void Factory_VerifyAndParseWebhook_DelegatesToAppClient()
        {
            var factory = new StreamClientFactory(API_KEY, API_SECRET);
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var signature = HmacHex(API_SECRET, raw);

            var viaFactory = factory.VerifyAndParseWebhook(Gzip(raw), signature);
            var viaAppClient = factory.GetAppClient().VerifyAndParseWebhook(Gzip(raw), signature);

            viaFactory.Type.Should().Be(viaAppClient.Type);
            viaFactory.Type.Should().Be("message.new");
        }

        [Test]
        public void Factory_VerifyAndParseSqs_DelegatesToAppClient()
        {
            var factory = new StreamClientFactory(API_KEY, API_SECRET);
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));
            var signature = HmacHex(API_SECRET, raw);

            var ev = factory.VerifyAndParseSqs(wrapped, signature);

            ev.Type.Should().Be("message.new");
        }

        [Test]
        public void Factory_VerifyAndParseSns_DelegatesToAppClient()
        {
            var factory = new StreamClientFactory(API_KEY, API_SECRET);
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));
            var signature = HmacHex(API_SECRET, raw);

            var ev = factory.VerifyAndParseSns(wrapped, signature);

            ev.Type.Should().Be("message.new");
        }

        [Test]
        public void Factory_VerifyAndParseWebhook_RejectsMismatchedSignature()
        {
            var factory = new StreamClientFactory(API_KEY, API_SECRET);
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);

            Action call = () => factory.VerifyAndParseWebhook(raw, new string('0', 64));

            call.Should().Throw<StreamInvalidWebhookException>();
        }

        [Test]
        public void DecodeSqsPayload_ThrowsOnInvalidBase64()
        {
            Action call = () => WebhookHelpers.DecodeSqsPayload("@@@-not-base64-@@@");

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*invalid base64 encoding*");
        }

        [Test]
        public void GunzipPayload_ThrowsOnCorruptGzip()
        {
            // Valid gzip magic + header so the magic check passes, then garbage
            // so the deflate stream fails.
            var bad = new byte[]
            {
                0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            };

            Action call = () => WebhookHelpers.GunzipPayload(bad);

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*gzip decompression failed*");
        }

        [Test]
        public void VerifyAndParseInternal_ThrowsOnInvalidJson()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes("not json");
            var signature = HmacHex(API_SECRET, raw);

            Action call = () => appClient.VerifyAndParseWebhook(raw, signature);

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*invalid JSON payload*");
        }
        [Test]
        public void VerifyAndParseSqs_WithoutSignature_Parses_Plain()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(raw);

            var ev = WebhookHelpers.VerifyAndParseSqs(wrapped);

            ev.Type.Should().Be("message.new");
            ev.Message.Text.Should().Be("the quick brown fox");
        }

        [Test]
        public void VerifyAndParseSqs_WithoutSignature_Parses_Base64()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(raw);

            var ev = WebhookHelpers.VerifyAndParseSqs(wrapped);

            ev.Type.Should().Be("message.new");
        }

        [Test]
        public void VerifyAndParseSqs_WithoutSignature_Parses_Base64Gzip()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));

            var ev = WebhookHelpers.VerifyAndParseSqs(wrapped);

            ev.Type.Should().Be("message.new");
            ev.Message.Text.Should().Be("the quick brown fox");
        }

        [Test]
        public void VerifyAndParseSns_WithoutSignature_Parses_PreExtractedMessage()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));

            var ev = WebhookHelpers.VerifyAndParseSns(wrapped);

            ev.Type.Should().Be("message.new");
            ev.Message.Text.Should().Be("the quick brown fox");
        }

        [Test]
        public void VerifyAndParseSns_WithoutSignature_Parses_FullEnvelope()
        {
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var envelope = SnsEnvelope(Base64Wrap(Gzip(raw)));

            var ev = WebhookHelpers.VerifyAndParseSns(envelope);

            ev.Type.Should().Be("message.new");
            ev.Message.Text.Should().Be("the quick brown fox");
        }

        [Test]
        public void AppClient_VerifyAndParseSqs_WithoutSignature_Parses()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var wrapped = Base64Wrap(Gzip(raw));

            var ev = appClient.VerifyAndParseSqs(wrapped);

            ev.Type.Should().Be("message.new");
            ev.Message.Text.Should().Be("the quick brown fox");
        }

        [Test]
        public void AppClient_VerifyAndParseSns_WithoutSignature_Parses()
        {
            var appClient = BuildAppClient();
            var raw = Encoding.UTF8.GetBytes(JSON_BODY);
            var envelope = SnsEnvelope(Base64Wrap(Gzip(raw)));

            var ev = appClient.VerifyAndParseSns(envelope);

            ev.Type.Should().Be("message.new");
        }

        [Test]
        public void VerifyAndParseSqs_ThrowsOnPartialCreds_SignatureOnly()
        {
            Action call = () => WebhookHelpers.VerifyAndParseSqs("body", "sig", null);

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*signature and secret must both be provided*");
        }

        [Test]
        public void VerifyAndParseSqs_ThrowsOnPartialCreds_SecretOnly()
        {
            Action call = () => WebhookHelpers.VerifyAndParseSqs("body", null, "secret");

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*signature and secret must both be provided*");
        }

        [Test]
        public void VerifyAndParseSns_ThrowsOnPartialCreds_SignatureOnly()
        {
            Action call = () => WebhookHelpers.VerifyAndParseSns("body", "sig", null);

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*signature and secret must both be provided*");
        }

        [Test]
        public void VerifyAndParseSns_ThrowsOnPartialCreds_SecretOnly()
        {
            Action call = () => WebhookHelpers.VerifyAndParseSns("body", null, "secret");

            call.Should().Throw<StreamInvalidWebhookException>()
                .WithMessage("*signature and secret must both be provided*");
        }
    }
}

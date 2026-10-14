using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using StreamChat.Clients;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>
    /// Request and response shape tests for the scheduled message methods of <see cref="MessageClient"/>.
    /// They run against a stubbed HTTP handler, so they need no Stream credentials;
    /// the round trip against a live app is covered in <see cref="MessageClientTests"/>.
    /// </summary>
    [TestFixture]
    public class ScheduledMessageRequestTests
    {
        private const string ScheduledMessageJson = @"{
            ""scheduled_message"": {
                ""id"": ""sm-1"",
                ""cid"": ""messaging:general"",
                ""user"": { ""id"": ""jane"" },
                ""message"": { ""text"": ""later"", ""mentioned_users"": [""bob""], ""color"": ""red"" },
                ""scheduled_at"": ""2030-01-02T03:04:05Z"",
                ""status"": ""failed"",
                ""failure_reason"": ""channel_frozen"",
                ""created_at"": ""2029-12-31T00:00:00Z"",
                ""updated_at"": ""2029-12-31T00:00:00Z""
            },
            ""duration"": ""1ms""
        }";

        private CapturingHandler _handler;
        private IMessageClient _client;

        [SetUp]
        public void Setup()
        {
            _handler = new CapturingHandler();
            var factory = new StreamClientFactory("key", "secret", opts =>
            {
                opts.HttpClient = new HttpClient(_handler);
                opts.BaseUrl = new Uri("https://chat.stream-io-api.com");
            });
            _client = factory.GetMessageClient();
        }

        [Test]
        public async Task TestScheduleMessageSendsScheduledAtAsync()
        {
            _handler.Respond(HttpStatusCode.Created, ScheduledMessageJson);
            var scheduledAt = new DateTimeOffset(2030, 1, 2, 3, 4, 5, TimeSpan.Zero);

            var resp = await _client.ScheduleMessageAsync("messaging", "general", new MessageRequest { Text = "later" }, "jane", scheduledAt,
                new SendMessageOptions { SkipPush = true });

            _handler.Method.Should().Be(HttpMethod.Post);
            _handler.Path.Should().Be("/channels/messaging/general/message");
            var body = ParseBody(_handler.Body);
            body["message"]["text"].Value<string>().Should().Be("later");
            body["message"]["user_id"].Value<string>().Should().Be("jane");
            body["skip_push"].Value<bool>().Should().BeTrue();
            body.ContainsKey("is_pending_message").Should().BeFalse("the API rejects scheduled_at with any pending field");
            DateTimeOffset.Parse(body["scheduled_at"].Value<string>()).Should().Be(scheduledAt);
            resp.ScheduledMessage.Id.Should().Be("sm-1");
        }

        [Test]
        public async Task TestSendMessageWithoutScheduledAtOmitsItAsync()
        {
            _handler.Respond(HttpStatusCode.Created, @"{ ""message"": { ""id"": ""m-1"" } }");

            var resp = await _client.SendMessageAsync("messaging", "general", "jane", "now");

            JObject.Parse(_handler.Body).ContainsKey("scheduled_at").Should().BeFalse();
            resp.ScheduledMessage.Should().BeNull();
        }

        [Test]
        public async Task TestUpdateScheduledMessageAsync()
        {
            _handler.Respond(HttpStatusCode.OK, ScheduledMessageJson);
            var scheduledAt = new DateTimeOffset(2030, 1, 2, 3, 4, 5, TimeSpan.Zero);

            await _client.UpdateScheduledMessageAsync("sm-1", new UpdateScheduledMessageRequest
            {
                Message = new MessageRequest { Text = "edited" },
                ScheduledAt = scheduledAt,
            });

            _handler.Method.Should().Be(new HttpMethod("PATCH"));
            _handler.Path.Should().Be("/scheduled_messages/sm-1");
            var body = ParseBody(_handler.Body);
            body["message"]["text"].Value<string>().Should().Be("edited");
            DateTimeOffset.Parse(body["scheduled_at"].Value<string>()).Should().Be(scheduledAt);
        }

        [Test]
        public async Task TestUpdateScheduledMessageSendsOnlyGivenFieldsAsync()
        {
            _handler.Respond(HttpStatusCode.OK, ScheduledMessageJson);

            await _client.UpdateScheduledMessageAsync("sm-1", new UpdateScheduledMessageRequest
            {
                ScheduledAt = new DateTimeOffset(2030, 1, 2, 3, 4, 5, TimeSpan.Zero),
            });

            var body = ParseBody(_handler.Body);
            body.ContainsKey("message").Should().BeFalse();
            body.ContainsKey("scheduled_at").Should().BeTrue();
        }

        [Test]
        public async Task TestDeleteScheduledMessageAsync()
        {
            _handler.Respond(HttpStatusCode.OK, ScheduledMessageJson);

            var resp = await _client.DeleteScheduledMessageAsync("sm-1");

            _handler.Method.Should().Be(HttpMethod.Delete);
            _handler.Path.Should().Be("/scheduled_messages/sm-1");
            resp.ScheduledMessage.Id.Should().Be("sm-1");
        }

        [Test]
        public async Task TestSendScheduledMessageNowAsync()
        {
            _handler.Respond(HttpStatusCode.Created, ScheduledMessageJson);

            var resp = await _client.SendScheduledMessageNowAsync("sm-1");

            _handler.Method.Should().Be(HttpMethod.Post);
            _handler.Path.Should().Be("/scheduled_messages/sm-1/send_now");
            resp.ScheduledMessage.Id.Should().Be("sm-1");
        }

        [Test]
        public async Task TestQueryScheduledMessagesAsync()
        {
            _handler.Respond(HttpStatusCode.Created,
                @"{ ""scheduled_messages"": [ { ""id"": ""sm-1"", ""channel"": { ""cid"": ""messaging:general"", ""type"": ""messaging"", ""id"": ""general"" } } ], ""next"": ""cursor-2"" }");

            var resp = await _client.QueryScheduledMessagesAsync(new QueryScheduledMessagesRequest
            {
                UserId = "jane",
                Filter = new ScheduledMessagesFilter
                {
                    Cid = "messaging:general",
                    Status = new List<string> { ScheduledMessageStatus.Scheduled, ScheduledMessageStatus.Failed },
                },
                Sort = new List<SortParameter> { new SortParameter { Field = "scheduled_at", Direction = SortDirection.Ascending } },
                Limit = 10,
                Next = "cursor-1",
            });

            _handler.Method.Should().Be(HttpMethod.Post);
            _handler.Path.Should().Be("/scheduled_messages/query");
            JToken.DeepEquals(JObject.Parse(_handler.Body), JObject.Parse(@"{
                ""user_id"": ""jane"",
                ""filter"": { ""cid"": ""messaging:general"", ""status"": [""scheduled"", ""failed""] },
                ""sort"": [ { ""field"": ""scheduled_at"", ""direction"": 1 } ],
                ""limit"": 10,
                ""next"": ""cursor-1""
            }")).Should().BeTrue(_handler.Body);
            resp.ScheduledMessages.Should().ContainSingle();
            resp.ScheduledMessages[0].Channel.Id.Should().Be("general");
            resp.Next.Should().Be("cursor-2");
        }

        [Test]
        public async Task TestScheduledMessageResponseIsDeserializedAsync()
        {
            _handler.Respond(HttpStatusCode.OK, ScheduledMessageJson);

            var sm = (await _client.DeleteScheduledMessageAsync("sm-1")).ScheduledMessage;

            sm.Cid.Should().Be("messaging:general");
            sm.User.Id.Should().Be("jane");
            sm.Message.Text.Should().Be("later");
            sm.Message.MentionedUsers.Should().Equal("bob");
            sm.Message.GetData<string>("color").Should().Be("red");
            sm.ScheduledAt.Should().Be(new DateTimeOffset(2030, 1, 2, 3, 4, 5, TimeSpan.Zero));
            sm.Status.Should().Be(ScheduledMessageStatus.Failed);
            sm.FailureReason.Should().Be(ScheduledMessageFailureReason.ChannelFrozen);
        }

        [Test]
        public void TestScheduledMessageSentWebhookIsParsed()
        {
            var payload = Encoding.UTF8.GetBytes(@"{
                ""type"": ""scheduled_message.sent"",
                ""cid"": ""messaging:general"",
                ""message_id"": ""m-1"",
                ""scheduled_message"": { ""id"": ""sm-1"", ""status"": ""sent"" }
            }");

            var ev = WebhookHelpers.ParseEvent(payload);

            ev.Type.Should().Be("scheduled_message.sent");
            ev.MessageId.Should().Be("m-1");
            ev.ScheduledMessage.Id.Should().Be("sm-1");
            ev.ScheduledMessage.Status.Should().Be(ScheduledMessageStatus.Sent);
        }

        // Keeps dates as the strings that were sent instead of converting them to local DateTime.
        private static JObject ParseBody(string body)
            => JObject.Load(new JsonTextReader(new StringReader(body)) { DateParseHandling = DateParseHandling.None });

        private class CapturingHandler : HttpMessageHandler
        {
            private HttpStatusCode _status;
            private string _responseBody;

            public HttpMethod Method { get; private set; }
            public string Path { get; private set; }
            public string Body { get; private set; }

            public void Respond(HttpStatusCode status, string body)
            {
                _status = status;
                _responseBody = body;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Method = request.Method;
                Path = request.RequestUri.AbsolutePath;
                Body = request.Content == null ? null : await request.Content.ReadAsStringAsync();
                return new HttpResponseMessage(_status)
                {
                    Content = new StringContent(_responseBody, Encoding.UTF8, "application/json"),
                };
            }
        }
    }
}

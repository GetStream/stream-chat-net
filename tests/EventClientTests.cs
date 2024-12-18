using System;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="EventClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class EventClientTests : TestBase
    {
        private UserRequest _user;
        private ChannelWithConfig _channel;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            _user = await UpsertNewUserAsync();
            _channel = await CreateChannelAsync(createdByUserId: _user.Id, members: new[] { _user.Id });
        }

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
            await TryDeleteUsersAsync(_user.Id);
        }

        [Test]
        public async Task TestSendEventAsync()
        {
            var expectedEvent = new Event { Type = "typing.start", UserId = _user.Id };
            expectedEvent.SetData("foo", new[] { 1 });

            var resp = await _eventClient.SendEventAsync(_channel.Type, _channel.Id, expectedEvent);

            resp.Event.CreatedAt.Should().NotBeNull();
            resp.Event.User.Should().NotBeNull();
            resp.Event.Type.Should().BeEquivalentTo(expectedEvent.Type);
            resp.Event.User.Id.Should().BeEquivalentTo(_user.Id);
            resp.Event.GetData<int[]>("foo")[0].Should().Be(expectedEvent.GetData<int[]>("foo")[0]);
        }

        [Test]
        public void TestDeserializingReceivedEventAsync()
        {
            var messageDeletedEventJson = @"
{
  ""cid"": ""messaging:fun"",
  ""type"": ""message.deleted"",
  ""message"": {
    ""id"": ""268d121f-82e0-4de1-8c8b-ef1201efd7a3"",
    ""text"": ""new stuff"",
    ""html"": ""<p>new stuff</p>\n"",
    ""type"": ""deleted"",
    ""user"": {
      ""id"": ""76cd8430-2f91-4059-90e5-02dffb910297"",
      ""role"": ""user"",
      ""created_at"": ""2019-04-24T09:44:21.390868Z"",
      ""updated_at"": ""2019-04-24T09:44:22.537305Z"",
      ""last_active"": ""2019-04-24T09:44:22.535872Z"",
      ""online"": false
    },
    ""attachments"": [],
    ""latest_reactions"": [],
    ""own_reactions"": [],
    ""reaction_counts"": {},
    ""reply_count"": 0,
    ""created_at"": ""2019-04-24T09:44:22.57073Z"",
    ""updated_at"": ""2019-04-24T09:44:22.717078Z"",
    ""deleted_at"": ""2019-04-24T09:44:22.730524Z"",
    ""mentioned_users"": []
  },
  ""created_at"": ""2019-04-24T09:44:22.733305Z"",
  ""request_info"": {
    ""type"": ""client"",
    ""ip"": ""86.84.2.2"",
    ""user_agent"": ""Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:109.0) Gecko/20100101 Firefox/117.0"",
    ""sdk"": ""stream-chat-react-10.11.0-stream-chat-javascript-client-browser-8.12.1""
  }
}";

            var deserializeObject = JsonConvert.DeserializeObject<EventResponse>(messageDeletedEventJson);
        }

        [Test]
        public async Task TestSendCustomUserEventAsync()
        {
            var customEvent = new UserCustomEvent { Type = "friendship_request" };
            customEvent.SetData("text", "Test field");

            Func<Task> eventTask = () => _eventClient.SendUserCustomEventAsync(_user.Id, customEvent);

            await eventTask.Should().NotThrowAsync();
        }
    }
}
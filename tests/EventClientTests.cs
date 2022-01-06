using System.Threading.Tasks;
using FluentAssertions;
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
        public async Task TearownAsync()
        {
            await TryDeleteChannelAsync(_channel.Cid);
            await TryDeleteUsersAsync(_user.Id);
        }

        [Test]
        public async Task TestSendEventAsync()
        {
            var expectedEvent = new Event { Type = EventType.TypingStart, UserId = _user.Id };
            expectedEvent.SetData("foo", new[] { 1 });

            var resp = await _eventClient.SendEventAsync(_channel.Type, _channel.Id, expectedEvent);

            resp.Event.CreatedAt.Should().NotBeNull();
            resp.Event.User.Should().NotBeNull();
            resp.Event.Type.Should().BeEquivalentTo(expectedEvent.Type);
            resp.Event.User.Id.Should().BeEquivalentTo(_user.Id);
            resp.Event.GetData<int[]>("foo")[0].Should().Be(expectedEvent.GetData<int[]>("foo")[0]);
        }
    }
}
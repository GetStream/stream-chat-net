using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="ReactionClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class ReactionClientTests : TestBase
    {
        private UserRequest _user;
        private ChannelWithConfig _channel;
        private Message _message;

        [OneTimeSetUp]
        public async Task OneTimeSetupAsync()
        {
            _user = await UpsertNewUserAsync();
            _channel = await CreateChannelAsync(createdByUserId: _user.Id, members: new[] { _user.Id });
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDownAsync()
        {
            await TryDeleteUsersAsync(_user.Id);
        }

        [SetUp]
        public async Task SetupAsync()
        {
            var resp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user.Id, Guid.NewGuid().ToString());
            _message = resp.Message;
        }

        [Test]
        public async Task TestSendReactionAsync()
        {
            var expectedReaction = new ReactionRequest { Type = "like", UserId = _user.Id };
            expectedReaction.SetData("foo", "bar");

            var actualReaction = await _reactionClient.SendReactionAsync(_message.Id, expectedReaction);

            actualReaction.Message.Id.Should().BeEquivalentTo(_message.Id);
            actualReaction.Message.ReactionCounts["like"].Should().Be(1);
            actualReaction.Message.LatestReactions.Count.Should().Be(1);
            actualReaction.Reaction.Type.Should().BeEquivalentTo(expectedReaction.Type);
            actualReaction.Reaction.CreatedAt.Should().NotBeNull();
            actualReaction.Reaction.User.Id.Should().BeEquivalentTo(_user.Id);
            actualReaction.Reaction.MessageId.Should().BeEquivalentTo(_message.Id);
            actualReaction.Reaction.GetData<string>("foo").Should().BeEquivalentTo(expectedReaction.GetData<string>("foo"));

            var resp = await _reactionClient.GetReactionsAsync(_message.Id);
            resp.Reactions.Count.Should().Be(1);
        }

        [Test]
        public async Task TestDeleteReactionAsync()
        {
            var actualReaction = await _reactionClient.SendReactionAsync(_message.Id, "like", _user.Id);

            await _reactionClient.DeleteReactionAsync(_message.Id, "like", _user.Id);

            var resp = await _reactionClient.GetReactionsAsync(_message.Id);
            resp.Reactions.Should().BeEmpty();
        }
    }
}
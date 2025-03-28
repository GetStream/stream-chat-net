using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Clients;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="ModerationClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class ModerationClientTests : TestBase
    {
        [Test]
        public async Task TestCheckUserProfileAsync_WithInappropriateUsernameAndImage()
        {
            var response = await _moderationClient.CheckUserProfileAsync(Guid.NewGuid().ToString(), new UserProfileCheckRequest
            {
                Username = "fucking_bitch_001",
                Image = "https://github.com/user-attachments/assets/b5ea60fd-e5dd-4d5e-8d72-013941358865",
            });

            response.RecommendedAction.Should().Be("remove");
        }

        [Test]
        public async Task TestCheckUserProfileAsync_WithInappropriateUsername()
        {
            var response = await _moderationClient.CheckUserProfileAsync(Guid.NewGuid().ToString(), new UserProfileCheckRequest
            {
                Username = "fucking_bitch_001",
            });

            response.RecommendedAction.Should().Be("remove");
        }

        [Test]
        public async Task TestCheckUserProfileAsync_WithInappropriateImage()
        {
            var response = await _moderationClient.CheckUserProfileAsync(Guid.NewGuid().ToString(), new UserProfileCheckRequest
            {
                Image = "https://github.com/user-attachments/assets/b5ea60fd-e5dd-4d5e-8d72-013941358865",
            });

            response.RecommendedAction.Should().Be("remove");
        }

        [Test]
        public async Task TestCheckUserProfileAsync_WithAppropriateUsername()
        {
            var response = await _moderationClient.CheckUserProfileAsync(Guid.NewGuid().ToString(), new UserProfileCheckRequest
            {
                Username = "avenger_001",
            });

            response.RecommendedAction.Should().Be("keep");
        }

        [Test]
        public void TestCheckUserProfileAsync_WithNoUsernameAndNoImage()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _moderationClient.CheckUserProfileAsync(Guid.NewGuid().ToString(), new UserProfileCheckRequest()));
        }
    }
}
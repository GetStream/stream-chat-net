using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="ChannelTypeClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// Note: Create and Delete methods are not tested explicitly, because
    /// we use them in the setup and teardown already.
    /// </remarks>
    [TestFixture]
    public class ChannelTypeClientTests : TestBase
    {
        private ChannelTypeWithStringCommandsResponse _channelType;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            _channelType = await _channelTypeClient.CreateChannelTypeAsync(new ChannelTypeWithStringCommandsRequest
            {
                Name = Guid.NewGuid().ToString(),
            });
        }

        [OneTimeTearDown]
        public async Task TeardownAsync()
        {
            try
            {
                await WaitForAsync(async () =>
                {
                    try
                    {
                        await _channelTypeClient.DeleteChannelTypeAsync(_channelType.Name);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch (TimeoutException)
            {
            }
        }

        [Test]
        public async Task TestGetChannelTypeAsync()
        {
            var actualChannelType = await _channelTypeClient.GetChannelTypeAsync(_channelType.Name);

            actualChannelType.Name.Should().BeEquivalentTo(_channelType.Name);
        }

        [Test]
        public async Task TestListChannelTypeAsync()
        {
            var resp = await _channelTypeClient.ListChannelTypesAsync();

            resp.ChannelTypes.Should().ContainKey(_channelType.Name);
        }

        [Test]
        public async Task TestUpdateChannelTypeAsync()
        {
            var expectedAutmod = Autmod.Simple;

            await WaitForAsync(async () =>
            {
                try
                {
                    var updated = await _channelTypeClient.UpdateChannelTypeAsync(_channelType.Name, new ChannelTypeWithStringCommandsRequest
                    {
                        Automod = expectedAutmod,
                    });

                    return updated.Automod == expectedAutmod;
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}
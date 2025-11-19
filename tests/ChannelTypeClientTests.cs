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
        public Task TestGetChannelTypeAsync()
            => TryMultipleAsync(testBody: async () =>
            {
                var actualChannelType = await _channelTypeClient.GetChannelTypeAsync(_channelType.Name);
                actualChannelType.Name.Should().BeEquivalentTo(_channelType.Name);
            });

        [Test]
        public async Task TestListChannelTypeAsync()
        {
            var resp = await _channelTypeClient.ListChannelTypesAsync();

            resp.ChannelTypes.Should().ContainKey(_channelType.Name);
        }

        [Test]
        public async Task TestUpdateChannelTypeAsync()
        {
            var expectedAutomod = Automod.Simple;

            await WaitForAsync(async () =>
            {
                try
                {
                    var updated = await _channelTypeClient.UpdateChannelTypeAsync(_channelType.Name,
                        new ChannelTypeWithStringCommandsRequest
                        {
                            Automod = expectedAutomod,
                        });

                    return updated.Automod == expectedAutomod;
                }
                catch
                {
                    return false;
                }
            });
        }

        [Test]
        public async Task TestUserMessageRemindersFieldOnChannelTypeAsync()
        {
            var channelTypeName = Guid.NewGuid().ToString();
            ChannelTypeWithStringCommandsResponse createdChannelType = null;

            try
            {
                // Create a basic channel type (without user_message_reminders enabled)
                // We're testing that the field exists in the model, not that it can be enabled
                // (enabling requires Push V3 which may not be configured in test environment)
                createdChannelType = await _channelTypeClient.CreateChannelTypeAsync(
                    new ChannelTypeWithStringCommandsRequest
                    {
                        Name = channelTypeName,
                    });

                // Retrieve the channel type
                await WaitForAsync(async () =>
                {
                    try
                    {
                        var retrieved = await _channelTypeClient.GetChannelTypeAsync(channelTypeName);
                        return retrieved.Name == channelTypeName;
                    }
                    catch
                    {
                        return false;
                    }
                });

                // Test that the field can be set to false (should work without Push V3)
                var updated = await _channelTypeClient.UpdateChannelTypeAsync(channelTypeName,
                    new ChannelTypeWithStringCommandsRequest
                    {
                        UserMessageReminders = false,
                    });

                // Verify the field is accessible in the response (even if false)
                updated.UserMessageReminders.Should().NotBeNull();

                // Verify the update persisted and field is accessible
                await WaitForAsync(async () =>
                {
                    try
                    {
                        var retrieved = await _channelTypeClient.GetChannelTypeAsync(channelTypeName);
                        // The field should be present in the response
                        return retrieved.UserMessageReminders != null;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            finally
            {
                // Cleanup
                if (createdChannelType != null)
                {
                    try
                    {
                        await WaitForAsync(async () =>
                        {
                            try
                            {
                                await _channelTypeClient.DeleteChannelTypeAsync(channelTypeName);
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
                        // Ignore cleanup failures
                    }
                }
            }
        }
    }
}
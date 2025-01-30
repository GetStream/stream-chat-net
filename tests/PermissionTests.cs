using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Clients;
using StreamChat.Exceptions;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="PermissionClient"/></summary>
    /// <remarks>
    /// <para>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </para>
    /// Even if we create/update/delete a new role, we still need
    /// to wait a few seconds for the changes to propagate in the backend.
    /// For this reason, we wrote just a single test to test all functionalites at once.
    /// </remarks>
    public class PermissionTests : TestBase
    {
        private const string TestPermissionDescription = "Test Permission";

        private UserRequest _user1;
        private UserRequest _user2;

        [OneTimeSetUp]
        [OneTimeTearDown]
        public async Task CleanupAsync()
        {
            await DeleteCustomRolesAsync();
            await DeleteCustomPermissionsAsync();
        }

        [SetUp]
        public async Task SetupAsync()
        {
            (_user1, _user2) = (await UpsertNewUserAsync(), await UpsertNewUserAsync());
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            await TryDeleteUsersAsync(_user1.Id, _user2.Id);
        }

        private async Task DeleteCustomRolesAsync()
        {
            var roleResp = await _permissionClient.ListRolesAsync();
            foreach (var role in roleResp.Roles.Where(x => x.Custom))
            {
                try
                {
                    await _permissionClient.DeleteRoleAsync(role.Name);
                }
                catch
                {
                    // It throws 'role does not exist' exception which
                    // is obviously nonsense. So let's ignore it.
                }
            }
        }

        private async Task DeleteCustomPermissionsAsync()
        {
            var permResp = await _permissionClient.ListPermissionsAsync();
            foreach (var perm in permResp.Permissions.Where(x => x.Description == TestPermissionDescription))
            {
                try
                {
                    await _permissionClient.DeletePermissionAsync(perm.Id);
                }
                catch
                {
                    // It throws 'not found' exception which
                    // is obviously nonsense. So let's ignore it.
                }
            }
        }

        [Test]
        public async Task TestRolesEnd2EndAsync()
        {
            // Test create
            var roleResp = await _permissionClient.CreateRoleAsync(Guid.NewGuid().ToString());

            roleResp.Role.Name.Should().NotBeNullOrEmpty();
            roleResp.Role.Custom.Should().BeTrue();
            roleResp.Role.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));
            roleResp.Role.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));

            // Test list
            var rolesResp = await _permissionClient.ListRolesAsync();
            rolesResp.Roles.Should().NotBeEmpty();

            // Test delete
            try
            {
                await _permissionClient.DeleteRoleAsync(roleResp.Role.Name);
            }
            catch (StreamChatException ex)
            {
                if (ex.Message.Contains("does not exist"))
                {
                    // Unfortounatly, the backend is too slow to propagate the role creation
                    // so this error message is expected. Facepalm.
                    return;
                }

                throw;
            }
        }

        [Test]
        public async Task TestPermissionsEnd2EndAsync()
        {
            var permission = new Permission
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Description = TestPermissionDescription,
                Action = "DeleteChannel",
                Condition = new Dictionary<string, object>
                {
                    {
                        "$subject.magic_custom_field", new Dictionary<string, string>
                        {
                            { "$eq", "true" },
                        }
                    },
                },
            };

            // Test create
            await _permissionClient.CreatePermissionAsync(permission);

            // Test Get
            var permResponse = await _permissionClient.GetPermissionAsync("upload-attachment-owner");
            permResponse.Permission.Id.Should().NotBeNull();
            permResponse.Permission.Name.Should().NotBeNull();
            permResponse.Permission.Description.Should().NotBeNull();
            permResponse.Permission.Action.Should().NotBeNull();
            permResponse.Permission.Condition.Should().BeNull();

            // Test list
            var listResp = await _permissionClient.ListPermissionsAsync();
            listResp.Permissions.Should().NotBeEmpty();

            // Test delete
            try
            {
                await _permissionClient.DeletePermissionAsync(permission.Id);
            }
            catch (StreamChatException ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    // Unfortunately, the backend is too slow to propagate the permission creation
                    // so this error message is expected. Facepalm.
                    return;
                }

                throw;
            }
        }

        [Test]
        public async Task WhenUpdatingChannelGrantsExpectChannelGrantsChanged()
        {
            ChannelTypeWithStringCommandsResponse tempChannelType = null;
            try
            {
                tempChannelType = await _channelTypeClient.CreateChannelTypeAsync(
                    new ChannelTypeWithStringCommandsRequest()
                    {
                        Name = Guid.NewGuid().ToString(),
                    });

                // Expect delete-message-owner to not be present by default
                tempChannelType.Grants.First(g => g.Key == "channel_member").Value.Should()
                    .NotContain("delete-message-owner");

                var update = new ChannelTypeWithStringCommandsRequest
                {
                    Grants = new Dictionary<string, List<string>>
                    {
                        {
                            "channel_member", new List<string>
                            {
                                "delete-message-owner",
                            }
                        },
                    },
                };
                await TryMultipleAsync(() => _channelTypeClient.UpdateChannelTypeAsync(tempChannelType.Name, update));

                var getChannelType2 = await _channelTypeClient.GetChannelTypeAsync(tempChannelType.Name);

                // Expect delete-message-owner to not be present by default
                var channelMemberGrants = getChannelType2.Grants.First(g => g.Key == "channel_member").Value;
                channelMemberGrants.Should().HaveCount(1);
                channelMemberGrants.Should().Contain("delete-message-owner");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                try
                {
                    if (tempChannelType != null)
                    {
                        await _channelTypeClient.DeleteChannelTypeAsync(tempChannelType.Name);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        [Test]
        public async Task WhenUpdatingGrantsWithEmptyListExpectResetToDefault()
        {
            var tempChannelType = await _channelTypeClient.CreateChannelTypeAsync(
                new ChannelTypeWithStringCommandsRequest
                {
                    Name = Guid.NewGuid().ToString(),
                });

            var channelMemberInitialGrantsCounts
                = tempChannelType.Grants.First(g => g.Key == "channel_member").Value.Count;

            // We expect more than 1 grant by default
            channelMemberInitialGrantsCounts.Should().NotBe(1);

            // Wait for data propagation - channel type is sometimes not present immediately after creation
            await WaitForAsync(async () =>
            {
                try
                {
                    var channelType = await _channelTypeClient.GetChannelTypeAsync(tempChannelType.Name);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });

            // Override channel_members grants to replace with a single grant
            var updateGrants = new ChannelTypeWithStringCommandsRequest
            {
                Grants = new Dictionary<string, List<string>>
                {
                    {
                        "channel_member", new List<string>
                        {
                            "delete-message-owner",
                        }
                    },
                },
            };
            var updateChannelTypeResponse
                = await _channelTypeClient.UpdateChannelTypeAsync(tempChannelType.Name, updateGrants);

            // Confirm a single grant is present
            updateChannelTypeResponse.Grants.First(g => g.Key == "channel_member").Value.Should().HaveCount(1);

            // Restore grants
            var restoreGrantsRequest
                = new ChannelTypeWithStringCommandsRequest(grants: new Dictionary<string, List<string>>());
            var restoreGrantsResponse
                = await _channelTypeClient.UpdateChannelTypeAsync(tempChannelType.Name, restoreGrantsRequest);

            // Assert more than 1 grant is present
            restoreGrantsResponse.Grants.First(g => g.Key == "channel_member").Value.Should().HaveCountGreaterThan(1);
        }

        [Test]
        public async Task WhenAssigningAppScopedPermissionsExpectAppGrantsMatchingAsync()
        {
            var settings = new AppSettingsRequest
            {
                Grants = new Dictionary<string, List<string>>
                {
                    { "anonymous", new List<string>() },
                    { "guest", new List<string>() },
                    { "user", new List<string> { "search-user", "mute-user" } },
                    { "admin", new List<string> { "search-user", "mute-user", "ban-user" } },
                },
            };
            await _appClient.UpdateAppSettingsAsync(settings);

            var getAppResponse = await _appClient.GetAppSettingsAsync();
            getAppResponse.App.Grants.Should().NotBeNull();
            getAppResponse.App.Grants["anonymous"].Should().BeEmpty();
            getAppResponse.App.Grants["guest"].Should().BeEmpty();
            getAppResponse.App.Grants["user"].Should().BeEquivalentTo(new[] { "search-user", "mute-user" });
            getAppResponse.App.Grants["admin"].Should()
                .BeEquivalentTo(new[] { "search-user", "mute-user", "ban-user" });
        }

        [Test]
        public async Task WhenUpdatingChannelConfigGrantsOverridesExpectGrantsOverridenAsync()
        {
            var channel = await CreateChannelAsync(createdByUserId: _user1.Id);
            await _channelClient.AddMembersAsync(channel.Type, channel.Id, new[] { _user2.Id });

            var request = new PartialUpdateChannelRequest
            {
                Set = new Dictionary<string, object>
                {
                    {
                        "config_overrides", new Dictionary<string, object>
                        {
                            {
                                "grants", new Dictionary<string, object>
                                {
                                    {
                                        "user", new List<string> { "!add-links", "create-reaction" }
                                    },
                                }
                            },
                        }
                    },
                },
            };
            var partialUpdateChannelResponse
                = await _channelClient.PartialUpdateAsync(channel.Type, channel.Id, request);

            var channelResp = await _channelClient.GetOrCreateAsync(channel.Type, channel.Id, new ChannelGetRequest());
            channelResp.Channel.Config.Grants["user"].Should()
                .BeEquivalentTo(new List<string> { "!add-links", "create-reaction" });
        }
    }
}
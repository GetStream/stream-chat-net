using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="ChannelClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class ChannelClientTests : TestBase
    {
        private UserRequest _user1;
        private UserRequest _user2;
        private UserRequest _user3;
        private ChannelWithConfig _channel;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            (_user1, _user2, _user3) = (await UpsertNewUserAsync(), await UpsertNewUserAsync(),
                await UpsertNewUserAsync());
            _channel = await CreateChannelAsync(createdByUserId: _user1.Id,
                members: new[] { _user1.Id, _user2.Id, _user3.Id });
            await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user1.Id, "text");
        }

        [OneTimeTearDown]
        public async Task TeardownAsync()
        {
            await TryDeleteUsersAsync(_user1.Id, _user2.Id, _user3.Id);
        }

        [Test]
        public async Task TestChannelCreateWithoutIdAsync()
        {
            var chanData = new ChannelRequest();
            chanData.SetData("name", "one big party");
            chanData.SetData("food", new[] { "pizza", "gabagool" });
            chanData.CreatedBy = new UserRequest { Id = _user1.Id };
            chanData.Members = new List<ChannelMember>
            {
                new ChannelMember { UserId = _user1.Id },
                new ChannelMember { UserId = _user2.Id },
            };

            var chanState = await _channelClient.GetOrCreateAsync("messaging", new ChannelGetRequest
            {
                Data = chanData,
            });

            chanState.Channel.Id.Should().NotBeEmpty();
            chanState.Channel.MemberCount.Should().BeGreaterThan(0);
            chanState.Members.Count.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task TestChannelQueryAsync()
        {
            var currentChannelState = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id,
                new ChannelGetRequest
                {
                    State = true,
                    Watch = false,
                    MembersPagination = new PaginationParams
                    {
                        Limit = 1,
                        Offset = 1,
                    },
                });

            currentChannelState.Members.Count.Should().Be(1);
        }

        [Test]
        public async Task TestQueryMembersAsync()
        {
            var res = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = _channel.Type,
                Id = _channel.Id,
                UserId = _user1.Id,
                FilterConditions = new Dictionary<string, object>(),
                Limit = 1,
            });

            res.Members.Should().HaveCount(1);
        }

        [Test]
        public async Task TestQueryMembersSortingAsync()
        {
            // Setup channel with Members
            var userIds = Enumerable.Range(0, 10).Select(i => Guid.NewGuid().ToString()).ToArray();

            userIds.Should().OnlyHaveUniqueItems();

            var userRequests = userIds.Select(id => new UserRequest { Id = id }).ToArray();
            var upsertResponse = await _userClient.UpsertManyAsync(userRequests);

            upsertResponse.Users.Should().HaveCount(10);

            var channel = await CreateChannelAsync(_user1.Id);
            await _channelClient.AddMembersAsync(channel.Type, channel.Id, userIds);

            // Set custom members data
            for (int i = 0; i < userIds.Length; i++)
            {
                if (i % 2 == 0)
                {
                    continue;
                }

                var request = new ChannelMemberPartialRequest()
                {
                    UserId = userIds[i],
                    Set = new Dictionary<string, object>()
                    {
                        { "color", "blue" },
                    },
                };

                await _channelClient.UpdateMemberPartialAsync(channel.Type, channel.Id, request);
            }

            // Test member queries without filter and sort
            var response = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = channel.Type,
                Id = channel.Id,
                FilterConditions = new Dictionary<string, object>(),
            });

            response.Members.Should().HaveCount(10);

            // Test member queries sort
            var response2 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = channel.Type,
                Id = channel.Id,
                FilterConditions = new Dictionary<string, object>(),
                Limit = 20,
                Sorts = new[]
                {
                    new SortParameter
                    {
                        Field = "created_at",
                        Direction = SortDirection.Ascending,
                    },
                },
            });

            response2.Members.Should().HaveCount(10);
            response2.Members.Select(member => member.CreatedAt).Should().BeInAscendingOrder();

            // Test member queries with offset
            var response3 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = channel.Type,
                Id = channel.Id,
                FilterConditions = new Dictionary<string, object>(),
                Limit = 20,
                Offset = 5,
            });

            response3.Members.Should().HaveCount(5);

            // Test member queries filter and sort
            var response4 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = channel.Type,
                Id = channel.Id,
                FilterConditions = new Dictionary<string, object>
                {
                    { "color", "blue" }, // Filters by member custom data (not user custom data)
                },
                Limit = 20,
                Sorts = new[]
                {
                    new SortParameter
                    {
                        Field = "created_at",
                        Direction = SortDirection.Descending,
                    },
                },
            });

            response4.Members.Should().HaveCount(5);
            response4.Members.Select(member => member.CreatedAt).Should().BeInDescendingOrder();
        }

        [Test]
        public async Task TestMembersQueryAsync()
        {
            // Setup channel with Members
            var userIds = Enumerable.Range(0, 10).Select(i => Guid.NewGuid().ToString()).ToArray();
            var userIdsToInvite = Enumerable.Range(0, 2).Select(i => Guid.NewGuid().ToString()).ToArray();
            var usersToBan = userIds.Take(2).ToArray();
            var usersModerators = userIds.Skip(2).Take(2).ToArray();
            var usersWithMemberCustomData = userIds.Skip(4).Take(2).ToArray();
            var usersWithUserCustomData = userIds.Skip(6).Take(2).ToArray();

            userIds.Should().OnlyHaveUniqueItems();
            userIdsToInvite.Should().OnlyHaveUniqueItems();

            var allUserIds = userIds.Concat(userIdsToInvite).ToArray();
            var userRequests = allUserIds.Select(id => new UserRequest { Id = id }).ToArray();
            userRequests[6].Name = "Tommaso";
            userRequests[6].SetData("email", "example@getstream.io");

            var upsertResponse = await _userClient.UpsertManyAsync(userRequests);

            upsertResponse.Users.Should().HaveCount(12);

            var channel = await CreateChannelAsync(_user1.Id);
            await _channelClient.AddMembersAsync(channel.Type, channel.Id, userIds);

            // Invite members
            await _channelClient.InviteAsync(channel.Type, channel.Id, userIdsToInvite);

            // Ban members
            foreach (var userToBanId in usersToBan)
            {
                await _userClient.BanAsync(new BanRequest
                {
                    Type = channel.Type,
                    Id = channel.Id,
                    TargetUserId = userToBanId,
                    UserId = allUserIds[10],
                    Reason = "spam",
                });
            }

            // Set moderators
            await _channelClient.AddModeratorsAsync(channel.Type, channel.Id, usersModerators);

            // Set member custom data
            foreach (var member in usersWithMemberCustomData)
            {
                await _channelClient.UpdateMemberPartialAsync(channel.Type, channel.Id, new ChannelMemberPartialRequest
                {
                    UserId = member,
                    Set = new Dictionary<string, object>
                    {
                        { "subscription", "gold_plan" },
                    },
                });
            }

            // Set user custom data
            foreach (var member in usersWithUserCustomData)
            {
                await _userClient.UpdatePartialAsync(new UserPartialRequest
                {
                    Id = member,
                    Set = new Dictionary<string, object>
                    {
                        { "color", "red" },
                    },
                });
            }

            // Tests start here

            // Get members with pending invites
            var response = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = channel.Type,
                Id = channel.Id,
                FilterConditions = new Dictionary<string, object>
                {
                    { "invite", "pending" },
                },
            });

            response.Members.Should().HaveCount(2);
            response.Members.Should().OnlyHaveUniqueItems();
            foreach (var invitedUserId in userIdsToInvite)
            {
                response.Members.Should().Contain(m => m.UserId == invitedUserId);
            }

            // Search by name autocomplete
            var response2 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = channel.Type,
                Id = channel.Id,
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "name", new Dictionary<string, string>
                        {
                            { "$autocomplete", "Tomm" },
                        }
                    },
                },
            });

            response2.Members.Should().HaveCount(1);
            response2.Members.Single().User.Name.Should().StartWith("Tomm");

            // Get moderators
            var response3 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = channel.Type,
                Id = channel.Id,
                FilterConditions = new Dictionary<string, object>
                {
                    { "channel_role", "channel_moderator" },
                },
            });

            response3.Members.Should().HaveCount(2);
            response3.Members.Should().OnlyHaveUniqueItems();
            foreach (var invitedUserId in usersModerators)
            {
                response3.Members.Should().Contain(m => m.UserId == invitedUserId);
            }

            // Get members who have joined the channel
            var response4 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = channel.Type,
                Id = channel.Id,
                FilterConditions = new Dictionary<string, object>
                {
                    { "joined", true },
                },
            });

            response4.Members.Should().HaveCount(10);
            response4.Members.Should().OnlyHaveUniqueItems();
            foreach (var bannedUserId in userIds)
            {
                response4.Members.Should().Contain(m => m.UserId == bannedUserId);
            }

            // Get banned members
            var response5 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = channel.Type,
                Id = channel.Id,
                FilterConditions = new Dictionary<string, object>
                {
                    { "banned", true },
                },
            });

            response5.Members.Should().HaveCount(2);
            response5.Members.Should().OnlyHaveUniqueItems();
            foreach (var bannedUserId in usersToBan)
            {
                response5.Members.Should().Contain(m => m.UserId == bannedUserId);
            }

            // Get members by custom data
            var response6 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = channel.Type,
                Id = channel.Id,
                FilterConditions = new Dictionary<string, object>
                {
                    { "subscription", "gold_plan" },
                },
            });

            response6.Members.Should().HaveCount(2);
            response6.Members.Should().OnlyHaveUniqueItems();
            foreach (var userId in usersWithMemberCustomData)
            {
                response6.Members.Should().Contain(m => m.UserId == userId);
            }

            // Get members by user custom data
            // Note that members custom data is separate from user custom data
            var response7 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = channel.Type,
                Id = channel.Id,
                FilterConditions = new Dictionary<string, object>
                {
                    { "user.email", "example@getstream.io" },
                },
            });
            response7.Members.Should().HaveCount(1);
        }

        [Test]
        public async Task TestChannelUpdateAsync()
        {
            var expectedChannel = new ChannelUpdateRequest { Data = new ChannelRequest() };
            expectedChannel.Data.SetData("plain", Guid.NewGuid().ToString());
            expectedChannel.Data.SetData("complex", new[] { Guid.NewGuid().ToString() });

            var actualChannel = await _channelClient.UpdateAsync(_channel.Type, _channel.Id, expectedChannel);

            actualChannel.Channel.GetData<string>("plain").Should()
                .BeEquivalentTo(expectedChannel.Data.GetData<string>("plain"));
            actualChannel.Channel.GetData<string[]>("complex").Should()
                .BeEquivalentTo(expectedChannel.Data.GetData<string[]>("complex"));
        }

        [Test]
        public async Task TestChannelPartialUpdateAsync()
        {
            var channelUpdates = new PartialUpdateChannelRequest
            {
                Set = new Dictionary<string, object> { { "color", "blue" }, { "age", 27 } },
            };

            var actualChannel = await _channelClient.PartialUpdateAsync(_channel.Type, _channel.Id, channelUpdates);

            actualChannel.Channel.GetData<string>("color").Should().BeEquivalentTo((string)channelUpdates.Set["color"]);
            actualChannel.Channel.GetData<int>("age").Should().Be((int)channelUpdates.Set["age"]);
        }

        [Test]
        public async Task TestDeleteChannelAsync()
        {
            // Let's not delete the channel that the other tests are using, so let's create a new one
            var channel = await CreateChannelAsync(createdByUserId: _user1.Id,
                members: new[] { _user1.Id, _user2.Id, _user3.Id }, autoDelete: false);

            await WaitForAsync(async () =>
            {
                try
                {
                    await _channelClient.DeleteAsync(channel.Type, channel.Id);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });

            var resp = await _channelClient.QueryChannelsAsync(QueryChannelsOptions.Default.WithFilter(
                new Dictionary<string, object>
                {
                    { "cid", channel.Cid },
                }));
            resp.Channels.Count.Should().Be(0);
        }

        [Test]
        public async Task TestChannelTruncateAsync()
        {
            await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user1.Id, "text");
            var originalChannel
                = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id,
                    new ChannelGetRequest { State = true });
            originalChannel.Messages.Should().NotBeEmpty();

            await _channelClient.TruncateAsync(_channel.Type, _channel.Id, new TruncateOptions { UserId = _user2.Id });

            var afterTruncateChannel
                = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id,
                    new ChannelGetRequest { State = true });
            afterTruncateChannel.Messages.Should().BeEmpty();

            // this isn't deployed to the test environment, so uncomment it a bit later:
            // afterTruncateChannel.Channel.TruncatedBy.Id.Should().Be(_user2.Id);
        }

        [Test]
        public async Task TestQueryChannelsAsync()
        {
            var queryResp = await _channelClient.QueryChannelsAsync(QueryChannelsOptions.Default.WithFilter(
                new Dictionary<string, object>
                {
                    { "cid", _channel.Cid },
                }));

            var read = queryResp.Channels[0].Reads[0];
            read.LastRead.Should().NotBe(default(DateTimeOffset));
            read.UnreadMessages.Should().NotBeNull();
            read.User.Should().NotBeNull();
        }

        [Test]
        public async Task TestHideAndShowChannelAsync()
        {
            Func<Task> hideCall = () => _channelClient.HideAsync(_channel.Type, _channel.Id, _user1.Id);
            Func<Task> showCall = () => _channelClient.ShowAsync(_channel.Type, _channel.Id, _user1.Id);

            await hideCall.Should().NotThrowAsync();
            await showCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestMuteUnmuteChannelAsync()
        {
            Func<Task> muteCall = () => _channelClient.MuteChannelAsync(new ChannelMuteRequest
            {
                ChannelCids = new[] { _channel.Cid },
                UserId = _user1.Id,
            });
            Func<Task> unmuteCall = () => _channelClient.UnmuteChannelAsync(new ChannelUnmuteRequest
            {
                ChannelCids = new[] { _channel.Cid },
                UserId = _user1.Id,
            });

            await muteCall.Should().NotThrowAsync();
            await unmuteCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestMuteUnmuteChannelMemberNotificationsMutedAsync()
        {
            async Task<bool?> GetUserNotificationsMuted()
            {
                var queryOptions = new QueryChannelsOptions().WithFilter(new Dictionary<string, object>
                {
                    { "cid", new Dictionary<string, object> { { "$in", new[] { _channel.Cid } } } },
                });

                var channels = await _channelClient.QueryChannelsAsync(queryOptions);

                var channelMember = channels.Channels.First().Members.First(u => u.UserId == _user1.Id);
                return channelMember.NotificationsMuted;
            }

            (await GetUserNotificationsMuted()).Should().BeFalse();

            await _channelClient.MuteChannelAsync(new ChannelMuteRequest
            {
                ChannelCids = new[] { _channel.Cid },
                UserId = _user1.Id,
            });

            (await GetUserNotificationsMuted()).Should().BeTrue();

            await _channelClient.UnmuteChannelAsync(new ChannelUnmuteRequest
            {
                ChannelCids = new[] { _channel.Cid },
                UserId = _user1.Id,
            });

            (await GetUserNotificationsMuted()).Should().BeFalse();
        }

        [Test]
        public async Task TestGetExportChannelAsync()
        {
            var resp = await _channelClient.ExportChannelAsync(new ExportChannelItem
                { Id = _channel.Id, Type = _channel.Type });

            await WaitForAsync(async () =>
            {
                var status = await _taskClient.GetTaskStatusAsync(resp.TaskId);

                return status.Status == AsyncTaskStatus.Completed;
            }, timeout: 10000);
        }

        [Test]
        public async Task TestAddMembersAsync()
        {
            var newUser = await UpsertNewUserAsync();

            await _channelClient.AddMembersAsync(_channel.Type, _channel.Id, newUser.Id);

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id,
                ChannelGetRequest.WithoutWatching());
            channel.Members.Should().Contain(x => x.UserId == newUser.Id);
            await TryDeleteUsersAsync(new[] { newUser.Id });
        }

        [Test]
        public async Task TestRemoveMembersAsync()
        {
            await _channelClient.RemoveMembersAsync(_channel.Type, _channel.Id, new[] { _user3.Id });

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id,
                ChannelGetRequest.WithoutWatching());
            channel.Members.Should().NotContain(x => x.UserId == _user3.Id);
        }

        [Test]
        public async Task TestAddModeratorsAsync()
        {
            await _channelClient.AddModeratorsAsync(_channel.Type, _channel.Id, new[] { _user2.Id });

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id,
                ChannelGetRequest.WithoutWatching());
            var user2 = channel.Members.First(x => x.UserId == _user2.Id);
            user2.IsModerator.Should().BeTrue();
        }

        [Test]
        public async Task TestDemoteModeratorsAsync()
        {
            await _channelClient.AddModeratorsAsync(_channel.Type, _channel.Id, new[] { _user2.Id });

            await _channelClient.DemoteModeratorsAsync(_channel.Type, _channel.Id, new[] { _user2.Id });

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id,
                ChannelGetRequest.WithoutWatching());
            var user2 = channel.Members.First(x => x.UserId == _user2.Id);
            user2.IsModerator.Should().BeNull();
        }

        [Test]
        public async Task TestAssignRoleAsync()
        {
            await _channelClient.AssignRolesAsync(_channel.Type, _channel.Id, new AssignRoleRequest
            {
                AssignRoles = new List<RoleAssignment>
                {
                    new RoleAssignment
                    {
                        UserId = _user2.Id,
                        ChannelRole = Role.ChannelModerator,
                    },
                },
            });

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id,
                ChannelGetRequest.WithoutWatching());
            var user2 = channel.Members.First(x => x.UserId == _user2.Id);
            user2.IsModerator.Should().BeTrue();
        }

        [Test]
        public async Task TestInvitationAndAcceptanceAsync()
        {
            var userToInvite = await UpsertNewUserAsync();
            Func<Task> inviteTask = () => _channelClient.InviteAsync(_channel.Type, _channel.Id, userToInvite.Id);
            Func<Task> acceptTask = () => _channelClient.AcceptInviteAsync(_channel.Type, _channel.Id, userToInvite.Id);

            await inviteTask.Should().NotThrowAsync();
            await acceptTask.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestInvitationAndRejectionAsync()
        {
            var userToInvite = await UpsertNewUserAsync();
            Func<Task> inviteTask = () => _channelClient.InviteAsync(_channel.Type, _channel.Id, userToInvite.Id);
            Func<Task> rejectTask = () => _channelClient.RejectInviteAsync(_channel.Type, _channel.Id, userToInvite.Id);

            await inviteTask.Should().NotThrowAsync();
            await rejectTask.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestChannelMemberPartialUpdateAsync()
        {
            await _channelClient.AddMembersAsync(_channel.Type, _channel.Id, _user1.Id);

            var partialRequest = new ChannelMemberPartialRequest
            {
                UserId = _user1.Id,
                Set = new Dictionary<string, object>()
                {
                    { "hat", "blue" },
                },
            };
            await _channelClient.UpdateMemberPartialAsync(_channel.Type, _channel.Id, partialRequest);

            var result = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = _channel.Type,
                Id = _channel.Id,
                FilterConditions = new Dictionary<string, object>()
                {
                    { "id", _user1.Id },
                },
            });

            result.Members.First().GetData<string>("hat").Should().Be("blue");
        }

        [Test]
        public async Task TestPinChannelForMemberAsync()
        {
            var channel = await CreateChannelAsync(createdByUserId: _user1.Id);

            await _channelClient.AddMembersAsync(channel.Type, channel.Id, _user1.Id);

            var timestamp = DateTimeOffset.UtcNow;

            // Pin
            var response = await _channelClient.PinAsync(channel.Type, channel.Id, _user1.Id);

            // Assert pinned_at
            response.ChannelMember.UserId.Should().Be(_user1.Id);
            response.ChannelMember.PinnedAt.Should().BeCloseTo(timestamp, TimeSpan.FromSeconds(1));

            // Assert query pinned channel
            var pinnedChannels = await _channelClient.QueryChannelsAsync(new QueryChannelsOptions
            {
                Filter = new Dictionary<string, object>()
                {
                    { "pinned", true },
                    { "cid", channel.Cid },
                },
                UserId = _user1.Id,
            });

            pinnedChannels.Channels.Single().Channel.Cid.Should().Be(channel.Cid);
        }

        [Test]
        public async Task TestUnpinChannelForMemberAsync()
        {
            var channel = await CreateChannelAsync(createdByUserId: _user1.Id);

            await _channelClient.AddMembersAsync(channel.Type, channel.Id, _user1.Id);

            var timestamp = DateTimeOffset.UtcNow;

            // Pin
            var pinResponse = await _channelClient.PinAsync(channel.Type, channel.Id, _user1.Id);

            // Assert pinned_at
            pinResponse.ChannelMember.UserId.Should().Be(_user1.Id);
            pinResponse.ChannelMember.PinnedAt.Should().BeCloseTo(timestamp, TimeSpan.FromSeconds(1));

            // Unpin
            var unpinResponse = await _channelClient.UnpinAsync(channel.Type, channel.Id, _user1.Id);

            // Assert unpinned
            unpinResponse.ChannelMember.UserId.Should().Be(_user1.Id);
            unpinResponse.ChannelMember.PinnedAt.Should().BeNull();

            // Assert query pinned channel
            var pinnedChannels = await _channelClient.QueryChannelsAsync(new QueryChannelsOptions
            {
                Filter = new Dictionary<string, object>()
                {
                    { "pinned", true },
                    { "cid", channel.Cid },
                },
                UserId = _user1.Id,
            });

            pinnedChannels.Channels.Should().NotContain(c => c.Channel.Cid == channel.Cid);
        }

        [Test]
        public async Task TestArchiveChannelForMemberAsync()
        {
            var channel = await CreateChannelAsync(createdByUserId: _user1.Id);

            await _channelClient.AddMembersAsync(channel.Type, channel.Id, _user1.Id);

            var timestamp = DateTimeOffset.UtcNow;

            // Archive
            var response = await _channelClient.ArchiveAsync(channel.Type, channel.Id, _user1.Id);

            // Assert archived_at
            response.ChannelMember.UserId.Should().Be(_user1.Id);
            response.ChannelMember.ArchivedAt.Should().BeCloseTo(timestamp, TimeSpan.FromSeconds(1));

            // Assert query archived channel
            var archivedChannels = await _channelClient.QueryChannelsAsync(new QueryChannelsOptions
            {
                Filter = new Dictionary<string, object>()
                {
                    { "archived", true },
                    { "cid", channel.Cid },
                },
                UserId = _user1.Id,
            });

            archivedChannels.Channels.Single().Channel.Cid.Should().Be(channel.Cid);
        }

        [Test]
        public async Task TestUnarchiveChannelForMemberAsync()
        {
            var channel = await CreateChannelAsync(createdByUserId: _user1.Id);

            await _channelClient.AddMembersAsync(channel.Type, channel.Id, _user1.Id);

            var timestamp = DateTimeOffset.UtcNow;

            // Archive
            var archiveResponse = await _channelClient.ArchiveAsync(channel.Type, channel.Id, _user1.Id);

            // Assert archived_at
            archiveResponse.ChannelMember.UserId.Should().Be(_user1.Id);
            archiveResponse.ChannelMember.ArchivedAt.Should().BeCloseTo(timestamp, TimeSpan.FromSeconds(1));

            // Unarchive
            var unarchiveResponse = await _channelClient.UnarchiveAsync(channel.Type, channel.Id, _user1.Id);

            // Assert unarchived
            unarchiveResponse.ChannelMember.UserId.Should().Be(_user1.Id);
            unarchiveResponse.ChannelMember.ArchivedAt.Should().BeNull();

            // Assert query archived channel
            var archivedChannels = await _channelClient.QueryChannelsAsync(new QueryChannelsOptions
            {
                Filter = new Dictionary<string, object>()
                {
                    { "archived", true },
                    { "cid", channel.Cid },
                },
                UserId = _user1.Id,
            });

            archivedChannels.Channels.Should().NotContain(c => c.Channel.Cid == channel.Cid);
        }
    }
}
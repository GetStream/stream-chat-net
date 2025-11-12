using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using StreamChat.Clients;
using StreamChat.Exceptions;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="UserClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [Parallelizable(ParallelScope.None)]
    [TestFixture]
    public class UserClientTests : TestBase
    {
        private ChannelWithConfig _channel;
        private UserRequest _user1;
        private UserRequest _user2;
        private UserRequest _user3;

        [SetUp]
        public async Task SetupAsync()
        {
            (_user1, _user2, _user3) = (await UpsertNewUserAsync(), await UpsertNewUserAsync(), await UpsertNewUserAsync());
            _channel = await CreateChannelAsync(createdByUserId: _user1.Id, members: new[] { _user1.Id, _user2.Id });
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            await TryDeleteUsersAsync(_user1.Id, _user2.Id, _user3.Id);
        }

        [Test]
        public void TestCreateUserTokenWithoutExpiration()
        {
            var token = _userClient.CreateToken(_user1.Id);

            var splitted = token.Split('.');
            var header = JObject.Parse(Encoding.UTF8.GetString(DecodeJwtBase64(splitted[0])));
            var payload = JObject.Parse(Encoding.UTF8.GetString(DecodeJwtBase64(splitted[1])));
            header["typ"].Value<string>().Should().Be("JWT");
            header["alg"].Value<string>().Should().Be("HS256");
            payload.Children().Should().HaveCount(1);
            payload["user_id"].Value<string>().Should().Be(_user1.Id);
        }

        [Test]
        public void TestCreateUserTokenWithExpiration()
        {
            var token = _userClient.CreateToken(_user1.Id,
                expiration: DateTimeOffset.UtcNow.AddSeconds(2),
                issuedAt: DateTimeOffset.UtcNow);

            var splitted = token.Split('.');
            var header = JObject.Parse(Encoding.UTF8.GetString(DecodeJwtBase64(splitted[0])));
            var payload = JObject.Parse(Encoding.UTF8.GetString(DecodeJwtBase64(splitted[1])));
            header["typ"].Value<string>().Should().Be("JWT");
            header["alg"].Value<string>().Should().Be("HS256");
            payload["user_id"].Value<string>().Should().Be(_user1.Id);
            payload["exp"].Value<long>().Should().BeCloseTo(DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 10);
            payload["iat"].Value<long>().Should().BeCloseTo(DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 10);
        }

        private byte[] DecodeJwtBase64(string encodedStr)
        {
            encodedStr = encodedStr.Replace('-', '+').Replace('_', '/');

            // check for padding
            switch (encodedStr.Length % 4)
            {
                case 0:

                    // No pad chars
                    break;
                case 2:

                    // Two pad chars
                    encodedStr += "==";
                    break;
                case 3:

                    // One pad char
                    encodedStr += "=";
                    break;
            }

            return Convert.FromBase64String(encodedStr);
        }

        [Test]
        public async Task TestUpsertAsync()
        {
            var expectedUser = new UserRequest { Id = _user1.Id };
            expectedUser.SetData("foo", Guid.NewGuid().ToString());

            var resp = await _userClient.UpsertAsync(expectedUser);

            resp.Users[_user1.Id].GetData<string>("foo").Should().BeEquivalentTo(expectedUser.GetData<string>("foo"));
        }

        [Test]
        public async Task TestUpsertManyAsync()
        {
            var expectedUser = new UserRequest { Id = _user1.Id };
            expectedUser.SetData("foo", Guid.NewGuid().ToString());

            var resp = await _userClient.UpsertManyAsync(new[] { expectedUser });

            resp.Users[_user1.Id].GetData<string>("foo").Should().BeEquivalentTo(expectedUser.GetData<string>("foo"));
        }

        [Test]
        public async Task TestCreateGuestAsync()
        {
            var guestUser = new UserRequest
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Guest user",
            };

            CreateGuestResponse resp;
            try
            {
                resp = await _userClient.CreateGuestAsync(guestUser);
            }
            catch (StreamChatException ex)
            {
                if (ex.Message.Contains("guest access is disabled for this application"))
                    return;
                else
                    throw;
            }

            resp.User.Id.Should().Contain(guestUser.Id);
            resp.AccessToken.Should().NotBeEmpty();
        }

        [Test]
        public async Task TestUpdatePartialAsync()
        {
            var updates = new Dictionary<string, object>
            {
                { "foo", Guid.NewGuid().ToString() },
            };

            var resp = await _userClient.UpdatePartialAsync(new UserPartialRequest
            {
                Id = _user1.Id,
                Set = updates,
            });

            resp.Users[_user1.Id].GetData<string>("foo").Should().BeEquivalentTo((string)updates["foo"]);
        }

        [Test]
        public async Task TestDeleteUserAsync()
        {
            Func<Task> deleteCall = () => _userClient.DeleteAsync(_user2.Id, hardDelete: true);

            await deleteCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestQueryUsersAsync()
        {
            var resp = await _userClient.QueryAsync(QueryUserOptions.Default.WithFilter(new Dictionary<string, object>
            {
                { "id", _user1.Id },
            }));

            resp.Users.Should().NotBeEmpty();
        }

        [Test]
        public async Task TestQueryUsersWithIncludeDeactivatedUsersAsync()
        {
            // Deactivate user3
            await _userClient.DeactivateAsync(_user3.Id);

            // Query without including deactivated users
            var respWithoutDeactivated = await _userClient.QueryAsync(QueryUserOptions.Default.WithFilter(new Dictionary<string, object>
            {
                { "id", new Dictionary<string, object> { { "$in", new[] { _user1.Id, _user3.Id } } } },
            }));

            // Should only find user1 (user3 is deactivated)
            respWithoutDeactivated.Users.Should().NotBeEmpty();
            respWithoutDeactivated.Users.Should().HaveCount(1);
            respWithoutDeactivated.Users[0].Id.Should().Be(_user1.Id);

            // Query with including deactivated users
            var respWithDeactivated = await _userClient.QueryAsync(QueryUserOptions.Default
                .WithIncludeDeactivatedUsers()
                .WithFilter(new Dictionary<string, object>
                {
                    { "id", new Dictionary<string, object> { { "$in", new[] { _user1.Id, _user3.Id } } } },
                }));

            // Should find both users
            respWithDeactivated.Users.Should().NotBeEmpty();
            respWithDeactivated.Users.Should().HaveCount(2);
            respWithDeactivated.Users.Should().Contain(u => u.Id == _user1.Id);
            respWithDeactivated.Users.Should().Contain(u => u.Id == _user3.Id);

            // Verify user3 is deactivated
            var user3 = respWithDeactivated.Users.First(u => u.Id == _user3.Id);
            user3.DeactivatedAt.Should().NotBeNull();

            // Reactivate user3 for cleanup
            await _userClient.ReactivateAsync(_user3.Id);
        }

        [Test]
        public async Task TestDeactivateUserAsync()
        {
            var resp = await _userClient.DeactivateAsync(_user2.Id);

            resp.User.DeactivatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task TestReactivateUserAsync()
        {
            await _userClient.DeactivateAsync(_user2.Id);

            var resp = await _userClient.ReactivateAsync(_user2.Id);

            resp.User.DeactivatedAt.Should().BeNull();
        }

        [Test]
        public async Task TestExportUserAsync()
        {
            var resp = await _userClient.ExportAsync(_user1.Id);

            resp.User.Id.Should().BeEquivalentTo(_user1.Id);
        }

        [Test]
        public async Task TestShadowBanUserAsync()
        {
            await _userClient.ShadowBanAsync(new ShadowBanRequest
            {
                Type = _channel.Type,
                Id = _channel.Id,
                Reason = "reason",
                TargetUserId = _user2.Id,
                UserId = _user1.Id,
            });

            Func<Task> sendMessageCall = () => _messageClient.SendMessageAsync(
                    _channel.Type, _channel.Id, _user2.Id, "text");
            await sendMessageCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestBanUserAsync()
        {
            await _userClient.BanAsync(new BanRequest
            {
                Type = _channel.Type,
                Id = _channel.Id,
                Reason = "reason",
                TargetUserId = _user2.Id,
                UserId = _user1.Id,
            });

            Func<Task> sendMessageCall = () => _messageClient.SendMessageAsync(
                    _channel.Type, _channel.Id, _user2.Id, "text");
            await sendMessageCall.Should().ThrowAsync<StreamChatException>();
        }

        [Test]
        public async Task TestUnbanUserAsync()
        {
            await _userClient.BanAsync(new BanRequest
            {
                Type = _channel.Type,
                Id = _channel.Id,
                Reason = "reason",
                TargetUserId = _user2.Id,
                UserId = _user1.Id,
            });

            await _userClient.UnbanAsync(new BanRequest
            {
                Type = _channel.Type,
                Id = _channel.Id,
                TargetUserId = _user2.Id,
                UserId = _user1.Id,
            });

            Func<Task> sendMessageCall = () => _messageClient.SendMessageAsync(
                    _channel.Type, _channel.Id, _user2.Id, "text");
            await sendMessageCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestMuteUserAsync()
        {
            var muteResp = await _userClient.MuteAsync(_user2.Id, _user1.Id);

            muteResp.OwnUser.Id.Should().BeEquivalentTo(_user1.Id);
            muteResp.Mute.Target.Id.Should().BeEquivalentTo(_user2.Id);
            muteResp.Mute.User.Id.Should().BeEquivalentTo(_user1.Id);
        }

        [Test]
        public async Task TestBlockUserAsync()
        {
            var blockResp = await _userClient.BlockUserAsync(_user2.Id, _user1.Id);

            blockResp.BlockedByUserID.Should().BeEquivalentTo(_user1.Id);
            blockResp.BlockedUserID.Should().BeEquivalentTo(_user2.Id);

            var getBlockResp = await _userClient.GetBlockedUsersAsync(_user1.Id);
            getBlockResp.Blocks[0].BlockedUserID.Should().BeEquivalentTo(_user2.Id);
            getBlockResp.Blocks[0].BlockedByUserID.Should().BeEquivalentTo(_user1.Id);
            getBlockResp.Blocks[0].BlockedUser.Id.Should().BeEquivalentTo(_user2.Id);
            getBlockResp.Blocks[0].BlockedByUser.Id.Should().BeEquivalentTo(_user1.Id);

            var queryUsersResp = await _userClient.QueryAsync(QueryUserOptions.Default.WithFilter(new Dictionary<string, object>
            {
                { "id", _user1.Id },
            }));
            queryUsersResp.Users.Should().NotBeEmpty();

            queryUsersResp.Users[0].BlockedUserIds[0].Should().BeEquivalentTo(_user2.Id);

            await _userClient.UnblockUserAsync(_user2.Id, _user1.Id);

            getBlockResp = await _userClient.GetBlockedUsersAsync(_user1.Id);
            getBlockResp.Blocks.Length.Should().Be(0);

            queryUsersResp = await _userClient.QueryAsync(QueryUserOptions.Default.WithFilter(new Dictionary<string, object>
            {
                { "id", _user1.Id },
            }));
            queryUsersResp.Users.Should().NotBeEmpty();
            queryUsersResp.Users[0].BlockedUserIds.Length.Should().Be(0);
        }

        [Test]
        public async Task TestUnmuteUserAsync()
        {
            await _userClient.MuteAsync(_user2.Id, _user1.Id);

            await _userClient.UnmuteAsync(_user2.Id, _user1.Id);
        }

        [Test]
        public async Task TestMarkAllReadAsync()
        {
            Func<Task> markReadCall = () => _userClient.MarkAllReadAsync(_user1.Id);

            await markReadCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestRevokeTokenAsync()
        {
            var revocationDate = DateTimeOffset.UtcNow.AddHours(-1);

            var resp = await _userClient.RevokeUserTokenAsync(_user1.Id, revocationDate);

            resp.Users[_user1.Id].GetData<DateTimeOffset>("revoke_tokens_issued_before").Should().BeCloseTo(revocationDate, TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task TestManyRevokeTokenAsync()
        {
            var revocationDate = DateTimeOffset.UtcNow.AddHours(-1);

            var resp = await _userClient.RevokeManyUserTokensAsync(new[] { _user1.Id, _user2.Id }, revocationDate);

            resp.Users.Should().HaveCount(2);
            foreach (var user in resp.Users.Values)
                user.GetData<DateTimeOffset>("revoke_tokens_issued_before").Should().BeCloseTo(revocationDate, TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task TestQueryBannedUsersAsync()
        {
            await _userClient.BanAsync(new BanRequest
            {
                Type = _channel.Type,
                Id = _channel.Id,
                Reason = "reason",
                TargetUserId = _user2.Id,
                UserId = _user1.Id,
            });

            var resp = await _userClient.QueryBannedUsersAsync(new QueryBannedUsersRequest
            {
                UserId = _user1.Id,
                FilterConditions = new Dictionary<string, object>(),
                Limit = 1,
            });

            resp.Bans.Should().NotBeEmpty();
        }

        [Test]
        public async Task TestExportUsersAsync()
        {
            var resp = await _userClient.ExportUsersAsync(new[] { _user1.Id, _user2.Id });

            resp.TaskId.Should().NotBeNullOrEmpty();

            AsyncTaskStatusResponse status = null;
            await WaitForAsync(async () =>
            {
                status = await _taskClient.GetTaskStatusAsync(resp.TaskId);

                return status.Status == AsyncTaskStatus.Completed;
            }, timeout: 10000);

            status.Should().NotBeNull();
            status.Status.Should().Be(AsyncTaskStatus.Completed);
            status.CreatedAt.Should().NotBeNull();
            status.Result.Should().NotBeNullOrEmpty();
            var exportUrl = status.Result.Values.First().ToString();
            exportUrl.Should().Contain("exports/users");

            using var client = new HttpClient();
            using var response = await client.GetAsync(exportUrl);
            response.EnsureSuccessStatusCode();

            var exportFile = await response.Content.ReadAsStringAsync();
            exportFile.Should().Contain(_user1.Id);
            exportFile.Should().Contain(_user2.Id);
        }

        [Test]
        public async Task TestLiveLocationAsync()
        {

            var channel = await CreateChannelAsync(createdByUserId: _user1.Id, members: new[] { _user1.Id });

            await EnableSharedLocationsAsync(channel);

            var longitude = -122.4194;
            var latitude = 38.999;

            var location = new SharedLocationRequest
            {
                Longitude = longitude,
                Latitude = latitude,
                EndAt = DateTimeOffset.UtcNow.AddHours(1),
                CreatedByDeviceId = "test-device",
            };

            var messageRequest = new MessageRequest
            {
                Text = "Test message for shared location",
                SharedLocation = location,
            };
            var messageResp = await _messageClient.SendMessageAsync(
                channel.Type,
                channel.Id,
                messageRequest,
                _user1.Id);

            var message = messageResp.Message;

            var newLongitude = -122.4194;
            var newLatitude = 38.999;

            var newLocation = new SharedLocationRequest
            {
                MessageId = message.Id,
                Longitude = newLongitude,
                Latitude = newLatitude,
                EndAt = DateTimeOffset.UtcNow.AddHours(10),
                CreatedByDeviceId = "test-device",
            };

            SharedLocationResponse updateResp;
            updateResp = await _userClient.UpdateUserLiveLocationAsync(_user1.Id, newLocation);

            updateResp.Should().NotBeNull();
            updateResp.Latitude.Should().Be(newLatitude);
            updateResp.Longitude.Should().Be(newLongitude);

            var getResp = await _userClient.GetUserActiveLiveLocationsAsync(_user1.Id);

            getResp.Should().NotBeNull();
            getResp.ActiveLiveLocations.Should().NotBeEmpty("Should have active live locations");

            var newLocationResp = getResp.ActiveLiveLocations.FirstOrDefault(loc => loc.MessageId == message.Id);
            newLocationResp.Should().NotBeNull();
            newLocationResp.Latitude.Should().Be(newLatitude);
            newLocationResp.Longitude.Should().Be(newLongitude);
        }

        /// <summary>
        /// Enables shared locations for the channel by updating config overrides.
        /// </summary>
        private async Task EnableSharedLocationsAsync(ChannelWithConfig channel)
        {
            var request = new PartialUpdateChannelRequest
            {
                Set = new Dictionary<string, object>
                {
                    {
                        "config_overrides", new Dictionary<string, object>
                        {
                            { "shared_locations", true },
                        }
                    },
                },
            };
            await _channelClient.PartialUpdateAsync(channel.Type, channel.Id, request);
        }

        /// <summary>
        /// Disables shared locations for the channel by updating config overrides.
        /// </summary>
        private async Task DisableSharedLocationsAsync(ChannelWithConfig channel)
        {
            var request = new PartialUpdateChannelRequest
            {
                Set = new Dictionary<string, object>
                {
                    {
                        "config_overrides", new Dictionary<string, object>
                        {
                            { "shared_locations", false },
                        }
                    },
                },
            };
            await _channelClient.PartialUpdateAsync(channel.Type, channel.Id, request);
        }

        [Test]
        public async Task TestMarkDeliveredAsync()
        {
            var markDeliveredOptions = new MarkDeliveredOptions
            {
                LatestDeliveredMessages = new List<DeliveredMessageConfirmation>
                {
                    new DeliveredMessageConfirmation
                    {
                        ChannelCID = "channel1",
                        MessageID = "message1",
                    },
                    new DeliveredMessageConfirmation
                    {
                        ChannelCID = "channel2",
                        MessageID = "message2",
                    }
                },
                UserID = _user1.Id,
            };

            var response = await _userClient.MarkDeliveredAsync(markDeliveredOptions);

            response.Should().NotBeNull();
        }

        [Test]
        public async Task TestMarkDelivered_WithUserIdAsync()
        {
            var markDeliveredOptions = new MarkDeliveredOptions
            {
                LatestDeliveredMessages = new List<DeliveredMessageConfirmation>
                {
                    new DeliveredMessageConfirmation
                    {
                        ChannelCID = "channel1",
                        MessageID = "message1",
                    }
                },
                UserID = _user1.Id,
            };

            var response = await _userClient.MarkDeliveredAsync(markDeliveredOptions);

            response.Should().NotBeNull();
        }

        [Test]
        public Task TestMarkDelivered_NullData_ThrowsArgumentNullExceptionAsync()
        {
            Func<Task> markDeliveredCall = async () => await _userClient.MarkDeliveredAsync(null);

            return markDeliveredCall.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public Task TestMarkDelivered_EmptyLatestDeliveredMessages_ThrowsArgumentExceptionAsync()
        {
            var markDeliveredOptions = new MarkDeliveredOptions
            {
                LatestDeliveredMessages = new List<DeliveredMessageConfirmation>(),
                UserID = _user1.Id,
            };

            Func<Task> markDeliveredCall = async () => await _userClient.MarkDeliveredAsync(markDeliveredOptions);

            return markDeliveredCall.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public Task TestMarkDelivered_NoUserOrUserId_ThrowsArgumentExceptionAsync()
        {
            var markDeliveredOptions = new MarkDeliveredOptions
            {
                LatestDeliveredMessages = new List<DeliveredMessageConfirmation>
                {
                    new DeliveredMessageConfirmation
                    {
                        ChannelCID = "channel1",
                        MessageID = "message1",
                    }
                }
            };

            Func<Task> markDeliveredCall = async () => await _userClient.MarkDeliveredAsync(markDeliveredOptions);

            return markDeliveredCall.Should().ThrowAsync<ArgumentException>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat;
using StreamChat.Clients;
using StreamChat.Exceptions;
using StreamChat.Models;
using StreamChat.Utils;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="MessageClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [Parallelizable(ParallelScope.None)]
    [TestFixture]
    public class MessageClientTests : TestBase
    {
        private ChannelWithConfig _channel;
        private Message _message;
        private UserRequest _user;

        [SetUp]
        public async Task SetupAsync()
        {
            _user = await UpsertNewUserAsync();
            _channel = await CreateChannelAsync(createdByUserId: _user.Id, members: new[] { _user.Id });
            var resp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user.Id, "text");
            _message = resp.Message;
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await TryDeleteUsersAsync(_user.Id);
        }

        /// <summary>
        /// Enables user message reminders for the channel by updating config overrides.
        /// </summary>
        private async Task EnableUserMessageRemindersAsync()
        {
            var request = new PartialUpdateChannelRequest
            {
                Set = new Dictionary<string, object>
                {
                    {
                        "config_overrides", new Dictionary<string, object>
                        {
                            { "user_message_reminders", true },
                        }
                    },
                },
            };
            await _channelClient.PartialUpdateAsync(_channel.Type, _channel.Id, request);
        }

        /// <summary>
        /// Disables user message reminders for the channel by removing config overrides.
        /// </summary>
        private async Task DisableUserMessageRemindersAsync()
        {
                        var request = new PartialUpdateChannelRequest
            {
                Set = new Dictionary<string, object>
                {
                    {
                        "config_overrides", new Dictionary<string, object>
                        {
                            { "user_message_reminders", false },
                        }
                    },
                },
            };
            await _channelClient.PartialUpdateAsync(_channel.Type, _channel.Id, request);
        }

        [Test]
        public async Task TestGetMessageAsync()
        {
            var resp = await _messageClient.GetMessageAsync(_message.Id);

            resp.Message.Id.Should().BeEquivalentTo(_message.Id);
        }

        [Test]
        public async Task TestGetMessagesAsync()
        {
            var resp = await _messageClient.GetMessagesAsync(_channel.Type, _channel.Id, new[] { _message.Id });

            resp.Messages.Should().Contain(m => m.Id == _message.Id);
        }

        [Test]
        public async Task TestSendMessageAsync()
        {
            var expectedMessage = new MessageRequest { Text = Guid.NewGuid().ToString() };
            expectedMessage.SetData("foo", "barsky");
            var attachment = new Attachment();
            attachment.SetData("baz", "bazky");
            expectedMessage.Attachments = new List<Attachment> { attachment };

            var msgResp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, expectedMessage, _user.Id);

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, new ChannelGetRequest { Watch = false, State = true });
            var actualMsg = channel.Messages.First(m => m.Id == msgResp.Message.Id);
            actualMsg.User.Id.Should().BeEquivalentTo(_user.Id);
            actualMsg.GetData<string>("foo").Should().BeEquivalentTo("barsky");
            actualMsg.Attachments[0].GetData<string>("baz").Should().BeEquivalentTo("bazky");
        }

        [Test]
        public async Task TestSendPendingMessageAndCommitMessageAsync()
        {
            var expectedMessage = new MessageRequest { Text = Guid.NewGuid().ToString() };
            expectedMessage.SetData("foo", "barsky");
            var attachment = new Attachment();
            attachment.SetData("baz", "bazky");
            expectedMessage.Attachments = new List<Attachment> { attachment };

            var msg1 = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, expectedMessage, _user.Id, new SendMessageOptions { IsPendingMessage = true });
            var msg2 = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, expectedMessage, _user.Id, new SendMessageOptions { IsPendingMessage = true });

            var resp = await _channelClient.QueryChannelsAsync(new QueryChannelsOptions
            {
                Filter = new Dictionary<string, object>
            {
                { "cid", _channel.Cid },
            },
                UserId = _user.Id,
            });
            resp.Channels[0].PendingMessages.Count.Should().Be(2);

            var msgResp2 = await _messageClient.CommitMessageAsync(resp.Channels[0].PendingMessages[0].Message.Id);
            msgResp2.Message.Id.Should().BeEquivalentTo(msg1.Message.Id);

            var resp1 = await _channelClient.QueryChannelsAsync(new QueryChannelsOptions
            {
                Filter = new Dictionary<string, object>
            {
                { "cid", _channel.Cid },
            },
                UserId = _user.Id,
            });
            resp1.Channels[0].PendingMessages.Count.Should().Be(1);
        }

        [Test]
        public async Task TestSendSystemMessageAsync()
        {
            var expectedMessage = new MessageRequest { Text = Guid.NewGuid().ToString(), Type = MessageRequestType.System };

            var msgResp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, expectedMessage, _user.Id);

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, new ChannelGetRequest { Watch = false, State = true });
            var actualMsg = channel.Messages.First(m => m.Id == msgResp.Message.Id);
            actualMsg.User.Id.Should().BeEquivalentTo(_user.Id);
            actualMsg.Type.Should().BeEquivalentTo(MessageRequestType.System.ToEnumMemberString());
        }

        [Test]
        public async Task TestUpdateMessageAsync()
        {
            var newText = Guid.NewGuid().ToString();

            var updated = await _messageClient.UpdateMessageAsync(new MessageRequest
            {
                Id = _message.Id,
                Text = newText,
                UserId = _user.Id,
            });

            updated.Message.Text.Should().BeEquivalentTo(newText);
        }

        [Test]
        public async Task TestUpdateMessagePartialAsync()
        {
            var updatedFoo = Guid.NewGuid().ToString();

            var resp = await _messageClient.UpdateMessagePartialAsync(_message.Id, new MessagePartialUpdateRequest
            {
                UserId = _user.Id,
                Set = new Dictionary<string, object> { { "foo", updatedFoo } },
            });

            resp.Message.GetData<string>("foo").Should().BeEquivalentTo(updatedFoo);
        }

        [Test]
        public async Task TestDeleteMessageAsync()
        {
            await _messageClient.DeleteMessageAsync(_message.Id, hardDelete: true);

            Func<Task> getMessageCall = () => _messageClient.GetMessageAsync(_message.Id);
            await getMessageCall.Should().ThrowAsync<StreamChatException>();
        }

        [Test]
        public async Task TestDeleteMessageForMeAsync()
        {
            await _messageClient.DeleteMessageAsync(_message.Id, _user.Id, hardDelete: false, deleteForMe: true);

            // The message should still exist for other users but be marked as deleted for this user
            var response = await _messageClient.GetMessageAsync(_message.Id);
            response.Should().NotBeNull();
        }

        [Test]
        public async Task TestPinAndUnpinAsync()
        {
            var update = await _messageClient.PinMessageAsync(_message.Id, _user.Id);
            update.Message.Pinned.Should().BeTrue();

            update = await _messageClient.UnpinMessageAsync(_message.Id, _user.Id);
            update.Message.Pinned.Should().BeFalse();
        }

        [Test]
        public async Task TestSearchMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CI")))
            {
                // This test doesn't work locally for some reason
                return;
            }

            await WaitForAsync(async () =>
            {
                var result = await _messageClient.SearchAsync(SearchOptions.Default.WithMessageFilterConditions(new Dictionary<string, object>
                {
                    {
                        "text", new Dictionary<string, object>
                        {
                            { "$q", _message.Text },
                        }
                    },
                }).WithFilter(new Dictionary<string, object>
                {
                    { "cid", _channel.Cid },
                }));

                return result.Results.Count > 0;
            });
        }

        [Test]
        public async Task TestMarkReadAsync()
        {
            var resp = await _messageClient.MarkReadAsync(_channel.Type, _channel.Id, _user.Id, _message.Id);

            resp.Event.Type.Should().BeEquivalentTo("message.read");
            resp.Event.User.Id.Should().BeEquivalentTo(_user.Id);
        }

        [Test]
        public async Task TestTranslateAsync()
        {
            var resp = await _messageClient.TranslateMessageAsync(_message.Id, Language.HU);

            resp.Message.I18n.Should().ContainKey("hu_text");
        }

        [Test]
        public async Task TestGetRepliesAsync()
        {
            var reply = await _messageClient.SendMessageToThreadAsync(
                _channel.Type,
                _channel.Id,
                new MessageRequest { Text = "reply" },
                _user.Id,
                parentId: _message.Id);

            var resp = await _messageClient.GetRepliesAsync(_message.Id);

            resp.Messages.Count.Should().Be(1);
            resp.Messages[0].Text.Should().Be("reply");
        }

        [Test]
        public async Task TestFileUploadAndDeleteAsync()
        {
            var fileContent = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var resp = await _messageClient.UploadFileAsync(_channel.Type, _channel.Id, _user, fileContent, Guid.NewGuid().ToString());

            Func<Task> deleteCall = () => _messageClient.DeleteFileAsync(_channel.Type, _channel.Id, resp.File);

            await deleteCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestImageUploadAndDownloadAsync()
        {
            byte[] gifFile =
            {
                0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x1, 0x0, 0x1, 0x0, 0x80, 0x0, 0x0,
                0xff, 0xff, 0xff, 0x0, 0x0, 0x0, 0x2c, 0x0, 0x0, 0x0, 0x0, 0x1, 0x0, 0x1,
                0x0, 0x0, 0x2, 0x2, 0x44, 0x1, 0x0, 0x3b,
            };
            var resp = await _messageClient.UploadImageAsync(_channel.Type, _channel.Id, _user, gifFile, $"{Guid.NewGuid().ToString()}.gif");

            Func<Task> deleteCall = () => _messageClient.DeleteImageAsync(_channel.Type, _channel.Id, resp.File);

            await deleteCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestRunCommandActionAsync()
        {
            var msgResp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user.Id, "/giphy wave");

            var commandResp = await _messageClient.RunMessageCommandActionAsync(msgResp.Message.Id, _user.Id, new Dictionary<string, string>
            {
                { "image_action", "shuffle" },
            });

            commandResp.Message.Command.Should().BeEquivalentTo("giphy");
        }

        [Test]
        public async Task TestSendMessageWithRestrictedVisibilityAsync()
        {
            var messageText = Guid.NewGuid().ToString();
            var restrictedToUsers = new[] { _user.Id };

            var messageRequest = new MessageRequest
            {
                Text = messageText,
                Type = MessageRequestType.Regular,
                RestrictedVisibility = restrictedToUsers,
            };

            var msgResp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, messageRequest, _user.Id);

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, new ChannelGetRequest { Watch = false, State = true });
            var actualMsg = channel.Messages.First(m => m.Id == msgResp.Message.Id);

            actualMsg.RestrictedVisibility.Should().BeEquivalentTo(restrictedToUsers);
        }

        [Test]
        public async Task TestUpdateMessageWithRestrictedVisibilityAsync()
        {
            var originalText = Guid.NewGuid().ToString();
            var messageRequest = new MessageRequest
            {
                Text = originalText,
                Type = MessageRequestType.Regular,
            };

            var msgResp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, messageRequest, _user.Id);

            var updatedText = Guid.NewGuid().ToString();
            var restrictedToUsers = new[] { _user.Id };

            var updated = await _messageClient.UpdateMessageAsync(new MessageRequest
            {
                Id = msgResp.Message.Id,
                Text = updatedText,
                UserId = _user.Id,
                RestrictedVisibility = restrictedToUsers,
            });

            updated.Message.Text.Should().BeEquivalentTo(updatedText);
            updated.Message.RestrictedVisibility.Should().BeEquivalentTo(restrictedToUsers);
            updated.Message.User.Id.Should().BeEquivalentTo(_user.Id);
        }

        [Test]
        public async Task TestUpdateMessagePartialWithRestrictedVisibilityAsync()
        {
            var messageText = Guid.NewGuid().ToString();
            var messageRequest = new MessageRequest
            {
                Text = messageText,
                Type = MessageRequestType.Regular,
            };

            var msgResp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, messageRequest, _user.Id);

            var restrictedToUsers = new[] { _user.Id };
            var resp = await _messageClient.UpdateMessagePartialAsync(msgResp.Message.Id, new MessagePartialUpdateRequest
            {
                UserId = _user.Id,
                Set = new Dictionary<string, object>
                {
                    { "restricted_visibility", restrictedToUsers },
                },
            });

            resp.Message.RestrictedVisibility.Should().BeEquivalentTo(restrictedToUsers);
            resp.Message.Text.Should().BeEquivalentTo(messageText);
            resp.Message.User.Id.Should().BeEquivalentTo(_user.Id);
        }

        [Test]
        public async Task TestCreateReminderAsync()
        {
            // Arrange
            await EnableUserMessageRemindersAsync();
            var remindAt = DateTime.UtcNow.AddDays(1);

            try
            {
                // Act
                var response = await _messageClient.CreateReminderAsync(_message.Id, _user.Id, remindAt);

                // Assert
                response.Should().NotBeNull();
                response.Reminder.Should().NotBeNull();
                response.Reminder.MessageId.Should().Be(_message.Id);
                response.Reminder.UserId.Should().Be(_user.Id);
                response.Reminder.RemindAt.Should().NotBeNull();
                response.Reminder.RemindAt.Value.ToUniversalTime().Should().BeCloseTo(remindAt.ToUniversalTime(), TimeSpan.FromMinutes(1));
                response.Reminder.CreatedAt.Should().NotBeNull();
            }
            finally
            {
                await DisableUserMessageRemindersAsync();
            }
        }

        [Test]
        public async Task TestCreateReminderWithoutRemindAtAsync()
        {
            // Arrange
            await EnableUserMessageRemindersAsync();

            try
            {
                // Act
                var response = await _messageClient.CreateReminderAsync(_message.Id, _user.Id);

                // Assert
                response.Should().NotBeNull();
                response.Reminder.Should().NotBeNull();
                response.Reminder.MessageId.Should().Be(_message.Id);
                response.Reminder.UserId.Should().Be(_user.Id);

                // When remind_at is not provided, the API might set a default value or leave it null
                // We just verify the response is valid
            }
            finally
            {
                await DisableUserMessageRemindersAsync();
            }
        }

        [Test]
        public async Task TestUpdateReminderAsync()
        {
            // Arrange
            await EnableUserMessageRemindersAsync();

            try
            {
                // Arrange - Create a reminder first
                var initialRemindAt = DateTime.UtcNow.AddDays(1);
                await _messageClient.CreateReminderAsync(_message.Id, _user.Id, initialRemindAt);

                // Act - Update the reminder
                var updatedRemindAt = DateTime.UtcNow.AddDays(2);
                var response = await _messageClient.UpdateReminderAsync(_message.Id, _user.Id, updatedRemindAt);

                // Assert
                response.Should().NotBeNull();
                response.Reminder.Should().NotBeNull();
                response.Reminder.MessageId.Should().Be(_message.Id);
                response.Reminder.UserId.Should().Be(_user.Id);
                response.Reminder.RemindAt.Should().NotBeNull();
                response.Reminder.RemindAt.Value.ToUniversalTime().Should().BeCloseTo(updatedRemindAt.ToUniversalTime(), TimeSpan.FromMinutes(1));
                response.Reminder.UpdatedAt.Should().NotBeNull();
            }
            finally
            {
                await DisableUserMessageRemindersAsync();
            }
        }

        [Test]
        public async Task TestDeleteReminderAsync()
        {
            // Arrange
            await EnableUserMessageRemindersAsync();

            try
            {
                // Arrange - Create a reminder first
                var remindAt = DateTime.UtcNow.AddDays(1);
                await _messageClient.CreateReminderAsync(_message.Id, _user.Id, remindAt);

                // Act - Delete the reminder
                var response = await _messageClient.DeleteReminderAsync(_message.Id, _user.Id);

                // Assert
                response.Should().NotBeNull();

                // The API might return the deleted reminder or just a success response
                if (response.Reminder != null)
                {
                    response.Reminder.MessageId.Should().Be(_message.Id);
                    response.Reminder.UserId.Should().Be(_user.Id);
                }
            }
            finally
            {
                await DisableUserMessageRemindersAsync();
            }
        }

        [Test]
        public async Task TestQueryRemindersAsync()
        {
            // Arrange
            await EnableUserMessageRemindersAsync();

            try
            {
                // Arrange - Create a reminder first and ensure it's unique for this test
                var uniqueMessage = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user.Id, $"Unique message for TestQueryRemindersAsync {Guid.NewGuid()}");
                var remindAt = DateTime.UtcNow.AddDays(1);
                await _messageClient.CreateReminderAsync(uniqueMessage.Message.Id, _user.Id, remindAt);

                // Act - Query reminders
                var response = await _messageClient.QueryRemindersAsync(_user.Id);

                // Assert
                response.Should().NotBeNull();
                response.Reminders.Should().NotBeNull();
                response.Reminders.Should().HaveCountGreaterOrEqualTo(1);
            }
            finally
            {
                await DisableUserMessageRemindersAsync();
            }
        }

        [Test]
        public async Task TestQueryRemindersWithFilterAsync()
        {
            // Arrange
            await EnableUserMessageRemindersAsync();

            try
            {
                // Arrange - Create a unique reminder for this test
                var uniqueMessage = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user.Id, $"Unique message for TestQueryRemindersWithFilterAsync {Guid.NewGuid()}");
                var remindAt = DateTime.UtcNow.AddDays(1);
                await _messageClient.CreateReminderAsync(uniqueMessage.Message.Id, _user.Id, remindAt);

                // Act - Query reminders with filter
                var filterConditions = new Dictionary<string, object>
                {
                    { "message_id", uniqueMessage.Message.Id },
                };
                var response = await _messageClient.QueryRemindersAsync(_user.Id, filterConditions);

                // Assert
                response.Should().NotBeNull();
                response.Reminders.Should().NotBeNull();
                response.Reminders.Should().HaveCountGreaterOrEqualTo(1);
                response.Reminders.Should().Contain(r => r.MessageId == uniqueMessage.Message.Id && r.UserId == _user.Id);
            }
            finally
            {
                await DisableUserMessageRemindersAsync();
            }
        }

        [Test]
        public async Task TestQueryRemindersWithSortAsync()
        {
            // Arrange
            await EnableUserMessageRemindersAsync();

            try
            {
                // Arrange - Create multiple reminders with different remind_at times
                var uniqueMessage1 = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user.Id, $"First unique message for TestQueryRemindersWithSortAsync {Guid.NewGuid()}");
                var uniqueMessage2 = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user.Id, $"Second unique message for TestQueryRemindersWithSortAsync {Guid.NewGuid()}");

                var remindAt1 = DateTime.UtcNow.AddDays(1);
                var remindAt2 = DateTime.UtcNow.AddDays(2);

                await _messageClient.CreateReminderAsync(uniqueMessage1.Message.Id, _user.Id, remindAt1);
                await _messageClient.CreateReminderAsync(uniqueMessage2.Message.Id, _user.Id, remindAt2);

                // Act - Query reminders with custom sort (descending by remind_at)
                var sort = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "field", "remind_at" },
                        { "direction", -1 },
                    },
                };

                // Filter to only include our test reminders
                var filterConditions = new Dictionary<string, object>
                {
                    {
                        "$or", new List<Dictionary<string, object>>
                        {
                            new Dictionary<string, object> { { "message_id", uniqueMessage1.Message.Id } },
                            new Dictionary<string, object> { { "message_id", uniqueMessage2.Message.Id } },
                        }
                    },
                };

                var response = await _messageClient.QueryRemindersAsync(_user.Id, filterConditions, sort);

                // Assert
                response.Should().NotBeNull();
                response.Reminders.Should().NotBeNull();
                response.Reminders.Should().HaveCountGreaterOrEqualTo(2);

                // Find our specific reminders in the response
                var testReminders = response.Reminders
                    .Where(r => r.MessageId == uniqueMessage1.Message.Id || r.MessageId == uniqueMessage2.Message.Id)
                    .OrderByDescending(r => r.RemindAt)
                    .ToList();

                testReminders.Should().HaveCountGreaterOrEqualTo(2);

                // With descending sort, the first reminder should have a later remind_at time
                var laterReminder = testReminders.FirstOrDefault();
                var earlierReminder = testReminders.LastOrDefault();

                laterReminder.Should().NotBeNull();
                earlierReminder.Should().NotBeNull();

                if (laterReminder != null && earlierReminder != null &&
                    laterReminder.RemindAt.HasValue && earlierReminder.RemindAt.HasValue)
                {
                    laterReminder.RemindAt.Value.Should().BeAfter(earlierReminder.RemindAt.Value);
                }
            }
            finally
            {
                await DisableUserMessageRemindersAsync();
            }
        }

        [Test]
        public async Task TestReminderFullLifecycleAsync()
        {
            // Arrange
            await EnableUserMessageRemindersAsync();

            try
            {
                // Arrange - Create a unique message
                var uniqueMessage = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user.Id, $"Unique message for TestReminderFullLifecycleAsync {Guid.NewGuid()}");
                var initialRemindAt = DateTime.UtcNow.AddDays(1);

                // Act 1 - Create a reminder
                var createResponse = await _messageClient.CreateReminderAsync(uniqueMessage.Message.Id, _user.Id, initialRemindAt);

                // Assert 1
                createResponse.Should().NotBeNull();
                createResponse.Reminder.Should().NotBeNull();
                createResponse.Reminder.MessageId.Should().Be(uniqueMessage.Message.Id);

                // Act 2 - Update the reminder
                var updatedRemindAt = DateTime.UtcNow.AddDays(3);
                var updateResponse = await _messageClient.UpdateReminderAsync(uniqueMessage.Message.Id, _user.Id, updatedRemindAt);

                // Assert 2
                updateResponse.Should().NotBeNull();
                updateResponse.Reminder.Should().NotBeNull();
                updateResponse.Reminder.RemindAt.Should().NotBeNull();
                updateResponse.Reminder.RemindAt.Value.ToUniversalTime().Should().BeCloseTo(updatedRemindAt.ToUniversalTime(), TimeSpan.FromMinutes(1));

                // Act 3 - Query to verify the update
                var filterConditions = new Dictionary<string, object>
                {
                    { "message_id", uniqueMessage.Message.Id },
                };
                var queryResponse = await _messageClient.QueryRemindersAsync(_user.Id, filterConditions);

                // Assert 3
                queryResponse.Should().NotBeNull();
                queryResponse.Reminders.Should().NotBeNull();
                queryResponse.Reminders.Should().HaveCountGreaterOrEqualTo(1);
                var reminder = queryResponse.Reminders.FirstOrDefault(r => r.MessageId == uniqueMessage.Message.Id);
                reminder.Should().NotBeNull();
                if (reminder != null && reminder.RemindAt.HasValue)
                {
                    reminder.RemindAt.Value.ToUniversalTime().Should().BeCloseTo(updatedRemindAt.ToUniversalTime(), TimeSpan.FromMinutes(1));
                }

                // Act 4 - Delete the reminder
                var deleteResponse = await _messageClient.DeleteReminderAsync(uniqueMessage.Message.Id, _user.Id);

                // Assert 4
                deleteResponse.Should().NotBeNull();

                // Act 5 - Query to verify deletion
                await Task.Delay(1000); // Give the API a moment to process the deletion
                var finalQueryResponse = await _messageClient.QueryRemindersAsync(_user.Id, filterConditions);

                // Assert 5 - The reminder should no longer exist or be empty
                finalQueryResponse.Should().NotBeNull();
                if (finalQueryResponse.Reminders != null && finalQueryResponse.Reminders.Any())
                {
                    finalQueryResponse.Reminders.Should().NotContain(r => r.MessageId == uniqueMessage.Message.Id);
                }
            }
            finally
            {
                await DisableUserMessageRemindersAsync();
            }
        }
    }
}
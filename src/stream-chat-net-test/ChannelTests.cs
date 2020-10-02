using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using System.Threading.Tasks;
using StreamChat;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChatTests
{
    [Parallelizable(ParallelScope.None)]
    [TestFixture]
    public class MessageTests
    {
        private IClient _client;

        [SetUp]
        public void Setup()
        {
            _client = Credentials.Instance.Client;
        }

        [Test]
        public async Task TestChannelCreate()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            user1.SetData("name", "BOB");

            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            user2.SetData("details", new Dictionary<string, string>()
            {
                {"foo", "bar"}
            });

            var members = new User[] { user1, user2 };

            await this._client.Users.UpdateMany(members);
            var chanData = new GenericData();
            chanData.SetData("name", "one big party");
            chanData.SetData("food", new string[] { "pizza", "gabagool" });
            var channel = this._client.Channel("messaging", null, chanData);

            var chanState = await channel.Create(user1.ID, members.Select(u => u.ID));

            Assert.AreEqual(chanState.Channel.ID, channel.ID);
            Assert.AreEqual(2, chanState.Channel.MemberCount);
            Assert.AreEqual(2, chanState.Members.Count);

            var u1 = chanState.Members.Find(u => u.User.ID == user1.ID);
            Assert.NotNull(u1);

            Assert.AreEqual(u1.Role, ChannelRole.Owner);
            Assert.AreEqual(u1.User.GetData<string>("name"), user1.GetData<string>("name"));
            Assert.NotNull(u1.UpdatedAt);

            var u2 = chanState.Members.Find(u => u.User.ID == user2.ID);
            Assert.NotNull(u2);
            Assert.AreEqual(u2.Role, ChannelRole.Member);
            Assert.AreEqual(u2.User.GetData<Dictionary<string, string>>("details")["foo"], user2.GetData<Dictionary<string, string>>("details")["foo"]);
            Assert.NotNull(u2.UpdatedAt);

            Assert.NotNull(chanState.Channel.CreatedBy);
            Assert.AreEqual(chanState.Channel.CreatedBy.ID, user1.ID);
            Assert.AreEqual(chanState.Channel.CreatedBy.Role, user1.Role);
            Assert.AreEqual(chanState.Channel.GetData<string>("name"), chanData.GetData<string>("name"));
            Assert.AreEqual(chanState.Channel.GetData<string[]>("food"), chanData.GetData<string[]>("food"));

            var chanId = Guid.NewGuid().ToString();
            var channel2 = this._client.Channel("messaging", chanId);
            chanState = await channel2.Create(user2.ID);
            Assert.AreEqual(chanState.Channel.ID, channel2.ID);
            Assert.AreEqual(chanId, channel2.ID);
            Assert.AreEqual(0, chanState.Channel.MemberCount);
            Assert.AreEqual(0, chanState.Members.Count);
            Assert.AreEqual(chanState.Channel.CreatedBy.ID, user2.ID);
        }

        [Test]
        public async Task TestSendMessage()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Update(user1);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());
            await channel.Create(user1.ID, new string[] { user1.ID });

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };
            inMsg.SetData("foo", "barsky");

            var outMsg = await channel.SendMessage(inMsg, user1.ID);

            Assert.NotNull(outMsg);
            Assert.NotNull(outMsg.ID);
            Assert.NotZero(outMsg.ID.Count());
            Assert.NotNull(outMsg.CreatedAt);
            Assert.AreEqual(inMsg.Text, outMsg.Text);
            Assert.NotNull(outMsg.User);
            Assert.AreEqual(inMsg.User.ID, outMsg.User.ID);
            Assert.AreEqual("barsky", outMsg.GetData<string>("foo"));

            var chanState = await channel.Query(new ChannelQueryParams(false, true));

            Assert.NotNull(chanState);
            Assert.NotNull(chanState.Messages);
            Assert.AreEqual(1, chanState.Messages.Count);
            Assert.AreEqual(outMsg.ID, chanState.Messages[0].ID);
            Assert.AreEqual("barsky", chanState.Messages[0].GetData<string>("foo"));
        }

        [Test]
        public async Task TestSendEvent()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Update(user1);
            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());

            await channel.Create(user1.ID);

            var inEvt = new Event()
            {
                Type = EventType.TypingStart
            };
            inEvt.SetData("foo", new int[] { 1 });

            var outEvt = await channel.SendEvent(inEvt, user1.ID);

            Assert.NotNull(outEvt);
            Assert.NotNull(outEvt.CreatedAt);
            Assert.NotNull(outEvt.User);
            Assert.AreEqual(inEvt.Type, outEvt.Type);
            Assert.AreEqual(user1.ID, outEvt.User.ID);
            Assert.AreEqual(1, outEvt.GetData<int[]>("foo")[0]);
        }

        [Test]
        public async Task TestSendReaction()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Update(user1);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());
            await channel.Create(user1.ID, new string[] { user1.ID });

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };

            var outMsg = await channel.SendMessage(inMsg, user1.ID);

            var inReaction = new Reaction()
            {
                Type = "like"
            };
            inReaction.SetData("foo", "bar");

            var reactionResp = await channel.SendReaction(outMsg.ID, inReaction, user1.ID);

            Assert.NotNull(reactionResp);
            Assert.NotNull(reactionResp.Message);
            Assert.AreEqual(reactionResp.Message.ID, outMsg.ID);
            Assert.AreEqual(1, reactionResp.Message.ReactionCounts["like"]);
            Assert.AreEqual(1, reactionResp.Message.LatestReactions.Count);
            Assert.NotNull(reactionResp.Reaction);
            Assert.AreEqual(inReaction.Type, reactionResp.Reaction.Type);
            Assert.NotNull(reactionResp.Reaction.CreatedAt);
            Assert.AreEqual(reactionResp.Reaction.User.ID, user1.ID);
            Assert.AreEqual(reactionResp.Reaction.MessageID, outMsg.ID);
            Assert.AreEqual("bar", reactionResp.Reaction.GetData<string>("foo"));

            var reactions = await channel.GetReactions(outMsg.ID);
            Assert.NotNull(reactions);
            Assert.AreEqual(1, reactions.Count);
        }

        [Test]
        public async Task TestDeleteReaction()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };

            var members = new User[] { user1, user2 };

            await this._client.Users.UpdateMany(members);
            var channel = this._client.Channel("messaging");

            await channel.Create(user1.ID, members.Select(u => u.ID));

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };

            var outMsg = await channel.SendMessage(inMsg, user1.ID);

            var reaction1 = new Reaction()
            {
                Type = "like"
            };
            reaction1.SetData("foo", "bar");

            await channel.SendReaction(outMsg.ID, reaction1, user1.ID);

            var reaction2 = new Reaction()
            {
                Type = "love"
            };
            reaction2.SetData("some", "data");

            await channel.SendReaction(outMsg.ID, reaction2, user1.ID);

            var reaction3 = new Reaction()
            {
                Type = "like"
            };

            await channel.SendReaction(outMsg.ID, reaction3, user2.ID);

            Assert.ThrowsAsync<StreamChatException>(async () =>
            {
                await channel.DeleteReaction(outMsg.ID, reaction2.Type, user2.ID);
            });

            var reactionResponse = await channel.DeleteReaction(outMsg.ID, reaction2.Type, user1.ID);
            Assert.NotNull(reactionResponse);
            Assert.AreEqual(outMsg.ID, reactionResponse.Message.ID);
            Assert.AreEqual(reaction2.Type, reactionResponse.Reaction.Type);
            Assert.AreEqual(user1.ID, reactionResponse.Reaction.User.ID);

            var reactions = await channel.GetReactions(outMsg.ID);
            Assert.NotNull(reactions);
            Assert.AreEqual(2, reactions.Count);
            Assert.AreEqual(reaction3.Type, reactions[0].Type);
            Assert.AreEqual(user2.ID, reactions[0].User.ID);
            Assert.AreEqual(reaction1.Type, reactions[1].Type);
            Assert.AreEqual(user1.ID, reactions[1].User.ID);
        }

        [Test]
        public async Task TestGetReactions()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };

            var members = new User[] { user1, user2 };

            await this._client.Users.UpdateMany(members);
            var channel = this._client.Channel("messaging");

            await channel.Create(user1.ID, members.Select(u => u.ID));

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };

            var outMsg = await channel.SendMessage(inMsg, user1.ID);

            var reaction1 = new Reaction()
            {
                Type = "like"
            };
            reaction1.SetData("foo", "bar");

            await channel.SendReaction(outMsg.ID, reaction1, user1.ID);

            var reaction2 = new Reaction()
            {
                Type = "love"
            };
            reaction2.SetData("some", "data");

            await channel.SendReaction(outMsg.ID, reaction2, user1.ID);

            var reaction3 = new Reaction()
            {
                Type = "like"
            };

            await channel.SendReaction(outMsg.ID, reaction3, user2.ID);

            var reactions = await channel.GetReactions(outMsg.ID);
            Assert.NotNull(reactions);
            Assert.AreEqual(3, reactions.Count);

            Assert.AreEqual(reaction3.Type, reactions[0].Type);
            Assert.AreEqual(user2.ID, reactions[0].User.ID);

            Assert.AreEqual(reaction2.Type, reactions[1].Type);
            Assert.AreEqual(user1.ID, reactions[1].User.ID);

            Assert.AreEqual(reaction1.Type, reactions[2].Type);
            Assert.AreEqual(user1.ID, reactions[2].User.ID);

            reactions = await channel.GetReactions(outMsg.ID, 0, 2);
            Assert.NotNull(reactions);
            Assert.AreEqual(2, reactions.Count);

            Assert.AreEqual(reaction3.Type, reactions[0].Type);
            Assert.AreEqual(user2.ID, reactions[0].User.ID);

            Assert.AreEqual(reaction2.Type, reactions[1].Type);
            Assert.AreEqual(user1.ID, reactions[1].User.ID);

            reactions = await channel.GetReactions(outMsg.ID, 2);
            Assert.NotNull(reactions);
            Assert.AreEqual(1, reactions.Count);

            Assert.AreEqual(reaction1.Type, reactions[0].Type);
            Assert.AreEqual(user1.ID, reactions[0].User.ID);
        }

        [Test]
        public async Task TestQueryChannel()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };

            var user3 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user4 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Guest,
            };

            var members = new User[] { user1, user2, user3, user4 };

            await this._client.Users.UpdateMany(members);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());

            await channel.Create(user1.ID);

            foreach (var u in members)
            {
                await channel.AddMembers(new string[] { u.ID });
            }

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };

            var msg1 = await channel.SendMessage(inMsg, user1.ID);
            inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };
            var msg2 = await channel.SendMessage(inMsg, user2.ID);

            inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };
            var msg3 = await channel.SendMessage(inMsg, user3.ID);

            var qParams = new ChannelQueryParams();
            var chanState = await channel.Query(qParams);

            Assert.NotNull(chanState.Members);
            Assert.AreEqual(0, chanState.Messages.Count);
            Assert.IsNull(chanState.Watchers);

            Assert.AreEqual(4, chanState.Members.Count);
            Assert.AreEqual(user1.ID, chanState.Members[0].User.ID);
            Assert.AreEqual(user2.ID, chanState.Members[1].User.ID);
            Assert.AreEqual(user3.ID, chanState.Members[2].User.ID);
            Assert.AreEqual(user4.ID, chanState.Members[3].User.ID);

            qParams.MembersPagination = new PaginationParams()
            {
                Limit = 2
            };
            qParams.State = true;
            chanState = await channel.Query(qParams);

            Assert.NotNull(chanState.Members);
            Assert.IsNull(chanState.Watchers);

            Assert.AreEqual(2, chanState.Members.Count);
            Assert.AreEqual(user1.ID, chanState.Members[0].User.ID);
            Assert.AreEqual(user2.ID, chanState.Members[1].User.ID);

            qParams.MembersPagination = new PaginationParams()
            {
                Offset = 2,
                Limit = 1
            };
            chanState = await channel.Query(qParams);

            Assert.NotNull(chanState.Members);
            Assert.IsNull(chanState.Watchers);

            Assert.AreEqual(1, chanState.Members.Count);
            Assert.AreEqual(user3.ID, chanState.Members[0].User.ID);

            qParams = new ChannelQueryParams()
            {
                State = true,
            };

            chanState = await channel.Query(qParams);
            Assert.AreEqual(3, chanState.Messages.Count);

            Assert.AreEqual(msg1.ID, chanState.Messages[0].ID);
            Assert.AreEqual(msg2.ID, chanState.Messages[1].ID);
            Assert.AreEqual(msg3.ID, chanState.Messages[2].ID);

            qParams.MessagesPagination = new MessagePaginationParams()
            {
                Limit = 1
            };

            chanState = await channel.Query(qParams);
            Assert.AreEqual(1, chanState.Messages.Count);

            var pivot = chanState.Messages[0];
            qParams.MessagesPagination = new MessagePaginationParams()
            {
                Limit = 2,
                IDLT = pivot.ID
            };
            chanState = await channel.Query(qParams);
            Assert.AreEqual(2, chanState.Messages.Count);
            Assert.IsNull(chanState.Messages.Find(x => x.CreatedAt.Value.CompareTo(pivot.CreatedAt) == 1));
        }

        [Test]
        public async Task TestUpdate()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var customData = new GenericData();
            customData.SetData("foo", "bar");
            await this._client.Users.Update(user1);
            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString(), customData);

            await channel.Create(user1.ID);

            var newData = new GenericData();
            newData.SetData("updated", "stuff");
            var updateChanResponse = await channel.Update(newData);

            Assert.IsNotNull(updateChanResponse);
            Assert.IsNull(updateChanResponse.Message);
            Assert.IsNotNull(updateChanResponse.Channel);
            Assert.IsNull(updateChanResponse.Channel.GetData<string>("foo"));
            Assert.AreEqual("stuff", updateChanResponse.Channel.GetData<string>("updated"));

            newData = new GenericData();
            newData.SetData("more complex", new Dictionary<string, int>() { { "field", 123 } });
            var msg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString(),
                User = user1
            };

            updateChanResponse = await channel.Update(newData, msg);
            Assert.IsNotNull(updateChanResponse);
            Assert.IsNotNull(updateChanResponse.Channel);
            Assert.IsNull(updateChanResponse.Channel.GetData<string>("foo"));
            Assert.IsNull(updateChanResponse.Channel.GetData<string>("updated"));
            Assert.AreEqual(123, updateChanResponse.Channel.GetData<Dictionary<string, int>>("more complex")["field"]);
            Assert.IsNotNull(updateChanResponse.Message);
            Assert.AreEqual(msg.Text, updateChanResponse.Message.Text);
            Assert.AreEqual(user1.ID, updateChanResponse.Message.User.ID);

            var chanState = await channel.Query(new ChannelQueryParams());
            Assert.AreEqual(123, chanState.Channel.GetData<Dictionary<string, int>>("more complex")["field"]);
        }

        [Test]
        public async Task TestDelete()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Update(user1);
            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());

            var chanState = await channel.Create(user1.ID);
            Assert.IsNull(chanState.Channel.DeletedAt);

            await channel.Delete();

            var chanStates = await this._client.QueryChannels(QueryChannelsOptions.Default.WithFilter(new Dictionary<string, object>()
            {
                {"id", channel.ID}
            }));

            Assert.AreEqual(0, chanStates.Count);
        }

        [Test]
        public async Task TestTruncate()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };

            var user3 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user4 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Guest,
            };

            var members = new User[] { user1, user2, user3, user4 };

            await this._client.Users.UpdateMany(members);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());

            await channel.Create(user1.ID);

            foreach (var u in members)
            {
                await channel.AddMembers(new string[] { u.ID });
            }

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };

            var msg1 = await channel.SendMessage(inMsg, user1.ID);
            inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };
            var msg2 = await channel.SendMessage(inMsg, user2.ID);

            inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };
            var msg3 = await channel.SendMessage(inMsg, user3.ID);

            var chanState = await channel.Query(new ChannelQueryParams(false, true));
            Assert.AreEqual(3, chanState.Messages.Count);

            await channel.Truncate();

            chanState = await channel.Query(new ChannelQueryParams(false, true));

            Assert.AreEqual(0, chanState.Messages.Count);
        }

        [Test]
        public async Task TestMarkRead()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Update(user1);
            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());

            await channel.Create(user1.ID);

            var readEvent = await channel.MarkRead(user1.ID);
            Assert.NotNull(readEvent);
            Assert.AreEqual(EventType.MessageRead, readEvent.Type);
            Assert.AreEqual(user1.ID, readEvent.User.ID);

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };

            var msg1 = await channel.SendMessage(inMsg, user1.ID);

            readEvent = await channel.MarkRead(user1.ID, msg1.ID);
            Assert.NotNull(readEvent);
            Assert.AreEqual(EventType.MessageRead, readEvent.Type);
            Assert.AreEqual(user1.ID, readEvent.User.ID);
        }

        [Test]
        public async Task TestGetReplies()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Update(user1);
            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());

            await channel.Create(user1.ID);

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };

            var msg1 = await channel.SendMessage(inMsg, user1.ID);

            inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };

            var reply1 = await channel.SendMessage(inMsg, user1.ID, msg1.ID);

            inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };

            var reply2 = await channel.SendMessage(inMsg, user1.ID, msg1.ID);

            inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };

            var reply3 = await channel.SendMessage(inMsg, user1.ID, msg1.ID);

            var replies = await channel.GetReplies(msg1.ID, MessagePaginationParams.Default);
            Assert.AreEqual(3, replies.Count);
            Assert.AreEqual(3, replies.FindAll(m => m.ParentID == msg1.ID).Count);
            Assert.AreEqual(reply1.ID, replies[0].ID);
            Assert.AreEqual(reply2.ID, replies[1].ID);
            Assert.AreEqual(reply3.ID, replies[2].ID);

            var pagination = new MessagePaginationParams()
            {
                Limit = 1
            };
            replies = await channel.GetReplies(msg1.ID, pagination);
            Assert.AreEqual(1, replies.Count);
            Assert.AreEqual(reply3.ID, replies[0].ID);

            pagination.IDLT = reply2.ID;
            replies = await channel.GetReplies(msg1.ID, pagination);
            Assert.AreEqual(1, replies.Count);
            Assert.AreEqual(reply1.ID, replies[0].ID);
        }

        [Test]
        public async Task TestAddMembers()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };

            var user3 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user4 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Guest,
            };
            var members = new User[] { user1, user2, user3, user4 };

            await this._client.Users.UpdateMany(members);
            var channel = this._client.Channel("messaging", System.Guid.NewGuid().ToString());

            await channel.Create(user1.ID);

            await channel.AddMembers(members.Select(u => u.ID));

            var chanState = await channel.Query(new ChannelQueryParams());
            Assert.AreEqual(4, chanState.Channel.MemberCount);
            Assert.AreEqual(4, chanState.Members.Count);

            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user1.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user2.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user3.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user4.ID));
        }

        [Test]
        public async Task TestRemoveMembers()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };

            var user3 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user4 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Guest,
            };

            var members = new User[] { user1, user2, user3, user4 };

            await this._client.Users.UpdateMany(members);

            var channel = this._client.Channel("messaging", System.Guid.NewGuid().ToString());

            await channel.Create(user1.ID, members.Select(u => u.ID));

            await channel.RemoveMembers(new string[] { user3.ID });

            var chanState = await channel.Query(new ChannelQueryParams());
            Assert.AreEqual(3, chanState.Channel.MemberCount);
            Assert.AreEqual(3, chanState.Members.Count);

            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user1.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user2.ID));
            Assert.Null(chanState.Members.Find(u => u.User.ID == user3.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user4.ID));
        }

        [Test]
        public async Task TestAddModerators()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };

            var user3 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user4 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Guest,
            };

            var members = new User[] { user1, user2, user3, user4 };

            await this._client.Users.UpdateMany(members);
            var channel = this._client.Channel("messaging");

            await channel.Create(user1.ID, members.Select(u => u.ID));

            await channel.AddModerators(new string[] { user2.ID });

            var chanState = await channel.Query(new ChannelQueryParams());
            Assert.AreEqual(4, chanState.Channel.MemberCount);
            Assert.AreEqual(4, chanState.Members.Count);

            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user1.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user3.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user4.ID));

            var u2 = chanState.Members.Find(u => u.User.ID == user2.ID);
            Assert.NotNull(u2);
            Assert.AreEqual(ChannelRole.Moderator, u2.Role);
        }

        [Test]
        public async Task TestDemoteModerators()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };

            var user3 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user4 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Guest,
            };

            var members = new User[] { user1, user2, user3, user4 };

            await this._client.Users.UpdateMany(members);
            var channel = this._client.Channel("messaging");

            await channel.Create(user1.ID, members.Select(u => u.ID));

            await channel.AddModerators(new string[] { user2.ID });

            await channel.DemoteModerators(new string[] { user2.ID });

            var chanState = await channel.Query(new ChannelQueryParams());
            Assert.AreEqual(4, chanState.Channel.MemberCount);
            Assert.AreEqual(4, chanState.Members.Count);

            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user1.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user3.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user4.ID));

            var u2 = chanState.Members.Find(u => u.User.ID == user2.ID);
            Assert.NotNull(u2);
            Assert.AreEqual(ChannelRole.Member, u2.Role);
        }

        [Test]
        public async Task TestBanUser()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };

            var members = new User[] { user1, user2 };

            await this._client.Users.UpdateMany(members);
            var channel = this._client.Channel("messaging");

            await channel.Create(user1.ID, members.Select(u => u.ID));
            await channel.BanUser(user2.ID, user1.ID, Guid.NewGuid().ToString(), 3);
        }

        [Test]
        public async Task TestUnbanUser()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };

            var members = new User[] { user1, user2 };

            await this._client.Users.UpdateMany(members);
            var channel = this._client.Channel("messaging");

            await channel.Create(user1.ID, members.Select(u => u.ID));
            await channel.BanUser(user2.ID, user1.ID, Guid.NewGuid().ToString(), 3);
            await channel.UnbanUser(user2.ID);
        }
    }
}

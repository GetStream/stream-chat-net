using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using System.Threading.Tasks;
using StreamChat;

namespace StreamChatTests
{
    [Parallelizable(ParallelScope.None)]
    [TestFixture]
    public class UserTests
    {
        private IUsers _endpoint;
        private IClient _client;


        [SetUp]
        public void Setup()
        {
            _client = Credentials.Instance.Client;
            _endpoint = _client.Users;
        }

        [Test]
        public async Task TestUpdate()
        {
            var user = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            user.SetData("name", "BOB");
            user.SetData("details", new Dictionary<string, string>()
            {
                {"foo", "bar"}
            });

            var result = await this._endpoint.Update(user);
            Assert.AreEqual(result.ID, user.ID);
            Assert.AreEqual(result.Role, user.Role);
            Assert.AreEqual(result.GetData<string>("name"), user.GetData<string>("name"));
            Assert.AreEqual(result.GetData<Dictionary<string, string>>("details")["foo"], user.GetData<Dictionary<string, string>>("details")["foo"]);
            Assert.NotNull(result.UpdatedAt);
        }

        [Test]
        public async Task TestUpdateMany()
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

            var results = await this._endpoint.UpdateMany(new User[] { user1, user2 });
            Assert.AreEqual(2, results.Count());
            var u1 = results.ToList().Find(u => u.ID == user1.ID);
            Assert.NotNull(u1);
            Assert.AreEqual(u1.Role, user1.Role);
            Assert.AreEqual(u1.GetData<string>("name"), user1.GetData<string>("name"));
            Assert.NotNull(u1.UpdatedAt);

            var u2 = results.ToList().Find(u => u.ID == user2.ID);
            Assert.NotNull(u2);
            Assert.AreEqual(u2.Role, user2.Role);
            Assert.AreEqual(u2.GetData<Dictionary<string, string>>("details")["foo"], user2.GetData<Dictionary<string, string>>("details")["foo"]);
            Assert.NotNull(u2.UpdatedAt);
        }

        [Test]
        public async Task TestUpdatePartial()
        {
            var user = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            user.SetData("field", "value");

            await this._endpoint.Update(user);
            var result = this._endpoint.UpdatePartial(new UpdatePartialRequest() {
                ID = user.ID,
                Set = new Object(){
                    field = "updated",
                }
            });

            Assert.AreEqual(result.ID, user.ID);
            Assert.AreEqual(result.Role, user.Role);
            Assert.AreEqual(result.GetData<string>("field"), "updated");
        }

        [Test]
        public async Task TestDelete()
        {
            var user = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            user.SetData("name", "BOB");
            user.SetData("details", new Dictionary<string, string>()
            {
                {"foo", "bar"}
            });

            await this._endpoint.Update(user);

            var result = await this._endpoint.Delete(user.ID);
            Assert.AreEqual(result.ID, user.ID);
            Assert.AreEqual(result.Role, user.Role);
            Assert.AreEqual(result.GetData<string>("name"), user.GetData<string>("name"));
            Assert.AreEqual(result.GetData<Dictionary<string, string>>("details")["foo"], user.GetData<Dictionary<string, string>>("details")["foo"]);
            Assert.NotNull(result.DeletedAt);
        }

        [Test]
        public async Task TestDeactivate()
        {
            var user = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            user.SetData("name", "BOB");
            user.SetData("details", new Dictionary<string, string>()
            {
                {"foo", "bar"}
            });

            await this._endpoint.Update(user);

            var result = await this._endpoint.Deactivate(user.ID);
            Assert.AreEqual(result.ID, user.ID);
            Assert.AreEqual(result.Role, user.Role);
            Assert.AreEqual(result.GetData<string>("name"), user.GetData<string>("name"));
            Assert.AreEqual(result.GetData<Dictionary<string, string>>("details")["foo"], user.GetData<Dictionary<string, string>>("details")["foo"]);
            Assert.NotNull(result.DeactivatedAt);
        }

        [Test]
        public async Task TestReactivate()
        {
            var user = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            user.SetData("name", "BOB");
            user.SetData("details", new Dictionary<string, string>()
            {
                {"foo", "bar"}
            });

            await this._endpoint.Update(user);

            var result = await this._endpoint.Deactivate(user.ID);
            Assert.AreEqual(result.ID, user.ID);
            Assert.NotNull(result.DeactivatedAt);

            result = await this._endpoint.Reactivate(user.ID);
            Assert.AreEqual(result.ID, user.ID);
            Assert.Null(result.DeactivatedAt);

            Assert.ThrowsAsync<StreamChatException>(async () =>
            {
                await this._endpoint.Reactivate(user.ID);
            });
        }

        [Test]
        public async Task TestExport()
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
            var channel = _client.Channel("messaging");

            await channel.Create(user1.ID, members.Select(u => u.ID));

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };
            var msg1 = await channel.SendMessage(inMsg, user1.ID);

            inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };
            var msg2 = await channel.SendMessage(inMsg, user1.ID);

            inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };
            var reply1 = await channel.SendMessage(inMsg, user1.ID, msg1.ID);

            var inReact = new Reaction()
            {
                Type = "like"
            };
            var react1 = await channel.SendReaction(msg2.ID, inReact, user1.ID);

            var exportedUser = await this._endpoint.Export(user1.ID);
            Assert.IsNotNull(exportedUser.User);
            Assert.AreEqual(user1.ID, exportedUser.User.ID);
            Assert.AreEqual(3, exportedUser.Messages.Count);
            Assert.AreEqual(1, exportedUser.Reactions.Count);
            Assert.AreEqual(inReact.Type, exportedUser.Reactions[0].Type);
            Assert.AreEqual(msg2.ID, exportedUser.Reactions[0].MessageID);
        }

        [Test]
        public async Task TestQuery()
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
            user2.SetData("name", "Alice");

            await this._endpoint.UpdateMany(new User[] { user1, user2 });

            var opts = QueryUserOptions.Default.WithFilter(new Dictionary<string, object>() {
                {"id", user1.ID}
            });
            var results = await this._endpoint.Query(opts);
            Assert.AreEqual(1, results.Count());
            var u1 = results.First();
            Assert.AreEqual(u1.Role, user1.Role);
            Assert.AreEqual(u1.GetData<string>("name"), user1.GetData<string>("name"));
            Assert.NotNull(u1.UpdatedAt);

            opts = QueryUserOptions.Default.WithFilter(new Dictionary<string, object>() {
                {"id", new Dictionary<string,string[]>()
                    {
                        {"$in", new string[]{user1.ID, user2.ID}}
                    }
                }
            });
            results = await this._endpoint.Query(opts);
            Assert.AreEqual(2, results.Count());

            opts = opts.WithSortBy(new SortParameter
            {
                Field = "name",
                Direction = SortDirection.Ascending
            });
            results = await this._endpoint.Query(opts);
            Assert.AreEqual(2, results.Count());
            u1 = results.First();
            Assert.AreEqual(u1.ID, user2.ID);
            Assert.AreEqual(u1.GetData<string>("name"), "Alice");

            results = await this._endpoint.Query(opts.WithLimit(1));
            Assert.AreEqual(1, results.Count());
            u1 = results.First();
            Assert.AreEqual(u1.ID, user2.ID);
            Assert.AreEqual(u1.GetData<string>("name"), "Alice");

            results = await this._endpoint.Query(opts.WithLimit(1).WithOffset(1));
            Assert.AreEqual(1, results.Count());
            u1 = results.First();
            Assert.AreEqual(u1.ID, user1.ID);
            Assert.AreEqual(u1.GetData<string>("name"), "BOB");

            opts = QueryUserOptions.Default.WithFilter(new Dictionary<string, object>() {
                {"id", new Dictionary<string,string[]>()
                    {
                        {"$in", new string[]{user1.ID, user2.ID}}
                    }
                }
            }).WithSortBy(new SortParameter
            {
                Field = "name",
                Direction = SortDirection.Descending
            });

            results = await this._endpoint.Query(opts);
            Assert.AreEqual(2, results.Count());
            u1 = results.First();
            Assert.AreEqual(u1.ID, user1.ID);
            Assert.AreEqual(u1.GetData<string>("name"), "BOB");
        }

        [Test]
        public async Task TestBan()
        {
            var targetUser = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            await this._endpoint.UpdateMany(new User[] { targetUser, user });
            await this._endpoint.Ban(targetUser.ID, user.ID, Guid.NewGuid().ToString(), 5);
        }

        [Test]
        public async Task TestUnban()
        {
            var targetUser = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            await this._endpoint.UpdateMany(new User[] { targetUser, user });
            await this._endpoint.Ban(targetUser.ID, user.ID, Guid.NewGuid().ToString(), 5);
            await this._endpoint.Unban(targetUser.ID);
        }

        [Test]
        public async Task TestMute()
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
            await this._endpoint.UpdateMany(new User[] { user1, user2 });

            var muteResp = await this._endpoint.Mute(user2.ID, user1.ID);
            Assert.NotNull(muteResp.OwnUser);
            Assert.NotNull(muteResp.Mute);
            Assert.AreEqual(user1.ID, muteResp.OwnUser.ID);
            Assert.AreEqual(user2.ID, muteResp.Mute.Target.ID);
            Assert.AreEqual(user1.ID, muteResp.Mute.User.ID);
        }

        [Test]
        public async Task TestUnmute()
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
            await this._endpoint.UpdateMany(new User[] { user1, user2 });

            await this._endpoint.Unmute(user2.ID, user1.ID);
        }

        [Test]
        public async Task TestMarkAllRead()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            await this._endpoint.Update(user1);

            await this._endpoint.MarkAllRead(user1.ID);
        }
    }
}

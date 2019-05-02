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
        private Client _client;
        [SetUp]
        public void Setup()
        {
            _client = new Client("pewr3y7sksbu",
                "yxa2r93542v6bgj63gx5ayfk5buam5v82zudmc8tyt28qn9krmyfhrzx8bfavk6w",
                new ClientOptions()
                {
                    Location = ApiLocation.USEast,
                    Timeout = 10000
                });
        }

        [Test]
        public async Task TestChannelCreate()
        {
            var user1 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            user1.SetData("name", "BOB");

            var user2 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.ChannelMember,
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
            var channel = _client.Channel("messaging", null, chanData);

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
        }

        [Test]
        public async Task TestAddMembers()
        {
            var user1 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.ChannelMember,
            };

            var members = new User[] { user1, user2 };

            await this._client.Users.UpdateMany(members);
            var chanData = new GenericData();
            var channel = _client.Channel("messaging", null, chanData);

            await channel.Create(user1.ID, members.Select(u => u.ID));

            var user3 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user4 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.Guest,
            };

            var newMembers = new User[] { user3, user4 };
            await this._client.Users.UpdateMany(newMembers);

            await channel.AddMembers(newMembers.Select(u => u.ID));

            var chanState = await channel.Query(false, false, false);
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
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.ChannelMember,
            };

            var user3 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user4 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.Guest,
            };

            var members = new User[] { user1, user2, user3, user4 };

            await this._client.Users.UpdateMany(members);
            var chanData = new GenericData();
            var channel = _client.Channel("messaging", null, chanData);

            await channel.Create(user1.ID, members.Select(u => u.ID));

            await channel.RemoveMembers(new string[] { user3.ID });

            var chanState = await channel.Query(false, false, false);
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
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.ChannelMember,
            };

            var user3 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user4 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.Guest,
            };

            var members = new User[] { user1, user2, user3, user4 };

            await this._client.Users.UpdateMany(members);
            var chanData = new GenericData();
            var channel = _client.Channel("messaging", null, chanData);

            await channel.Create(user1.ID, members.Select(u => u.ID));

            await channel.AddModerators(new string[] { user2.ID });

            var chanState = await channel.Query(false, false, false);
            Assert.AreEqual(4, chanState.Channel.MemberCount);
            Assert.AreEqual(4, chanState.Members.Count);

            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user1.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user3.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user4.ID));

            var u2 = chanState.Members.Find(u => u.User.ID == user2.ID);
            Assert.NotNull(u2);
            // Assert.IsTrue(u2.IsModerator);
            Assert.AreEqual(ChannelRole.Moderator, u2.Role);
        }

        [Test]
        public async Task TestDemoteModerators()
        {
            var user1 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var user2 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.ChannelMember,
            };

            var user3 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.User,
            };
            var user4 = new User()
            {
                ID = System.Guid.NewGuid().ToString(),
                Role = Role.Guest,
            };

            var members = new User[] { user1, user2, user3, user4 };

            await this._client.Users.UpdateMany(members);
            var chanData = new GenericData();
            var channel = _client.Channel("messaging", null, chanData);

            await channel.Create(user1.ID, members.Select(u => u.ID));

            await channel.AddModerators(new string[] { user2.ID });

            await channel.DemoteModerators(new string[] { user2.ID });

            var chanState = await channel.Query(false, false, false);
            Assert.AreEqual(4, chanState.Channel.MemberCount);
            Assert.AreEqual(4, chanState.Members.Count);

            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user1.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user3.ID));
            Assert.NotNull(chanState.Members.Find(u => u.User.ID == user4.ID));

            var u2 = chanState.Members.Find(u => u.User.ID == user2.ID);
            Assert.NotNull(u2);
            // Assert.IsTrue(u2.IsModerator);
            Assert.AreEqual(ChannelRole.Member, u2.Role);
        }
    }
}

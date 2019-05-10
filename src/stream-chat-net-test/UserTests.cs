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
        private Users _endpoint;
        [SetUp]
        public void Setup()
        {
            _endpoint = Credentials.Instance.Client.Users;
        }

        [Test]
        public async Task TestUsersUpdate()
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
        public async Task TestUsersUpdateMany()
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
                Role = Role.ChannelMember,
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
        public async Task TestUsersDelete()
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
        public async Task TestUsersDeactivate()
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
        public async Task TestUsersQuery()
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
                Role = Role.ChannelMember,
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
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            await this._endpoint.Update(user1);
            await this._endpoint.Ban(user1.ID, Guid.NewGuid().ToString(), 5);
        }

        [Test]
        public async Task TestUnban()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            await this._endpoint.Update(user1);
            await this._endpoint.Ban(user1.ID, Guid.NewGuid().ToString(), 5);
            await this._endpoint.Unban(user1.ID);
        }
    }
}

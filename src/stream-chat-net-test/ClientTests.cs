using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using StreamChat;

namespace StreamChatTests
{
    [Parallelizable(ParallelScope.None)]
    [TestFixture]
    public class ClientTests
    {
        private StreamChat.IClient _client;
        [SetUp]
        public void Setup()
        {
            _client = Credentials.Instance.Client;
        }

        [Test]
        public async Task TestAppSettings()
        {
            var inSettings = new AppSettings()
            {
                WebhookURL = "http://www.example.com",
                DisableAuth = true,
                MultiTenantEnabled = true
            };
            await this._client.UpdateAppSettings(inSettings);

            var appSettings = await this._client.GetAppSettings();
            Assert.NotNull(appSettings);
            Assert.AreEqual(inSettings.WebhookURL, appSettings.WebhookURL);
            Assert.AreEqual(inSettings.DisableAuth, appSettings.DisableAuth);
            Assert.NotNull(appSettings.ChannelConfigs);
            Assert.True(appSettings.ChannelConfigs.ContainsKey("messaging"));
            Assert.True(appSettings.MultiTenantEnabled);
        }

        [Test]
        public async Task TestDevices()
        {
            //since testing device endpoints all rely on each other, we have a bit of a
            //catch 23 situation

            //add devices
            var user1 = Guid.NewGuid().ToString();
            var user2 = Guid.NewGuid().ToString();

            var d1 = new Device()
            {
                ID = Guid.NewGuid().ToString(),
                PushProvider = PushProvider.APN,
                UserID = user1
            };
            var d2 = new Device()
            {
                ID = Guid.NewGuid().ToString(),
                PushProvider = PushProvider.Firebase,
                UserID = user1
            };
            var d3 = new Device()
            {
                ID = Guid.NewGuid().ToString(),
                PushProvider = PushProvider.APN,
                UserID = user2
            };
            await this._client.AddDevice(d1);
            await this._client.AddDevice(d2);
            await this._client.AddDevice(d3);

            //retrieve devices
            var user1Devices = await this._client.GetDevices(user1);
            Assert.AreEqual(2, user1Devices.Count);
            Assert.NotNull(user1Devices.Find(d => d.ID == d1.ID && d.PushProvider == d1.PushProvider));
            Assert.NotNull(user1Devices.Find(d => d.ID == d2.ID && d.PushProvider == d2.PushProvider));

            var user2Devices = await this._client.GetDevices(user2);
            Assert.AreEqual(1, user2Devices.Count);
            Assert.NotNull(user2Devices.Find(d => d.ID == d3.ID && d.PushProvider == d3.PushProvider));

            var noDevices = await this._client.GetDevices("bogus");
            Assert.AreEqual(0, noDevices.Count);

            //delete devices
            await this._client.RemoveDevice(d1.ID, user1);
            user1Devices = await this._client.GetDevices(user1);
            Assert.AreEqual(1, user1Devices.Count);
            Assert.NotNull(user1Devices.Find(d => d.ID == d2.ID && d.PushProvider == d2.PushProvider));
        }

        [Test]
        public async Task TestGetRateLimits()
        {
            var allLimits = await this._client.GetRateLimits(new GetRateLimitsOptions());
            Assert.NotNull(allLimits.ServerSide);
            Assert.NotNull(allLimits.Android);
            Assert.NotNull(allLimits.IOS);
            Assert.NotNull(allLimits.Web);

            var serverSideLimits = await this._client.GetRateLimits(new GetRateLimitsOptions().WithServerSide());
            Assert.NotNull(serverSideLimits.ServerSide);
            Assert.Null(serverSideLimits.Android);
            Assert.Null(serverSideLimits.IOS);
            Assert.Null(serverSideLimits.Web);

            var options = new GetRateLimitsOptions().
                WithServerSide().
                WithAndroid().
                WithEndpoint("GetRateLimits").
                WithEndpoint("SendMessage");
            var endpointLimits = await this._client.GetRateLimits(options);
            Assert.NotNull(endpointLimits.ServerSide);
            Assert.NotNull(endpointLimits.Android);
            Assert.Null(endpointLimits.IOS);
            Assert.Null(endpointLimits.Web);
            Assert.AreEqual(2, endpointLimits.ServerSide.Count);
            Assert.AreEqual(2, endpointLimits.Android.Count);
            Assert.AreEqual(endpointLimits.Android["GetRateLimits"].Limit, endpointLimits.Android["GetRateLimits"].Remaining);
            Assert.Greater(endpointLimits.ServerSide["GetRateLimits"].Limit, endpointLimits.ServerSide["GetRateLimits"].Remaining);
        }

        [Test]
        public async Task TestUpdateMessage()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Upsert(user1);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());
            await channel.Create(user1.ID, new string[] { user1.ID });

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };
            inMsg.SetData("foo", "barsky");

            var outMsg = await channel.SendMessage(inMsg, user1.ID);

            var newMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString(),
                ID = outMsg.ID,
                User = new User()
                {
                    ID = outMsg.User.ID
                }
            };
            newMsg.SetData("new", "stuff");

            var updatedMessage = await this._client.UpdateMessage(newMsg);
            Assert.AreEqual(outMsg.ID, updatedMessage.ID);
            Assert.AreEqual(newMsg.Text, updatedMessage.Text);
            Assert.IsNull(updatedMessage.GetData<string>("foo"));
            Assert.AreEqual("stuff", updatedMessage.GetData<string>("new"));
        }

        [Test]
        public async Task TestGetMessage()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Upsert(user1);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());
            await channel.Create(user1.ID, new string[] { user1.ID });

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };
            inMsg.SetData("foo", "barsky");

            var outMsg = await channel.SendMessage(inMsg, user1.ID);
            Assert.IsNull(outMsg.DeletedAt);

            var msg = await this._client.GetMessage(outMsg.ID);
            Assert.AreEqual(outMsg.ID, msg.ID);
            Assert.AreEqual(outMsg.CID, msg.CID);
            Assert.AreEqual(outMsg.Text, msg.Text);
        }

        [Test]
        public async Task TestDeleteMessage()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Upsert(user1);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());
            await channel.Create(user1.ID, new string[] { user1.ID });

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString()
            };
            inMsg.SetData("foo", "barsky");

            var outMsg = await channel.SendMessage(inMsg, user1.ID);
            Assert.IsNull(outMsg.DeletedAt);

            var deletedMsg = await this._client.DeleteMessage(outMsg.ID);
            Assert.AreEqual(outMsg.ID, deletedMsg.ID);
            Assert.AreEqual(outMsg.CID, deletedMsg.CID);
            Assert.AreEqual(outMsg.Text, deletedMsg.Text);
            Assert.IsNotNull(deletedMsg.DeletedAt);
        }

        [Test]
        public async Task TestSendSilentMessage()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Upsert(user1);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());
            await channel.Create(user1.ID, new string[] { user1.ID });

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString(),
                Silent = true
            };
            inMsg.SetData("foo", "barsky");

            var outMsg = await channel.SendMessage(inMsg, user1.ID);
            Assert.IsNull(outMsg.DeletedAt);
            Assert.AreEqual(outMsg.Silent, inMsg.Silent);
        }


        [Test]
        public async Task TestFlag()
        {
            // flag user
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };
            var user2 = new User()
            {
                ID = Guid.NewGuid().ToString(),
            };
            await this._client.Users.Upsert(user2);

            await this._client.FlagUser(user2.ID, user1.ID);
            await this._client.UnflagUser(user2.ID, user1.ID);

            // flag message
            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());
            await channel.Create(user1.ID, new string[] { user1.ID });

            var inMsg = new MessageInput()
            {
                Text = Guid.NewGuid().ToString(),
                Silent = true
            };
            inMsg.SetData("foo", "barsky");

            var outMsg = await channel.SendMessage(inMsg, user1.ID);
            await this._client.FlagUser(outMsg.ID, user1.ID);
            await this._client.UnflagUser(outMsg.ID, user1.ID);
        }

        [Test]
        public async Task TestHideShow()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Upsert(user1);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());
            await channel.Create(user1.ID, new string[] { user1.ID });

            await channel.Hide(user1.ID);
            await channel.Show(user1.ID);
        }

        [Test]
        [Ignore("Fails randomly because of app polling")]
        public async Task TestChannelType()
        {
            var inChanType = new ChannelTypeInput()
            {
                Name = Guid.NewGuid().ToString(),
                Automod = Autmod.Disabled,
                Commands = new List<string>() { Commands.Ban },
            };
            var outChanType = await this._client.CreateChannelType(inChanType);

            Assert.AreEqual(inChanType.Name, outChanType.Name);
            Assert.Greater(outChanType.MaxMessageLength, 0);
            Assert.NotZero(outChanType.Permissions.Count);
            Assert.AreEqual(inChanType.Commands, outChanType.Commands);
            Assert.NotNull(outChanType.CreatedAt);

            var getChanType = await this._client.GetChannelType(inChanType.Name);
            Assert.AreEqual(outChanType.Name, getChanType.Name);
            Assert.AreEqual(outChanType.Permissions.Count, getChanType.Permissions.Count);
            Assert.AreEqual(outChanType.CreatedAt, getChanType.CreatedAt);
            Assert.AreEqual(outChanType.Commands, getChanType.Commands.Select(x => x.Name));

            Thread.Sleep(200);

            await this._client.DeleteChannelType(inChanType.Name);
            Assert.ThrowsAsync<StreamChatException>(async () =>
           {
               var c = await this._client.GetChannelType(inChanType.Name);
           });

            var newChanType = new ChannelTypeInput()
            {
                Name = Guid.NewGuid().ToString(),
            };
            await this._client.CreateChannelType(newChanType);

            var chans = await this._client.ListChannelTypes();

            ChannelTypeInfo chanInfo;
            var found = chans.TryGetValue(newChanType.Name, out chanInfo);
            Assert.IsTrue(found);
            Assert.AreEqual(newChanType.Name, chanInfo.Name);

            newChanType.Commands = new List<string>() { Commands.Ban };
            newChanType.Mutes = true;
            newChanType.MaxMessageLength = 123;

            Thread.Sleep(200);

            var updatedChan = await this._client.UpdateChannelType(newChanType.Name, newChanType);

            Assert.AreEqual(123, updatedChan.MaxMessageLength);
            Assert.IsTrue(updatedChan.Mutes);
            Assert.AreEqual(newChanType.Commands, updatedChan.Commands);
        }

        [Test]
        public async Task TestSearchQuery()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var content = Guid.NewGuid().ToString();
            await this._client.Users.Upsert(user1);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());
            await channel.Create(user1.ID, new string[] { user1.ID });

            var inMsg = new MessageInput()
            {
                Text = content
            };

            var outMsg = await channel.SendMessage(inMsg, user1.ID);
            var results = await this._client.Search(SearchOptions.Default.WithQuery(content).WithFilter(new Dictionary<string, object>()
            {
                {"cid", "messaging:"+channel.ID}
            }));

            Assert.AreEqual(outMsg.ID, results.Results[0].Message.ID);
        }

        [Test]
        public async Task TestSearchMessageFilters()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var content = Guid.NewGuid().ToString();
            await this._client.Users.Upsert(user1);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());
            await channel.Create(user1.ID, new string[] { user1.ID });

            var inMsg = new MessageInput()
            {
                Text = content
            };

            var outMsg = await channel.SendMessage(inMsg, user1.ID);
            var results = await this._client.Search(SearchOptions.Default.WithMessageFilterConditions(new Dictionary<string, object>()
            {
                {"text", new Dictionary<string, object>()
                {
                    {"$q", content}
                }}
            }).WithFilter(new Dictionary<string, object>()
            {
                {"cid", "messaging:"+channel.ID}
            }));

            Assert.AreEqual(outMsg.ID, results.Results[0].Message.ID);
        }

        [Test]
        public void TestSearchNoQueryMessageFiltersFails()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await this._client.Search(SearchOptions.Default.WithFilter(new Dictionary<string, object>()
            {
                {"cid", "messaging:123"}
            })));
        }

        [Test]
        public void TestSearchBothQueryMessageFiltersFails()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await this._client.Search(SearchOptions.Default.WithQuery("query").WithMessageFilterConditions(new Dictionary<string, object>()
            {
                {"text", new Dictionary<string, object>()
                {
                    {"$q", "query"}
                }}
            }).WithFilter(new Dictionary<string, object>()
            {
                {"cid", "messaging:123"}
            })));
        }

        [Test]
        public void TestSearchNextOffsetFails()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await this._client.Search(SearchOptions.Default.WithQuery("query").WithNext("next").WithOffset(1).WithFilter(new Dictionary<string, object>()
            {
                {"cid", "messaging:123"}
            })));
        }

        [Test]
        public void TestSearchSortOffsetFails()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await this._client.Search(SearchOptions.Default.WithQuery("query").WithSortBy(new SortParameter
            {
                Field = "created_at",
                Direction = SortDirection.Ascending
            }).WithOffset(1).WithFilter(new Dictionary<string, object>()
            {
                {"cid", "messaging:123"}
            })));
        }

        [Test]
        [Ignore("")]
        public async Task TestSearchSortNext()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            var content = Guid.NewGuid().ToString();
            await this._client.Users.Upsert(user1);

            var channel = this._client.Channel("messaging", Guid.NewGuid().ToString());
            await channel.Create(user1.ID, new string[] { user1.ID });

            var inMsg = new MessageInput()
            {
                Text = content
            };

            var outMsg1 = await channel.SendMessage(inMsg, user1.ID);
            var outMsg2 = await channel.SendMessage(inMsg, user1.ID);
            var results = await this._client.Search(SearchOptions.Default.WithQuery(content).WithLimit(1).WithSortBy(new SortParameter
            {
                Field = "created_at",
                Direction = SortDirection.Ascending
            }).WithFilter(new Dictionary<string, object>()
            {
                {"cid", "messaging:"+channel.ID}
            }));

            Assert.AreEqual(outMsg1.ID, results.Results[0].Message.ID);
            Assert.IsNotEmpty(results.Next);

            var resultsPage2 = await this._client.Search(SearchOptions.Default.WithQuery(content).WithLimit(1).WithNext(results.Next).WithFilter(new Dictionary<string, object>()
            {
                {"cid", "messaging:"+channel.ID}
            }));
            Assert.AreEqual(outMsg2.ID, resultsPage2.Results[0].Message.ID);
            Assert.IsNotEmpty(results.Previous);

        }

        [Test]
        public async Task TestExport()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Upsert(user1);
            var id = Guid.NewGuid().ToString();
            var channel = this._client.Channel("messaging", id);
            await channel.Create(user1.ID, new string[] { user1.ID });
            var taskId = await this._client.ExportChannels(new List<ExportChannelRequest>{
                new ExportChannelRequest().WithChannelId(id).WithChannelType("messaging")
            });
            var complete = false;
            var iterations = 0;
            ExportChannelsStatusResponse resp = null;
            while (!complete && iterations < 1000)
            {
                resp = await this._client.GetExportChannelsStatus(taskId);
                if (resp.Status.Equals("completed"))
                {
                    complete = true;
                }
                iterations++;
                await Task.Delay(100);
            }
            Assert.IsNotNull(resp);
            Assert.IsNotNull(resp.Result.URL);
        }

        [Test]
        public async Task TestDeleteChannels()
        {
            var user1 = new User()
            {
                ID = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await this._client.Users.Upsert(user1);
            var id = Guid.NewGuid().ToString();
            var channel = this._client.Channel("messaging", id);
            await channel.Create(user1.ID, new string[] { user1.ID });

            var cid = "messaging:" + id;
            var taskId = await this._client.DeleteChannels(new string[] { cid }, true);
            var complete = false;
            var iterations = 0;
            StreamChat.TaskStatus resp = null;
            while (!complete && iterations < 1000)
            {
                resp = await this._client.GetTaskStatus(taskId);
                if (resp.Status.Equals("completed"))
                {
                    complete = true;
                }
                iterations++;
                await Task.Delay(100);
            }
            Assert.IsNotNull(resp);
            Assert.IsNotNull(resp.Result);
            var cidStatus = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(resp.Result[cid]));
            Assert.IsNotNull(cidStatus);
            Assert.AreEqual("ok", cidStatus["status"]);
        }
    }
}

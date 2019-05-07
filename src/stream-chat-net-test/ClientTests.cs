using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamChat;

namespace StreamChatTests
{
    [Parallelizable(ParallelScope.None)]
    [TestFixture]
    public class ClientTests
    {
        private StreamChat.Client _client;
        [SetUp]
        public void Setup()
        {
            _client = Credentials.Instance.Client;
        }

        [Test]
        public async Task TestDevices()
        {
            //since testing device endpoints all rely on each other, we have a bit of a
            //catch 23 situation

            //add devices
            var user1 = System.Guid.NewGuid().ToString();
            var user2 = System.Guid.NewGuid().ToString();

            var d1 = new Device()
            {
                ID = System.Guid.NewGuid().ToString(),
                PushProvider = PushProvider.APN,
                UserID = user1
            };
            var d2 = new Device()
            {
                ID = System.Guid.NewGuid().ToString(),
                PushProvider = PushProvider.Firebase,
                UserID = user1
            };
            var d3 = new Device()
            {
                ID = System.Guid.NewGuid().ToString(),
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
        public async Task TestChannelType()
        {
            var inChanType = new ChannelTypeInput()
            {
                Name = System.Guid.NewGuid().ToString(),
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

            await this._client.DeleteChannelType(inChanType.Name);
            Assert.ThrowsAsync<StreamChatException>(async () =>
           {
               var c = await this._client.GetChannelType(inChanType.Name);
           });

            var newChanType = new ChannelTypeInput()
            {
                Name = System.Guid.NewGuid().ToString(),
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

            var updatedChan = await this._client.UpdateChannelType(newChanType.Name, newChanType);

            Assert.AreEqual(123, updatedChan.MaxMessageLength);
            Assert.IsTrue(updatedChan.Mutes);
            Assert.AreEqual(newChanType.Commands, updatedChan.Commands);
        }
    }
}

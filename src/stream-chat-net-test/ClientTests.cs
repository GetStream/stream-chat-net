using System;
using NUnit.Framework;
using System.Threading.Tasks;
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
            _client = new StreamChat.Client("pewr3y7sksbu",
                "yxa2r93542v6bgj63gx5ayfk5buam5v82zudmc8tyt28qn9krmyfhrzx8bfavk6w",
                new ClientOptions()
                {
                    Location = ApiLocation.USEast,
                    Timeout = 10000
                });
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
                PushProvider = Device.APN,
                UserID = user1
            };
            var d2 = new Device()
            {
                ID = System.Guid.NewGuid().ToString(),
                PushProvider = Device.Firebase,
                UserID = user1
            };
            var d3 = new Device()
            {
                ID = System.Guid.NewGuid().ToString(),
                PushProvider = Device.APN,
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
    }
}

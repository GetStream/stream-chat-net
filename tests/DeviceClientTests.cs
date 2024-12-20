using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="DeviceClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// Note: Create and Delete methods are not tested explicitly, because
    /// we use them in the setup and teardown already.
    /// </remarks>
    [TestFixture]
    public class DeviceClientTests : TestBase
    {
        private UserRequest _user;
        private string _deviceId;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            _user = await UpsertNewUserAsync();
            _deviceId = Guid.NewGuid().ToString();
            await _deviceClient.AddDeviceAsync(new Device
            {
                Id = _deviceId,
                PushProvider = "firebase",
                PushProviderName = "FirebaseTestApp",
                UserId = _user.Id,
            });
        }

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
            await _deviceClient.RemoveDeviceAsync(_deviceId, _user.Id);
            await TryDeleteUsersAsync(_user.Id);
        }

        [Test]
        public async Task TestListDevicesAsync()
        {
            var resp = await _deviceClient.GetDevicesAsync(_user.Id);

            resp.Devices.Should().Contain(d => d.Id == _deviceId);
        }
    }
}
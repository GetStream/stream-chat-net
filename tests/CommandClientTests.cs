using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="CommandClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// Note: Create and Delete methods are not tested explicitly, because
    /// we use them in the setup and teardown already.
    /// </remarks>
    [TestFixture]
    public class CommandClientTests : TestBase
    {
        private const string TestCommandDescription = "Test command by .NET test";
        private Command _command;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            var resp = await _commandClient.CreateAsync(new CommandCreateRequest
            {
                Name = Guid.NewGuid().ToString(),
                Description = TestCommandDescription,
            });
            _command = resp.Command;
        }

        [OneTimeTearDown]
        public async Task TeardownAsync()
        {
            await _commandClient.DeleteAsync(_command.Name);
        }

        [Test]
        public Task TestGetCommandAsync()
            => TryMultiple(async () =>
            {
                var command = await _commandClient.GetAsync(_command.Name);

                command.Name.Should().Be(_command.Name);
            });

        [Test]
        public Task TestListCommandsAsync()
            => TryMultiple(async () =>
            {
                var resp = await _commandClient.ListAsync();

                resp.Commands.Should().Contain(c => c.Name == _command.Name);
            });

        [Test]
        public async Task TestUpdateCommandAsync()
        {
            var newDescription = TestCommandDescription + Guid.NewGuid().ToString();
            var resp = await _commandClient.UpdateAsync(_command.Name, new CommandUpdateRequest
            {
                Description = newDescription,
            });

            resp.Command.Description.Should().Be(newDescription);
        }
    }
}
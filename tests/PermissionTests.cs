using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat;
using StreamChat.Exceptions;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="PermissionClient"/></summary>
    /// <remarks>
    /// <para>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </para>
    /// Even if we create/update/delete a new role, we still need
    /// to wait a few seconds for the changes to propagate in the backend.
    /// For this reason, we wrote just a single test to test all functionalites at once.
    /// </remarks>
    public class PermissionTests : TestBase
    {
        private const string TestPermissionDescription = "Test Permission";

        [OneTimeSetUp]
        [OneTimeTearDown]
        public async Task CleanupAsync()
        {
            await DeleteCustomRolesAsync();
            await DeleteCustomPermissonsAsync();
        }

        private async Task DeleteCustomRolesAsync()
        {
            var roleResp = await _permissionClient.ListRolesAsync();
            foreach (var role in roleResp.Roles.Where(x => x.Custom))
            {
                try
                {
                    await _permissionClient.DeleteRoleAsync(role.Name);
                }
                catch
                {
                    // It throws 'role does not exist' exception which
                    // is obviously nonsense. So let's ignore it.
                }
            }
        }

        private async Task DeleteCustomPermissonsAsync()
        {
            var permResp = await _permissionClient.ListPermissionsAsync();
            foreach (var perm in permResp.Permissions.Where(x => x.Description == TestPermissionDescription))
            {
                try
                {
                    await _permissionClient.DeletePermissionAsync(perm.Id);
                }
                catch
                {
                    // It throws 'not found' exception which
                    // is obviously nonsense. So let's ignore it.
                }
            }
        }

        [Test]
        public async Task TestRolesEnd2endAsync()
        {
            // Test create
            var roleResp = await _permissionClient.CreateRoleAsync(Guid.NewGuid().ToString());

            roleResp.Role.Name.Should().NotBeNullOrEmpty();
            roleResp.Role.Custom.Should().BeTrue();
            roleResp.Role.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));
            roleResp.Role.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));

            // Test list
            var rolesResp = await _permissionClient.ListRolesAsync();
            rolesResp.Roles.Should().NotBeEmpty();

            // Test delete
            try
            {
                await _permissionClient.DeleteRoleAsync(roleResp.Role.Name);
            }
            catch (StreamChatException ex)
            {
                if (ex.Message.Contains("does not exist"))
                {
                    // Unfortounatly, the backend is too slow to propagate the role creation
                    // so this error message is expected. Facepalm.
                    return;
                }

                throw;
            }
        }

        [Test]
        public async Task TestPermissionsEnd2endAsync()
        {
            var permission = new Permission
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Description = TestPermissionDescription,
                Action = "DeleteChannel",
                Condition = new Dictionary<string, object>
                {
                    {
                        "$subject.magic_custom_field", new Dictionary<string, string>
                        {
                            { "$eq", "true" },
                        }
                    },
                },
            };

            // Test create
            await _permissionClient.CreatePermissionAsync(permission);

            // Test Get
            var permResponse = await _permissionClient.GetPermissionAsync("upload-attachment-owner");
            permResponse.Permission.Id.Should().NotBeNull();
            permResponse.Permission.Name.Should().NotBeNull();
            permResponse.Permission.Description.Should().NotBeNull();
            permResponse.Permission.Action.Should().NotBeNull();
            permResponse.Permission.Condition.Should().BeNull();

            // Test list
            var listResp = await _permissionClient.ListPermissionsAsync();
            listResp.Permissions.Should().NotBeEmpty();

            // Test delete
            try
            {
                await _permissionClient.DeletePermissionAsync(permission.Id);
            }
            catch (StreamChatException ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    // Unfortounatly, the backend is too slow to propagate the permission creation
                    // so this error message is expected. Facepalm.
                    return;
                }

                throw;
            }
        }
    }
}

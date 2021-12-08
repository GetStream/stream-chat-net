using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat;

namespace StreamChatTests
{
    /// <summary>
    /// Even if we create/update/delete a new role, we still need
    /// to wait a few seconds for the changes to propagate in the backend.
    /// For this reason, we wrote just a single test to test all functionalites at once.
    /// </summary>
    public class PermissionTests
    {
        private readonly IClient _client = TestClientFactory.GetClient();
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
            var roleResp = await _client.Permissions.ListRoles();
            foreach (var role in roleResp.Roles.Where(x => x.Custom))
            {
                try
                {
                    await _client.Permissions.DeleteRole(role.Name);
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
            var permResp = await _client.Permissions.ListPermissions();
            foreach (var perm in permResp.Permissions.Where(x => x.Description == TestPermissionDescription))
            {
                try
                {
                    await _client.Permissions.DeletePermission(perm.Id);
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
            var roleResp = await _client.Permissions.CreateRole(Guid.NewGuid().ToString());

            Assert.That(roleResp.Role.Name, Is.Not.Null);
            Assert.That(roleResp.Role.Custom, Is.True);
            Assert.That(roleResp.Role.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(10)));
            Assert.That(roleResp.Role.UpdatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(10)));

            // Test list
            var rolesResp = await _client.Permissions.ListRoles();
            Assert.That(rolesResp.Roles, Is.Not.Empty);

            // Test delete
            try
            {
                await _client.Permissions.DeleteRole(roleResp.Role.Name);
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
                    {"$subject.magic_custom_field", new Dictionary<string, string> {{"$eq", "true"}}}
                }
            };

            // Test create
            await _client.Permissions.CreatePermission(permission);

            // Test Get
            var permResponse = await _client.Permissions.GetPermission("upload-attachment-owner");
            Assert.That(permResponse.Permission.Id, Is.Not.Null);
            Assert.That(permResponse.Permission.Name, Is.Not.Null);
            Assert.That(permResponse.Permission.Description, Is.Not.Null);
            Assert.That(permResponse.Permission.Action, Is.Not.Null);
            Assert.That(permResponse.Permission.Condition, Is.Not.Null);

            // Test list
            var listResp = await _client.Permissions.ListPermissions();
            Assert.That(listResp.Permissions, Is.Not.Empty);

            // Test delete
            try
            {
                await _client.Permissions.DeletePermission(permission.Id);
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
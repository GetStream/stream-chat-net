using System.Threading.Tasks;

namespace StreamChat
{
    public interface IPermissions
    {
        /// <summary>
        /// Creates a custom role.
        /// </summary>
        /// <param name="name">The name of the role to create.</param>
        /// <returns>The created role.</returns>
        Task<RoleResponse> CreateRole(string name);

        /// <summary>
        /// Returns all roles of an app.
        /// </summary>
        /// <returns>All of the roles of an app.</returns>
        Task<ListRolesResponse> ListRoles();

        /// <summary>
        /// Deletes a permission.
        /// </summary>
        /// <param name="id">The unique id of the permission</param>
        /// <returns>The basic <see cref="ApiResponse"/>.</returns>
        Task<ApiResponse> DeletePermission(string id);

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="name">The name of the role to delete.</param>
        /// <returns>The basic <see cref="ApiResponse"/>.</returns>
        Task<ApiResponse> DeleteRole(string name);

        /// <summary>
        /// Creates a custom permission.
        /// </summary>
        /// <param name="permission">The permission to create.</param>
        /// <returns>The basic <see cref="ApiResponse"/>.</returns>
        Task<ApiResponse> CreatePermission(Permission permission);

        /// <summary>
        /// Gets a permission.
        /// </summary>
        /// <param name="id">Id of the permission.</param>
        /// <returns>The <see cref="Permission"/>.</returns>
        Task<GetPermissionResponse> GetPermission(string id);

        /// <summary>
        /// Returns all permissions of an app.
        /// </summary>
        /// <returns>All of the permissions of an app.</returns>
        Task<ListPermissionsResponse> ListPermissions();

        /// <summary>
        /// Updates a permission.
        /// </summary>
        /// <param name="id">The unique id of the permission</param>
        /// <param name="permission">The updated permission object.</param>
        /// <returns>The basic <see cref="ApiResponse"/>.</returns>
        Task<ApiResponse> UpdatePermission(string id, PermissionBase permission);
    }
}
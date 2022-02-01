using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to retrieve, create and alter permissions and roles.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/chat_permission_policies/?language=csharp</remarks>
    public interface IPermissionClient
    {
        /// <summary>
        /// Creates a custom role.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/chat_permission_policies/?language=csharp</remarks>
        Task<RoleResponse> CreateRoleAsync(string name);

        /// <summary>
        /// Returns all roles of an app.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/chat_permission_policies/?language=csharp</remarks>
        Task<ListRolesResponse> ListRolesAsync();

        /// <summary>
        /// Deletes a permission.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/chat_permission_policies/?language=csharp</remarks>
        Task<ApiResponse> DeletePermissionAsync(string id);

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/chat_permission_policies/?language=csharp</remarks>
        Task<ApiResponse> DeleteRoleAsync(string name);

        /// <summary>
        /// Creates a custom permission.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/chat_permission_policies/?language=csharp</remarks>
        Task<ApiResponse> CreatePermissionAsync(Permission permission);

        /// <summary>
        /// Gets a permission.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/chat_permission_policies/?language=csharp</remarks>
        Task<GetPermissionResponse> GetPermissionAsync(string id);

        /// <summary>
        /// Returns all permissions of an app.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/chat_permission_policies/?language=csharp</remarks>
        Task<ListPermissionsResponse> ListPermissionsAsync();

        /// <summary>
        /// Updates a permission.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/chat_permission_policies/?language=csharp</remarks>
        Task<ApiResponse> UpdatePermissionAsync(string id, PermissionUpdateRequest permission);
    }
}
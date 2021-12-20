using System.Net;
using System.Threading.Tasks;
using StreamChat.Rest;

namespace StreamChat
{
    public class PermissionClient : ServiceBase, IPermissions
    {
        private const string RolesEndpoint = "roles";
        private const string PermissionsEndpoint = "permissions";

        public PermissionClient(Client client) : base(client)
        {
        }

        public async Task<RoleResponse> CreateRole(string name)
            => await ExecuteRequestAsync<RoleResponse>(
                RolesEndpoint,
                HttpMethod.POST,
                HttpStatusCode.Created,
                new { name }
            );

        public async Task<ListRolesResponse> ListRoles()
            => await ExecuteRequestAsync<ListRolesResponse>(
                RolesEndpoint,
                HttpMethod.GET,
                HttpStatusCode.OK
            );

        public async Task<ApiResponse> DeleteRole(string name)
            => await ExecuteRequestAsync<ApiResponse>(
                $"{RolesEndpoint}/{name}",
                HttpMethod.DELETE,
                HttpStatusCode.OK
            );

        public async Task<ApiResponse> CreatePermission(Permission permission)
            => await ExecuteRequestAsync<ApiResponse>(
                PermissionsEndpoint,
                HttpMethod.POST,
                HttpStatusCode.Created,
                permission
            );

        public async Task<GetPermissionResponse> GetPermission(string id)
            => await ExecuteRequestAsync<GetPermissionResponse>(
                $"{PermissionsEndpoint}/{id}",
                HttpMethod.GET,
                HttpStatusCode.OK
            );

        public async Task<ListPermissionsResponse> ListPermissions()
            => await ExecuteRequestAsync<ListPermissionsResponse>(
                PermissionsEndpoint,
                HttpMethod.GET,
                HttpStatusCode.OK
            );

        public async Task<ApiResponse> UpdatePermission(string id, PermissionBase permission)
            => await ExecuteRequestAsync<ApiResponse>(
                $"{PermissionsEndpoint}/{id}",
                HttpMethod.PUT,
                HttpStatusCode.OK,
                permission
            );

        public async Task<ApiResponse> DeletePermission(string id)
            => await ExecuteRequestAsync<ApiResponse>(
                $"{PermissionsEndpoint}/{id}",
                HttpMethod.DELETE,
                HttpStatusCode.OK
            );
    }
}
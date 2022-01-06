using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class PermissionClient : ClientBase, IPermissionClient
    {
        private const string RolesEndpoint = "roles";
        private const string PermissionsEndpoint = "permissions";

        internal PermissionClient(IRestClient client) : base(client)
        {
        }

        public async Task<RoleResponse> CreateRoleAsync(string name)
            => await ExecuteRequestAsync<RoleResponse>(RolesEndpoint,
                HttpMethod.POST,
                HttpStatusCode.Created,
                new { name });

        public async Task<ListRolesResponse> ListRolesAsync()
            => await ExecuteRequestAsync<ListRolesResponse>(RolesEndpoint,
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<ApiResponse> DeleteRoleAsync(string name)
            => await ExecuteRequestAsync<ApiResponse>($"{RolesEndpoint}/{name}",
                HttpMethod.DELETE,
                HttpStatusCode.OK);

        public async Task<ApiResponse> CreatePermissionAsync(Permission permission)
            => await ExecuteRequestAsync<ApiResponse>(PermissionsEndpoint,
                HttpMethod.POST,
                HttpStatusCode.Created,
                permission);

        public async Task<GetPermissionResponse> GetPermissionAsync(string id)
            => await ExecuteRequestAsync<GetPermissionResponse>($"{PermissionsEndpoint}/{id}",
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<ListPermissionsResponse> ListPermissionsAsync()
            => await ExecuteRequestAsync<ListPermissionsResponse>(PermissionsEndpoint,
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<ApiResponse> UpdatePermissionAsync(string id, PermissionUpdateRequest permission)
            => await ExecuteRequestAsync<ApiResponse>($"{PermissionsEndpoint}/{id}",
                HttpMethod.PUT,
                HttpStatusCode.OK,
                permission);

        public async Task<ApiResponse> DeletePermissionAsync(string id)
            => await ExecuteRequestAsync<ApiResponse>($"{PermissionsEndpoint}/{id}",
                HttpMethod.DELETE,
                HttpStatusCode.OK);
    }
}
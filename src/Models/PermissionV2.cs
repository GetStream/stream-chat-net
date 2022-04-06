using System;
using System.Collections.Generic;

namespace StreamChat.Models
{
    /// <summary>
    /// A custom role created by the user.
    /// </summary>
    public class CustomRole
    {
        /// <summary>
        /// Whether the role was created by the user or a built-in one.
        /// </summary>
        public bool Custom { get; set; }

        /// <summary>
        /// The date the role was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// The date the role was last updated.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// The name of the role.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// A response returned from create role API.
    /// </summary>
    public class RoleResponse : ApiResponse
    {
        /// <summary>
        /// The role returned from the backend.
        /// </summary>
        public CustomRole Role { get; set; }
    }

    /// <summary>
    /// The permission returned from get permission API.
    /// </summary>
    public class GetPermissionResponse : ApiResponse
    {
        /// <summary>
        /// The permission returned from the backend.
        /// </summary>
        public Permission Permission { get; set; }
    }

    /// <summary>
    /// The response returned from list roles API.
    /// </summary>
    public class ListRolesResponse : ApiResponse
    {
        /// <summary>
        /// The roles returned from the backend.
        /// </summary>
        public List<CustomRole> Roles { get; set; }
    }

    /// <summary>
    /// The permission object without id.
    /// </summary>
    public class PermissionUpdateRequest
    {
        /// <summary>
        /// The name of the permission.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Action name this permission is for (e.g. SendMessage).
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// MongoDB style condition which decides whether or not the permission is granted.
        /// </summary>
        public Dictionary<string, object> Condition { get; set; }

        /// <summary>
        /// The description of the permission.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether this permission applies to resource owner or not.
        /// </summary>
        public bool? Owner { get; set; }

        /// <summary>
        /// Whether this permission applies to teammates (multi-tenancy mode only).
        /// </summary>
        public bool? SameTeam { get; set; }
    }

    /// <summary>
    /// The permission object.
    /// </summary>
    public class Permission : PermissionUpdateRequest
    {
        /// <summary>
        /// The unique id of the permission.
        /// </summary>
        public string Id { get; set; }
    }

    /// <summary>
    /// The response returned from list permissions API.
    /// </summary>
    public class ListPermissionsResponse : ApiResponse
    {
        /// <summary>
        /// The permissions returned from the backend.
        /// </summary>
        public List<Permission> Permissions { get; set; }
    }
}
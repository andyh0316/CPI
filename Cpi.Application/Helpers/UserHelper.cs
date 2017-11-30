using Cpi.Application.DataModels;
using Cpi.Application.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Linq;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.DataModels.Base;

namespace Cpi.Application.Helpers
{
    public static class UserHelper
    {
        private const string FULL_NAME = "FullName";

        private const string ALL_PERMISSIONS = "AllPermissions";
        private const string ROLE_ID = "RoleId";

        public static void Login(UserDm user)
        {
            // login: encrypted
            FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);

            HttpContext.Current.Session[ROLE_ID] = user.UserRoleId.Value;

            // set the rest of the information in cookie (ex. grantProgramName, userId, etc)
            // No encryption needed since these are not sensitive information
            // No expiration needed, when browser closes or when FormsAuthentication expires the user
            // will need to relog which means these fields will get reset anyways
            // REMEMBER: to clear them when the user logs out

            // store all permissions in session for user (for performance)
            StorePermissions(user);
        }

        public static void StorePermissions(UserDm user)
        {
            List<PermissionDto> permissions = user.UserRole.UserRolePermissions.Select(m => new PermissionDto
            {
                Id = m.Permission.Id,
                Name = m.Permission.Name,
                Create = m.Create,
                Edit = m.Edit,
                Delete = m.Delete
            }).ToList();

            HttpContext.Current.Session[ALL_PERMISSIONS] = permissions;
        }

        public static List<PermissionDto> GetPermissions()
        {
            return (List<PermissionDto>)HttpContext.Current.Session[ALL_PERMISSIONS];
        }

        public static bool CheckPermission(int permissionId, int? actionId = null)
        {
            int roleId = GetRoleId();

            if (roleId == (int)LookUpUserRoleDm.LookUpIds.老子 || roleId == (int)LookUpUserRoleDm.LookUpIds.Admin)
            {
                return true;
            }
            else
            {
                List<PermissionDto> permissions = GetPermissions();
                if (actionId.HasValue)
                {
                    if (permissions.Any(a => a.Id == permissionId &&
                                            (
                                                (actionId == (int)LookUpPermissionDm.ActionIds.Create && a.Create) ||
                                                (actionId == (int)LookUpPermissionDm.ActionIds.Edit && a.Edit) ||
                                                (actionId == (int)LookUpPermissionDm.ActionIds.Delete && a.Delete)
                                            )
                                       )
                       )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (permissions.Any(a => a.Id == permissionId))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public static void CheckPermissionForEntities(IEnumerable<BaseDm> entities, int permissionId)
        {
            bool canCreate = CheckPermission(permissionId, (int)LookUpPermissionDm.ActionIds.Create);
            bool canEdit = CheckPermission(permissionId, (int)LookUpPermissionDm.ActionIds.Edit);
            bool canDelete = CheckPermission(permissionId, (int)LookUpPermissionDm.ActionIds.Delete);

            foreach (BaseDm entity in entities)
            {
                if ((entity.Id == 0 && !canCreate) ||
                    (entity.Id > 0 && !entity.Deleted && !canEdit) ||
                    (entity.Id > 0 && entity.Deleted && !canDelete))
                {
                    throw new Exception("Unauthorized");
                }
            }
        }

        public static void CheckPermissionForEntity(BaseDm entity, int permissionId)
        {
            bool canCreate = CheckPermission(permissionId, (int)LookUpPermissionDm.ActionIds.Create);
            bool canEdit = CheckPermission(permissionId, (int)LookUpPermissionDm.ActionIds.Edit);
            bool canDelete = CheckPermission(permissionId, (int)LookUpPermissionDm.ActionIds.Delete);

            if ((entity.Id == 0 && !canCreate) ||
                (entity.Id > 0 && !entity.Deleted && !canEdit) ||
                (entity.Id > 0 && entity.Deleted && !canDelete))
            {
                throw new Exception("Unauthorized");
            }
        }

        public static int? GetUserId()
        {
            string userId = HttpContext.Current.User.Identity.Name;
            if (!string.IsNullOrEmpty(userId))
            {
                return Convert.ToInt32(HttpContext.Current.User.Identity.Name);
            }
            else
            {
                return null;
            }
        }

        public static int GetRoleId()
        {
            return (int)HttpContext.Current.Session[ROLE_ID];
        }
    }
}

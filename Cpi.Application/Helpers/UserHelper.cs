using Cpi.Application.DataModels;
using System;
using System.Web;
using System.Web.Security;

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
            //StorePermissions(user);

        }

        public static int GetUserId()
        {
            return Convert.ToInt32(HttpContext.Current.User.Identity.Name);
        }
    }
}

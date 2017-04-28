using Cpi.Application.Helpers;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cobro.Compass.Web.Attributes
{
    // Where to use this: controller level. we are only checking if the user is logged in.
    public class CpiAuthenticateAttribute : AuthorizeAttribute
    {
        private int[] RoleIds { get; set; }
        private bool ValidateRole { get; set; }

        public CpiAuthenticateAttribute()
        {
            ValidateRole = false;
        }

        public CpiAuthenticateAttribute(params int[] roleIds)
        {
            RoleIds = roleIds;
            ValidateRole = true;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuthorized = base.AuthorizeCore(httpContext);

            if (isAuthorized && ValidateRole) // if user is authorized and role validation is needed, we check if the role has access to this controller
            {
                int userRoleId = UserHelper.GetRoleId();
                isAuthorized = false;
                foreach (int roleId in RoleIds)
                {
                    if (roleId == userRoleId)
                    {
                        isAuthorized = true;
                        break;
                    }
                }
            }

            return isAuthorized;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var ctx = HttpContext.Current;

            // If the browser session has expired...
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // For AJAX requests, we're overriding the returned JSON result with a simple string,
                // indicating to the calling JavaScript code that a redirect should be performed.
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        IsSessionExpired = true
                    },

                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                // else, in MVC, just redirect
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Controller", "Public"},
                        {"Action", "Index"},
                        {"Area", ""}
                    });
            }
        }
    }
}
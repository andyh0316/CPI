using Cpi.Application.DataModels.LookUp;
using Cpi.Application.DataTransferObjects;
using Cpi.Application.Helpers;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;

namespace Cobro.Compass.Web.Attributes
{
    public class CpiAuthenticateAttribute : AuthorizeAttribute
    {
        public int PermissionId { get; set; }
        public int[] ActionIds { get; set; }

        public CpiAuthenticateAttribute(int permissionId, params int[] actionIds)
        {
            PermissionId = permissionId;
            ActionIds = actionIds;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuthorized = base.AuthorizeCore(httpContext);

            if (isAuthorized)
            {
                if (ActionIds != null && ActionIds.Length > 0)
                {
                    foreach (int actionId in ActionIds)
                    {
                        if (UserHelper.CheckPermission(PermissionId, actionId))
                        {
                            return true;
                        }
                    }

                    return false;
                }
                else
                {
                    return UserHelper.CheckPermission(PermissionId);
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
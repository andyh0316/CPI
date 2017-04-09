using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Routing;

namespace Cobro.Compass.Web.Attributes
{
    // Where to use this: controller level. we are only checking if the user is logged in.
    public class CpiAuthenticateAttribute : AuthorizeAttribute
    {
        public CpiAuthenticateAttribute()
        {
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var ctx = HttpContext.Current;

            // If the browser session has expired...
            if (actionContext.Request.IsAjaxRequest())
            {
                // For AJAX requests, we're overriding the returned JSON result with a simple string,
                // indicating to the calling JavaScript code that a redirect should be performed.
                actionContext.Result = new JsonResult
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
                actionContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Controller", "Public"},
                        {"Action", "Login"},
                        {"Area", ""}
                    });
            }
        }
    }
}
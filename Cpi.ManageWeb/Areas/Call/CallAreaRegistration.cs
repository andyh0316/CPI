using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Call
{
    public class CallAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Call";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Call_default",
                "Call/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

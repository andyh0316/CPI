using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Revenue
{
    public class RevenueAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Revenue";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Revenue_default",
                "Revenue/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

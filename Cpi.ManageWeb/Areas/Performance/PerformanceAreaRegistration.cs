using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Invoice
{
    public class PerformanceAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Performance";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Performance_default",
                "Performance/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

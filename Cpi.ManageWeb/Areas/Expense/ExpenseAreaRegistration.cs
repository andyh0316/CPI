using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Expense
{
    public class ExpenseAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Expense";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Expense_default",
                "Expense/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

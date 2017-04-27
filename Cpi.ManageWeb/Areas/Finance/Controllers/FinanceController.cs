using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.BusinessObjects.Other;
using Cpi.ManageWeb.Controllers.Base;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Finance.Controllers
{
    [CpiAuthenticate]
    public class FinanceController : BaseController
    {
        private InvoiceBo InvoiceBo;
        private FinanceBo FinanceBo;
        public FinanceController(InvoiceBo InvoiceBo, FinanceBo FinanceBo)
        {
            this.InvoiceBo = InvoiceBo;
            this.FinanceBo = FinanceBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetFinance()
        {
            var model = new
            {
                RevenueForToday = FinanceBo.GetRevenueForToday(),
                RevenueForThisMonth = FinanceBo.GetRevenueForThisMonth()
            };

            return JsonModel(model);
        }

        [HttpGet]
        public ContentResult GetFinanceData()
        {
            return null;
        }
    }
}

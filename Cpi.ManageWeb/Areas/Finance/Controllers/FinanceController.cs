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
        private CallBo CallBo;
        private FinanceBo FinanceBo;
        public FinanceController(CallBo CallBo, FinanceBo FinanceBo)
        {
            this.CallBo = CallBo;
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
                //Revenue = FinanceBo.GetRevenueForToday(),
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

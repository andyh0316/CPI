using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects.LookUp;
using Cpi.Application.BusinessObjects.Other;
using Cpi.Application.DataModels.LookUp;
using Cpi.ManageWeb.Controllers.Base;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Invoice.Controllers
{
    [CpiAuthenticate((int)LookUpUserRoleDm.LookUpIds.Admin)]
    public class PerformanceController : BaseController
    {
        private LookUpBo LookUpBo;
        private PerformanceBo PerformanceBo;
        public PerformanceController(LookUpBo LookUpBo, PerformanceBo PerformanceBo)
        {
            this.LookUpBo = LookUpBo;
            this.PerformanceBo = PerformanceBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ContentResult GetPerformance()
        {
            var model = new
            {
                PerformanceForOperators = PerformanceBo.GetPerformanceForOperators(),
                PerformanceForDeliveryStaff = PerformanceBo.GetPerformanceForDeliverStaff()
            };

            return JsonModel(model);
        }

        [HttpGet]
        public ContentResult GetPerformanceData()
        {
            return JsonModel(null);
        }
    }
}

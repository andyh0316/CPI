using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects.LookUp;
using Cpi.Application.BusinessObjects.Other;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.Filters;
using Cpi.Application.Helpers;
using Cpi.Application.Models;
using Cpi.ManageWeb.Controllers.Base;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Report.Controllers
{
    [CpiAuthenticate((int)LookUpUserRoleDm.LookUpIds.老子, (int)LookUpUserRoleDm.LookUpIds.Admin, (int)LookUpUserRoleDm.LookUpIds.DataSpecialist)]
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
        public ContentResult GetPerformance(ReportDateFilter filter)
        {
            if (filter == null)
            {
                filter = new ReportDateFilter
                {
                    ReportDateId = (int)ReportDateFilter.ReportDateIdEnums.Today
                };
            }

            var model = new
            {
                PerformanceForOperators = PerformanceBo.GetPerformanceForOperators(filter),
                PerformanceForDeliveryStaff = PerformanceBo.GetPerformanceForDeliverStaff(filter)
            };

            return JsonModel(model);
        }

        [HttpGet]
        public ContentResult GetPerformanceData()
        {
            var model = new
            {
                ReportDates = ReportDateFilter.GetSelectList(),
                ReportDateIdEnums = EnumHelper.GetEnumIntList(typeof(ReportDateFilter.ReportDateIdEnums))
            };

            return JsonModel(model);
        }
    }
}

using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.BusinessObjects.Other;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.Filters;
using Cpi.Application.Helpers;
using Cpi.Application.Models;
using Cpi.ManageWeb.Controllers.Base;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Finance.Controllers
{
    [CpiAuthenticate((int)LookUpUserRoleDm.LookUpIds.Admin)]
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
        public ContentResult GetFinance(FinanceFilter filter)
        {
            if (filter == null)
            {
                filter = new FinanceFilter
                {
                    ReportDateId = (int)FinanceBo.ReportDates.Today
                };
            }

            var model = new
            {
                Filter = filter,
                Revenue = FinanceBo.GetRevenue(filter),
            };

            return JsonModel(model);
        } 

        [HttpGet]
        public ContentResult GetFinanceData()
        {
            List<CpiSelectListItem> reportDates = new List<CpiSelectListItem>
            {
                new CpiSelectListItem { Id = (int)FinanceBo.ReportDates.Today, Name = "Today 今天" },
                new CpiSelectListItem { Id = (int)FinanceBo.ReportDates.Yesterday, Name = "Yesterday 昨天" },
                new CpiSelectListItem { Id = (int)FinanceBo.ReportDates.ThisMonth, Name = "This Month 本月" },
                new CpiSelectListItem { Id = (int)FinanceBo.ReportDates.Last30Days, Name = "Last 30 Days 近三十天" },
                new CpiSelectListItem { Id = (int)FinanceBo.ReportDates.ThisYear, Name = "This Year 今年" },
                new CpiSelectListItem { Id = (int)FinanceBo.ReportDates.AllTime, Name = "Beginning of Time 所有" },
                new CpiSelectListItem { Id = (int)FinanceBo.ReportDates.SelectDates, Name = "Select Your Dates 自定日期" }
            };

            var model = new
            {
                ReportDates = reportDates,
                ReportDateIdEnums = EnumHelper.GetEnumIntList(typeof(FinanceBo.ReportDates))
            };

            return JsonModel(model);
        }
    }
}

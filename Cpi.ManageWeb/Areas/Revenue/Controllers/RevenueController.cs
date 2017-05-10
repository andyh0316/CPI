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

namespace Cpi.ManageWeb.Areas.Revenue.Controllers
{
    [CpiAuthenticate((int)LookUpUserRoleDm.LookUpIds.Admin)]
    public class RevenueController : BaseController
    {
        private InvoiceBo InvoiceBo;
        private RevenueBo RevenueBo;
        public RevenueController(InvoiceBo InvoiceBo, RevenueBo RevenueBo)
        {
            this.InvoiceBo = InvoiceBo;
            this.RevenueBo = RevenueBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetRevenue(ReportDateFilter filter)
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
                Filter = filter,
                Revenue = RevenueBo.GetRevenue(filter),
                ProductSoldCount = RevenueBo.GetProductSoldCount(filter),
                ProductCancelledCount = RevenueBo.GetProductCancelledCount(filter),
                InvoiceSoldCount = RevenueBo.GetInvoiceSoldCount(filter),
                InvoiceCancelledCount = RevenueBo.GetInvoiceCancelledCount(filter),
                ReceivedCallCount = RevenueBo.GetReceivedCallCount(filter),
                Revenues = RevenueBo.GetRevenues(filter),
                Calls = RevenueBo.GetCalls(filter)
            };

            return JsonModel(model);
        } 

        [HttpGet]
        public ContentResult GetRevenueData()
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

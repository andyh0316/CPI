using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.BusinessObjects.LookUp;
using Cpi.Application.BusinessObjects.Other;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.DataTransferObjects;
using Cpi.Application.Filters;
using Cpi.Application.Helpers;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Report.Controllers
{
    [CpiAuthenticate((int)LookUpUserRoleDm.LookUpIds.老子, (int)LookUpUserRoleDm.LookUpIds.Admin)]
    public class FinanceOverviewController : BaseController
    {
        private InvoiceBo InvoiceBo;
        private FinanceBo FinanceBo;
        private LookUpBo LookUpBo;
        public FinanceOverviewController(InvoiceBo InvoiceBo, FinanceBo FinanceBo, LookUpBo LookUpBo)
        {
            this.InvoiceBo = InvoiceBo;
            this.FinanceBo = FinanceBo;
            this.LookUpBo = LookUpBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetFinanceOverview(ClassFilter.Finance filter)
        {
            var model = new
            {
                Filter = filter,
                Revenue = FinanceBo.GetRevenue(filter),
                Expense = FinanceBo.GetExpense(filter),
                Profit = FinanceBo.GetRevenue(filter) - FinanceBo.GetExpense(filter),
                ProductSoldCount = FinanceBo.GetProductSoldCount(filter),
                ProductCancelledCount = FinanceBo.GetProductCancelledCount(filter),
                ProductPendingCount = FinanceBo.GetProductPendingCount(filter),
                ProductTotalCount = FinanceBo.GetProductTotalCount(filter),
                InvoiceSoldCount = FinanceBo.GetInvoiceSoldCount(filter),
                InvoiceCancelledCount = FinanceBo.GetInvoiceCancelledCount(filter),
                InvoicePendingCount = FinanceBo.GetInvoicePendingCount(filter),
                InvoiceTotalCount = FinanceBo.GetInvoiceTotalCount(filter),
                ReceivedCallCount = FinanceBo.GetReceivedCallCount(filter),
                Revenues = FinanceBo.GetRevenues(filter),
                Calls = FinanceBo.GetCalls(filter),
                Products = FinanceBo.GetProducts(filter),
            };

            return JsonModel(model);
        } 

        [HttpGet]
        public ContentResult GetFinanceOverviewData()
        {
            var model = new
            {
                Locations = LookUpBo.GetList<LookUpLocationDm>(),
                ReportDates = ReportDateFilter.GetSelectList(),
                ReportDateIdEnums = EnumHelper.GetEnumIntList(typeof(ReportDateFilter.ReportDateIdEnums)),
                //CanSeeMoney = (UserHelper.GetRoleId() == (int)LookUpUserRoleDm.LookUpIds.老子) ? true : false
            };

            return JsonModel(model);
        }
    }
}

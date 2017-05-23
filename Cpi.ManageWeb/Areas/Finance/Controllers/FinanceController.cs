using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.BusinessObjects.Other;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.DataTransferObjects;
using Cpi.Application.Filters;
using Cpi.Application.Helpers;
using Cpi.Application.Models;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System.Collections.Generic;
using System.Linq;
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
        public ContentResult GetFinance(ReportDateFilter filter)
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

        [HttpPost]
        public ContentResult GetFinanceList(ListFilter.Finance filter)
        {
            IQueryable<FinanceDto> query = FinanceBo.GetListBaseQuery();
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
            List<FinanceDto> records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortColumn, filter.SortDesc).ToList();
            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        }

        [HttpGet]
        public ContentResult GetFinanceData()
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

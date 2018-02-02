using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.BusinessObjects.Other;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.DataTransferObjects;
using Cpi.Application.Filters;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Report.Controllers
{
    [CpiAuthenticate((int)LookUpPermissionDm.LookUpIds.FinanceList)]
    public class FinanceListController : BaseController
    {
        private FinanceBo FinanceBo;
        private InvoiceBo InvoiceBo;
        private ExpenseBo ExpenseBo;
        public FinanceListController(FinanceBo FinanceBo, InvoiceBo InvoiceBo, ExpenseBo ExpenseBo)
        {
            this.FinanceBo = FinanceBo;
            this.InvoiceBo = InvoiceBo;
            this.ExpenseBo = ExpenseBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetFinanceList(ListFilter.Finance filter)
        {
            IQueryable<FinanceDto> query = FinanceBo.GetListBaseQuery();
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter, query.Count());
            List<FinanceDto> listItems = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortObjects).ToList();
            return JsonModel(new { ListItems = listItems, ListLoadCalculator = listLoadCalculator });
        }

        [HttpGet]
        public ContentResult GetFinance(DateTime? date)
        {
            List<InvoiceSummaryDto> invoiceSummaries = InvoiceBo.GetDailyInvoiceSummary(date.Value);
            List<ExpenseDm> expenses = ExpenseBo.GetQueryByDate(date.Value).ToList();

            var model = new
            {
                Date = date.Value,
                InvoiceSummaries = invoiceSummaries,
                Expenses = expenses.OrderBy(a => a.Location.DisplayOrder).ThenByDescending(a => a.Expense),
            };

            return JsonModel(model);
        }
    }
}

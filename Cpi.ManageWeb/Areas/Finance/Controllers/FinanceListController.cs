using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects.Other;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.DataTransferObjects;
using Cpi.Application.Filters;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Finance.Controllers
{
    [CpiAuthenticate((int)LookUpUserRoleDm.LookUpIds.老子, (int)LookUpUserRoleDm.LookUpIds.Admin)]
    public class FinanceListController : BaseController
    {
        private FinanceBo FinanceBo;
        public FinanceListController(FinanceBo FinanceBo)
        {
            this.FinanceBo = FinanceBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetFinanceList(ListFilter.Finance filter)
        {
            IQueryable<FinanceDto> query = FinanceBo.GetListBaseQuery();
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
            List<FinanceDto> records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortColumn, filter.SortDesc).ToList();
            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        }
    }
}

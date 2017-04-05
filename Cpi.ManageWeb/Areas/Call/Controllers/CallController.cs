using Cpi.Application.BusinessObjects;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Call.Controllers
{
    public class CallController : BaseController
    {
        private CallBo CallBo;
        public CallController(CallBo callBo)
        {
            CallBo = callBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(ListFilter.Call filter)
        {
            IQueryable<CallDm> query = CallBo.GetListBaseQuery();
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
            List<CallDm> records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortColumn, filter.SortDesc).ToList();
            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        } 

        //[HttpGet]
        //public ContentResult GetImport()
        //{

        //}

        
    }
}

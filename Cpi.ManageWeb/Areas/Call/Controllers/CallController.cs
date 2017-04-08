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
            IQueryable<CallDm> query = CallBo.GetListBaseQuery(filter);
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
            var records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortColumn, filter.SortDesc).Select(a => new
            {
                Id = a.Id,
                CustomerName = a.CustomerName,
                CustomerPhone = a.CustomerPhone,
                Date = a.Date,
                DeliveryDate = a.DeliveryDate,
                OperatorId = a.OperatorId,
                OperatorName = a.Operator.Name,
                DeliveryStaffId = a.DeliveryStaffId,
                DeliveryStaffName = a.DeliveryStaff.Name
            }).ToList();

            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        }

        [HttpPost]
        public ContentResult SaveList(List<CallDm> calls)
        {
            return null;
        }

        //[HttpGet]
        //public ContentResult GetImport()
        //{

        //}


    }
}

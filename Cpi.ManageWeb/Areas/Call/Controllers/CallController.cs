using AutoMapper;
using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using Cpi.Application.BusinessObjects.LookUp;
using Cpi.Application.DataModels.LookUp;

namespace Cpi.ManageWeb.Areas.Call.Controllers
{
    [CpiAuthenticate]
    public class CallController : BaseController
    {
        private CallBo CallBo;
        private CommodityBo CommodityBo;
        private CallCommodityBo CallCommodityBo;
        private LookUpBo LookUpBo;
        public CallController(CallBo callBo, CommodityBo commodityBo, CallCommodityBo callCommodityBo, LookUpBo lookUpBo)
        {
            CallBo = callBo;
            CommodityBo = commodityBo;
            CallCommodityBo = callCommodityBo;
            LookUpBo = lookUpBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(ListFilter.Call filter)
        {
            IQueryable<CallDm> query = CallBo.GetListBaseQuery(filter).Include(a => a.DeliveryStaff).Include(a => a.Operator).Include(a => a.Status).Include(a => a.CallCommodities.Select(b => b.Commodity));
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
            List<CallDm> records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortColumn, filter.SortDesc).ToList();
            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        }

        [HttpGet]
        public ContentResult GetListData()
        {
            var model = new
            {
                Commodities = CommodityBo.GetList(),
                CallStatuses = LookUpBo.GetList<LookUpCallStatusDm>().ToList()
            };

            return JsonModel(model);
        }

        [HttpPost]
        public ContentResult SaveList(List<CallDm> calls)
        {
            if (!ModelState.IsValid)
            {
                return JsonModelState(ModelState);
            }

            List<CallDm> trackedCalls = CallBo.GetListByIds(calls.Where(a => a.Id > 0).Select(a => a.Id).ToList(), true).ToList();
            foreach (CallDm call in calls)
            {
                CallDm trackedCall = (call.Id > 0) ? trackedCalls.Find(a => a.Id == call.Id) : new CallDm();
                trackedCall.Address = (trackedCall.Address) ?? new AddressDm();

                Mapper.Map(call, trackedCall);
                Mapper.Map(call.Address, trackedCall.Address);

                call.CallCommodities = (call.CallCommodities) ?? new List<CallCommodityDm>();
                trackedCall.CallCommodities = (trackedCall.CallCommodities) ?? new List<CallCommodityDm>();
                CallCommodityBo.RemoveRange(trackedCall.CallCommodities.Where(a => !call.CallCommodities.Select(b => b.Id).Contains(a.Id)).ToList()); // first delete all the call commodities that are not in the view model
                foreach (CallCommodityDm callCommodity in call.CallCommodities)
                {
                    CallCommodityDm trackedCallCommodity = (callCommodity.Id > 0 ) ? trackedCall.CallCommodities.Find(a => a.Id == callCommodity.Id) : new CallCommodityDm();

                    Mapper.Map(callCommodity, trackedCallCommodity);

                    if (trackedCallCommodity.Id > 0)
                    {
                        trackedCallCommodity.ModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        trackedCall.CallCommodities.Add(trackedCallCommodity);
                        trackedCallCommodity.CreatedDate = DateTime.Now;
                    }
                }

                if (trackedCall.Id > 0)
                {
                    trackedCall.ModifiedDate = DateTime.Now;
                }
                else
                {
                    trackedCall.CreatedDate = DateTime.Now;
                    CallBo.Add(trackedCall);
                }
            }

            CallBo.Commit();

            return JsonModel(null);
        }

        //[HttpGet]
        //public ContentResult GetImport()
        //{

        //}


    }
}

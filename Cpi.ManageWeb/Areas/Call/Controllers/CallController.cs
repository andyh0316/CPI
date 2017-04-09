using AutoMapper;
using Cpi.Application.BusinessObjects;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System;
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
                CreatedDate = a.CreatedDate,
                DeliveryDate = a.DeliveryDate,
                OperatorId = a.OperatorId,
                OperatorName = a.Operator.Name,
                DeliveryStaffId = a.DeliveryStaffId,
                DeliveryStaffName = a.DeliveryStaff.Name,
                AddressId = a.AddressId,
                Address = new {
                    
                }
            }).ToList();

            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        }

        [HttpPost]
        public ContentResult SaveList(List<CallDm> calls)
        {
            if (!ModelState.IsValid)
            {
                return JsonModelState(ModelState);
            }

            List<CallDm> trackedCalls = CallBo.GetList();
            foreach (CallDm call in calls)
            {
                if (call.Id == 0)
                {
                    call.Address = (call.Address) ?? new AddressDm();
                    call.CreatedDate = DateTime.Now;
                    CallBo.Add(call);
                }
                else
                {
                    CallDm trackedCall = trackedCalls.Find(a => a.Id == call.Id);
                    Mapper.Map(call, trackedCall);
                    Mapper.Map(call.Address, trackedCall.Address);
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

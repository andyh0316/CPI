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
using System.Web;
using System.IO;
using Excel;
using System.Data;

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
                CallStatuses = LookUpBo.GetList<LookUpCallStatusDm>().ToList(),
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
            List<CommodityDm> allCommodities = CommodityBo.GetList();

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
                    CallCommodityDm trackedCallCommodity = (callCommodity.Id > 0) ? trackedCall.CallCommodities.Find(a => a.Id == callCommodity.Id) : new CallCommodityDm();

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

                // calculate the total price
                trackedCall.TotalPrice = 0;
                foreach (CallCommodityDm callCommodity in trackedCall.CallCommodities)
                {
                    decimal? commodityPrice = allCommodities.Find(a => a.Id == callCommodity.CommodityId).Price;
                    trackedCall.TotalPrice = trackedCall.TotalPrice + (commodityPrice * callCommodity.Quantity);
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

        [HttpPost]
        public ContentResult OrganizePhoneNumbers(string phoneNumbers)
        {
            List<string> phoneNumbersList = ParsePhoneNumbersMethod(phoneNumbers);

            List<string> smartPrefixes = new List<string> { "10", "15", "16", "69", "70", "81", "86", "87", "93", "98", "96" };
            List<string> metFonePrefixes = new List<string> { "88", "97", "71", "60", "66", "67", "68", "90", "31", "91" };
            List<string> cellCardPrefixes = new List<string> { "11", "12", "14", "17", "61", "76", "77", "78", "85", "89", "92", "95", "99" };

            List<string> smartPhoneNumbers = new List<string>();
            List<string> metFoneNumbers = new List<string>();
            List<string> cellCardPhoneNumbers = new List<string>();
            List<string> otherPhoneNumbers = new List<string>();

            foreach (string phoneNumber in phoneNumbersList)
            {
                if (smartPrefixes.Any(a => phoneNumber.StartsWith(a)))
                {
                    smartPhoneNumbers.Add(phoneNumber);
                }
                else if (metFonePrefixes.Any(a => phoneNumber.StartsWith(a)))
                {
                    metFoneNumbers.Add(phoneNumber);
                }
                else if (cellCardPrefixes.Any(a => phoneNumber.StartsWith(a)))
                {
                    cellCardPhoneNumbers.Add(phoneNumber);
                }
                else
                {
                    otherPhoneNumbers.Add(phoneNumber);
                }
            }

            var model = new
            {
                SmartPhoneNumbers = smartPhoneNumbers,
                MetFoneNumbers = metFoneNumbers,
                CellCardPhoneNumbers = cellCardPhoneNumbers,
                OtherPhoneNumbers = otherPhoneNumbers
            };

            return JsonModel(model);
        }

        [HttpPost]
        public ContentResult ParsePhoneNumbers(string phoneNumbers)
        {
            var model = new
            {
                PhoneNumbers = ParsePhoneNumbersMethod(phoneNumbers)
            };

            return JsonModel(model);
        }

        private List<string> ParsePhoneNumbersMethod(string phoneNumbers)
        {
            List<string> phoneNumbersList = new List<string>();
            var reader = new StringReader(phoneNumbers);
            string line;
            while (null != (line = reader.ReadLine()))
            {
                phoneNumbersList.Add(line);
            }

            phoneNumbersList = phoneNumbersList.Distinct().ToList();

            return phoneNumbersList;
        }
    }
}

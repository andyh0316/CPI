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
using System.IO;
using System.Data;
using Cpi.Application.Models;
using Cpi.Application.Helpers;

namespace Cpi.ManageWeb.Areas.Call.Controllers
{
    [CpiAuthenticate((int)LookUpPermissionDm.LookUpIds.Call)]
    public class CallController : BaseController
    {
        private CallBo CallBo;
        private CallCommodityBo CallCommodityBo;
        private LookUpBo LookUpBo;
        private UserBo UserBo;
        private CommodityBo CommodityBo;
        public CallController(CallBo CallBo, CallCommodityBo CallCommodityBo, LookUpBo LookUpBo, UserBo UserBo, CommodityBo CommodityBo)
        {
            this.CallBo = CallBo;
            this.CallCommodityBo = CallCommodityBo;
            this.LookUpBo = LookUpBo;
            this.UserBo = UserBo; 
            this.CommodityBo = CommodityBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(ListFilter.Call filter)
        {
            IQueryable<CallDm> query = CallBo.GetListBaseQuery(filter).Include(a => a.Status);
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter, query.Count());
            List<CallDm> listItems = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortObjects).ToList();
            return JsonModel(new { ListItems = listItems, ListLoadCalculator = listLoadCalculator });
        }

        [HttpGet]
        public ContentResult GetListData()
        {
            var model = new
            {
                Commodities = CommodityBo.GetList(),
                CallStatuses = LookUpBo.GetList<LookUpCallStatusDm>().ToList(),
                Users = UserBo.GetSearchDropDownList(),
                ReportDates = ReportDateFilter.GetSelectList(),
                CallStatusIdEnums = EnumHelper.GetEnumIntList(typeof(LookUpCallStatusDm.LookUpIds)),
                ReportDateIdEnums = EnumHelper.GetEnumIntList(typeof(ReportDateFilter.ReportDateIdEnums)),
                TodayDate = new DateTime(DateTime.Now.Ticks), 
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

            UserHelper.CheckPermissionForEntities(calls, (int)LookUpPermissionDm.LookUpIds.Call);

            List<CallDm> trackedCalls = CallBo.GetListByIds(calls.Where(a => a.Id > 0).Select(a => a.Id).ToList(), true).ToList();
            List<CommodityDm> allCommodities = CommodityBo.GetList();

            foreach (CallDm call in calls)
            {
                CallDm trackedCall = (call.Id > 0) ? trackedCalls.Find(a => a.Id == call.Id) : new CallDm();

                Mapper.Map(call, trackedCall);

                //call.CallCommodities = (call.CallCommodities) ?? new List<CallCommodityDm>();
                //trackedCall.CallCommodities = (trackedCall.CallCommodities) ?? new List<CallCommodityDm>();

                // save each callCommodity
                //foreach (CallCommodityDm callCommodity in call.CallCommodities)
                //{
                //    CallCommodityDm trackedCallCommodity = (callCommodity.Id > 0) ? trackedCall.CallCommodities.Find(a => a.Id == callCommodity.Id) : new CallCommodityDm();

                //    Mapper.Map(callCommodity, trackedCallCommodity);

                //    if (trackedCallCommodity.Id > 0)
                //    {
                //        if (trackedCallCommodity.Quantity == 0) // deleting
                //        {
                //            CallCommodityBo.Remove(trackedCallCommodity);
                //        }
                //    }
                //    else
                //    {
                //        trackedCall.CallCommodities.Add(trackedCallCommodity);
                //    }
                //}

                if (trackedCall.Id == 0)
                {
                    // once theres commodities enterable from the website to here: we will need to check if thers already commodity, if so add new commodities to existing
                    if (!CallBo.CallWithPhoneExistsToday(trackedCall.CustomerPhone))
                    {
                        CallBo.Add(trackedCall);
                    }
                }
            }

            CallBo.Commit();

            return JsonModel(null);
        }

        [HttpPost]
        public ContentResult OrganizePhoneNumbers(string phoneNumbers)
        {
            List<string> phoneNumbersList = ParsePhoneNumbersMethod(phoneNumbers);

            List<string> smartPrefixes = CommonHelper.SmartPrefixes;
            List<string> metFonePrefixes = CommonHelper.MetFonePrefixes;
            List<string> cellCardPrefixes = CommonHelper.CellCardPrefixes;

            List<string> smartPhoneNumbers = new List<string>();
            List<string> metFonePhoneNumbers = new List<string>();
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
                    metFonePhoneNumbers.Add(phoneNumber);
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
                MetFonePhoneNumbers = metFonePhoneNumbers,
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
                phoneNumbersList.Add(line.Trim());
            }

            phoneNumbersList = phoneNumbersList.Distinct().ToList();

            return phoneNumbersList;
        }
    }
}

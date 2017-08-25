using AutoMapper;
using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using Cpi.Application.BusinessObjects.LookUp;
using Cpi.Application.DataModels.LookUp;
using System.Data;
using Cpi.Application.Models;
using Cpi.Application.Helpers;
using System;

namespace Cpi.ManageWeb.Areas.Invoice.Controllers
{
    [CpiAuthenticate((int)LookUpUserRoleDm.LookUpIds.老子, (int)LookUpUserRoleDm.LookUpIds.Admin)]
    public class InvoiceController : BaseController
    {
        private InvoiceBo InvoiceBo;
        private InvoiceCommodityBo InvoiceCommodityBo;
        private LookUpBo LookUpBo;
        private UserBo UserBo;
        public InvoiceController(InvoiceBo InvoiceBo, InvoiceCommodityBo InvoiceCommodityBo, LookUpBo LookUpBo, UserBo UserBo)
        {
            this.InvoiceBo = InvoiceBo;
            this.InvoiceCommodityBo = InvoiceCommodityBo;
            this.LookUpBo = LookUpBo;
            this.UserBo = UserBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(ListFilter.Invoice filter)
        {
            IQueryable<InvoiceDm> query = InvoiceBo.GetListBaseQuery(filter).Include(a => a.DeliveryStaff).Include(a => a.Operator).Include(a => a.Status).Include(a => a.Location).Include(a => a.InvoiceCommodities.Select(b => b.Commodity));
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
            List<InvoiceDm> records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortObjects).ToList();
            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        }

        [HttpGet]
        public ContentResult GetListData()
        {
            var model = new
            {
                Commodities = LookUpBo.GetList<LookUpCommodityDm>().ToList(),
                InvoiceStatuses = LookUpBo.GetList<LookUpInvoiceStatusDm>().ToList(),
                Locations = LookUpBo.GetList<LookUpLocationDm>().ToList(),
                Users = UserBo.GetListQuery().OrderBy(a => a.Nickname).ThenBy(a => a.Fullname).Select(a => new CpiSelectListItem
                {
                    Id = a.Id,
                    Name = a.Nickname + " (" + a.Fullname + ")"
                }).ToList(),
                ReportDates = ReportDateFilter.GetSelectList(),
                InvoiceStatusIdEnums = EnumHelper.GetEnumIntList(typeof(LookUpInvoiceStatusDm.LookUpIds)),
                ReportDateIdEnums = EnumHelper.GetEnumIntList(typeof(ReportDateFilter.ReportDateIdEnums)),
            };

            return JsonModel(model);
        }

        [HttpPost]
        public ContentResult SaveList(List<InvoiceDm> Invoices)
        {
            if (!ModelState.IsValid)
            {
                return JsonModelState(ModelState);
            }

            List<InvoiceDm> trackedInvoices = InvoiceBo.GetListByIds(Invoices.Where(a => a.Id > 0).Select(a => a.Id).ToList(), true).ToList();
            List<LookUpCommodityDm> allCommodities = LookUpBo.GetList<LookUpCommodityDm>();

            foreach (InvoiceDm invoice in Invoices)
            {
                InvoiceDm trackedInvoice = (invoice.Id > 0) ? trackedInvoices.Find(a => a.Id == invoice.Id) : new InvoiceDm();

                bool isUpdatingStatusWithValue = (invoice.StatusId.HasValue && invoice.StatusId != trackedInvoice.StatusId);

                Mapper.Map(invoice, trackedInvoice);

                invoice.InvoiceCommodities = (invoice.InvoiceCommodities) ?? new List<InvoiceCommodityDm>();
                trackedInvoice.InvoiceCommodities = (trackedInvoice.InvoiceCommodities) ?? new List<InvoiceCommodityDm>();

                //InvoiceCommodityBo.RemoveRange(trackedInvoice.InvoiceCommodities.Where(a => !Invoice.InvoiceCommodities.Select(b => b.Id).Contains(a.Id)).ToList()); // first delete all the Invoice commodities that are not in the view model

                // save each InvoiceCommodity
                foreach (InvoiceCommodityDm InvoiceCommodity in invoice.InvoiceCommodities)
                {
                    InvoiceCommodityDm trackedInvoiceCommodity = (InvoiceCommodity.Id > 0) ? trackedInvoice.InvoiceCommodities.Find(a => a.Id == InvoiceCommodity.Id) : new InvoiceCommodityDm();

                    Mapper.Map(InvoiceCommodity, trackedInvoiceCommodity);

                    if (trackedInvoiceCommodity.Id > 0)
                    {
                        if (trackedInvoiceCommodity.Quantity == 0) // deleting
                        {
                            InvoiceCommodityBo.Remove(trackedInvoiceCommodity);
                        }
                    }
                    else
                    {
                        trackedInvoice.InvoiceCommodities.Add(trackedInvoiceCommodity);
                    }
                }

                // calculate the total price
                //if (trackedInvoice.InvoiceCommodities != null && trackedInvoice.InvoiceCommodities.Count > 0)
                //{
                //    trackedInvoice.TotalPrice = 0;
                //    foreach (InvoiceCommodityDm invoiceCommodity in trackedInvoice.InvoiceCommodities)
                //    {
                //        decimal? commodityPrice = allCommodities.Find(a => a.Id == invoiceCommodity.CommodityId).Price;
                //        trackedInvoice.TotalPrice = trackedInvoice.TotalPrice + (commodityPrice * invoiceCommodity.Quantity);
                //    }
                //}

                if (trackedInvoice.Id > 0)
                {
                    // if the invoice's status is set to something, and the invoice's created date was yesterday or earlier, then we make
                    // the invoice's createdDate to now. The reason for this is sometimes they entered invoices from yesterday night, but the
                    // deliverers take the product and go home and keeps the product to deliver today, in that case, we need to make the invoice
                    // today to reflect proper finance analysis
                    if (isUpdatingStatusWithValue && DateTime.Now.Date > trackedInvoice.CreatedDate.Value.Date)
                    {
                        trackedInvoice.CreatedDate = DateTime.Now;
                    }
                }
                else
                {
                    InvoiceBo.Add(trackedInvoice);
                }
            }

            InvoiceBo.Commit();

            return JsonModel(null);
        }
    }
}

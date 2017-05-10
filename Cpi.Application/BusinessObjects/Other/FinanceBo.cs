using Cpi.Application.BusinessObjects.LookUp;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cpi.Application.BusinessObjects.Other
{
    public class FinanceBo
    {
        private InvoiceBo InvoiceBo;
        private CallBo CallBo;
        private LookUpBo LookUpBo;
        public FinanceBo(InvoiceBo InvoiceBo, CallBo CallBo, LookUpBo LookUpBo)
        {
            this.InvoiceBo = InvoiceBo;
            this.CallBo = CallBo;
            this.LookUpBo = LookUpBo;
        }

        public decimal GetRevenue(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            invoiceQuery = invoiceQuery.Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            decimal revenue = invoiceQuery.Select(a => a.TotalPrice.Value).DefaultIfEmpty(0).Sum();

            return revenue;
        }

        public int GetProductSoldCount(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            invoiceQuery = invoiceQuery.Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            return invoiceQuery.SelectMany(a => a.InvoiceCommodities.Select(b => b.Quantity.Value)).DefaultIfEmpty(0).Sum();
        }

        public int GetProductCancelledCount(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            invoiceQuery = invoiceQuery.Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Cancelled);

            return invoiceQuery.SelectMany(a => a.InvoiceCommodities.Select(b => b.Quantity.Value)).DefaultIfEmpty(0).Sum();
        }

        public int GetProductPendingCount(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            invoiceQuery = invoiceQuery.Where(a => !a.StatusId.HasValue);

            return invoiceQuery.SelectMany(a => a.InvoiceCommodities.Select(b => b.Quantity.Value)).DefaultIfEmpty(0).Sum();
        }

        public int GetProductTotalCount(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            return invoiceQuery.SelectMany(a => a.InvoiceCommodities.Select(b => b.Quantity.Value)).DefaultIfEmpty(0).Sum();
        }

        public int GetInvoiceSoldCount(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            invoiceQuery = invoiceQuery.Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            return invoiceQuery.Count();
        }

        public int GetInvoiceCancelledCount(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            invoiceQuery = invoiceQuery.Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Cancelled);

            return invoiceQuery.Count();
        }

        public int GetInvoicePendingCount(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            invoiceQuery = invoiceQuery.Where(a => !a.StatusId.HasValue);

            return invoiceQuery.Count();
        }

        public int GetInvoiceTotalCount(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            return invoiceQuery.Count();
        }

        public int GetReceivedCallCount(ReportDateFilter filter)
        {
            IQueryable<CallDm> callQuery = CallBo.GetDateFilteredQuery(filter);

            return callQuery.Count();
        }

        public List<Tuple<string, decimal>> GetRevenues(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetListQuery().Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            List<Tuple<string, decimal>> finances = new List<Tuple<string, decimal>>();

            if (filter.ReportDateId.HasValue)
            {
                DateTime dateFrom = DateTime.Now;
                DateTime dateTo = DateTime.Now;
                bool splitByMonth = false;

                if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past7Days)
                {
                    dateFrom = dateFrom.Date.AddDays(-6);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past30Days)
                {
                    dateFrom = dateFrom.Date.AddDays(-29);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.PastYear)
                {
                    DateTime dateLastYear = DateTime.Now.Date.AddYears(-1).AddMonths(1);
                    dateFrom = new DateTime(dateLastYear.Year, dateLastYear.Month, 1);
                    splitByMonth = true;
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.AllTimeOrSelectDateRange)
                {
                    InvoiceDm earliestInvoice = invoiceQuery.Where(a => a.CreatedDate.HasValue).OrderBy(a => a.CreatedDate.Value).FirstOrDefault();

                    if (earliestInvoice == null) // if theres no invoices with created date
                    {
                        return null;
                    }

                    DateTime earliestInvoiceDate = earliestInvoice.CreatedDate.Value; // this is guaranteed to have value because of the query and check above
                    dateFrom = earliestInvoiceDate;

                    if (filter.DateFrom.HasValue)
                    {
                        if (filter.DateFrom.Value > dateFrom)
                        {
                            dateFrom = filter.DateFrom.Value;
                        }
                    }

                    if (filter.DateTo.HasValue)
                    {
                        if (filter.DateTo.Value < dateTo)
                        {
                            dateTo = filter.DateTo.Value;
                        }
                    }

                    // if dateTo is more than a month away from dateFrom: do more things
                    if (dateTo.AddMonths(-1) > dateFrom)
                    {
                        splitByMonth = true;
                    }
                }
                else
                {
                    return null;
                }

                while (dateFrom <= dateTo)
                {
                    DateTime dateAfter = (splitByMonth) ? dateFrom.AddMonths(1) : dateFrom.AddDays(1);
                    string dateDisplayFormat = (splitByMonth) ? "MM.yyyy" : "dd.MM.yyyy";

                    decimal finance = invoiceQuery.Where(a => a.CreatedDate >= dateFrom && a.CreatedDate < dateAfter)
                                                      .Select(a => a.TotalPrice.Value).DefaultIfEmpty(0).Sum();
                    finances.Add(new Tuple<string, decimal>(dateFrom.ToString(dateDisplayFormat), finance));

                    dateFrom = (splitByMonth) ? dateFrom.AddMonths(1) : dateFrom.AddDays(1);
                }

                return finances;
            }

            return null;
        }

        public List<Tuple<string, int, int>> GetCalls(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetListQuery().Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);
            IQueryable<CallDm> callQuery = CallBo.GetListQuery();

            List<Tuple<string, int, int>> calls = new List<Tuple<string, int, int>>();

            if (filter.ReportDateId.HasValue)
            {
                DateTime dateFrom = DateTime.Now;
                DateTime dateTo = DateTime.Now;
                bool splitByMonth = false;

                if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past7Days)
                {
                    dateFrom = DateTime.Now.Date.AddDays(-6);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past30Days)
                {
                    dateFrom = DateTime.Now.Date.AddDays(-29);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.PastYear)
                {
                    DateTime dateLastYear = DateTime.Now.Date.AddYears(-1).AddMonths(1);
                    dateFrom = new DateTime(dateLastYear.Year, dateLastYear.Month, 1);
                    splitByMonth = true;
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.AllTimeOrSelectDateRange)
                {
                    InvoiceDm earliestInvoice = invoiceQuery.Where(a => a.CreatedDate.HasValue).OrderBy(a => a.CreatedDate.Value).FirstOrDefault();

                    if (earliestInvoice == null) // if theres no invoices with created date
                    {
                        return null;
                    }

                    DateTime earliestInvoiceDate = earliestInvoice.CreatedDate.Value; // this is guaranteed to have value because of the query and check above
                    dateFrom = earliestInvoiceDate;

                    if (filter.DateFrom.HasValue)
                    {
                        if (filter.DateFrom.Value > dateFrom)
                        {
                            dateFrom = filter.DateFrom.Value;
                        }
                    }

                    if (filter.DateTo.HasValue)
                    {
                        if (filter.DateTo.Value < dateTo)
                        {
                            dateTo = filter.DateTo.Value;
                        }
                    }

                    // if dateTo is more than a month away from dateFrom: do more things
                    if (dateTo.AddMonths(-1) > dateFrom)
                    {
                        splitByMonth = true;
                    }
                }
                else
                {
                    return null;
                }

                while (dateFrom <= dateTo)
                {
                    DateTime dateAfter = (splitByMonth) ? dateFrom.AddMonths(1) : dateFrom.AddDays(1);
                    string dateDisplayFormat = (splitByMonth) ? "MM.yyyy" : "dd.MM.yyyy";

                    int callCount = callQuery.Where(a => a.CreatedDate >= dateFrom && a.CreatedDate < dateAfter)
                                             .Count();

                    int callSuceededCount = invoiceQuery.Where(a => a.CreatedDate >= dateFrom && a.CreatedDate < dateAfter)
                                                      .Count();

                    calls.Add(new Tuple<string, int, int>(dateFrom.ToString(dateDisplayFormat), callCount, callSuceededCount));

                    dateFrom = (splitByMonth) ? dateFrom.AddMonths(1) : dateFrom.AddDays(1);
                }

                return calls;
            }

            return null;
        }

        public List<Tuple<string, int, int>> GetProducts(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);
            IQueryable<LookUpCommodityDm> commodityQuery = LookUpBo.GetListQuery<LookUpCommodityDm>();

            List<Tuple<string, int, int>> dtos = (from a in commodityQuery
                                                join b in invoiceQuery.SelectMany(a => a.InvoiceCommodities).GroupBy(a => a.CommodityId).Select(a => new
                                                {
                                                    Id = a.Key,
                                                    SoldCount = a.Where(b => b.Invoice.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold).Select(b => b.Quantity).DefaultIfEmpty(0).Sum(),
                                                    CancelledCount = a.Where(b => b.Invoice.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Cancelled).Select(b => b.Quantity).DefaultIfEmpty(0).Sum(),
                                                })
                                                on a.Id equals b.Id into bGroup
                                                from bSub in bGroup.DefaultIfEmpty()
                                                select new
                                                {
                                                    Name = a.Name,
                                                    SoldCount = (bSub != null) ? bSub.SoldCount.Value : 0,
                                                    CancelledCount = (bSub != null) ? bSub.CancelledCount.Value : 0
                                                })
                                                .ToList()
                                                .Select(a => new Tuple<string, int, int>(a.Name, a.SoldCount, a.CancelledCount))
                                                .ToList();

            return dtos;
        }
    }
}

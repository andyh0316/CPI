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
        public FinanceBo(InvoiceBo InvoiceBo, CallBo CallBo)
        {
            this.InvoiceBo = InvoiceBo;
            this.CallBo = CallBo;
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

        public int GetReceivedCallCount(ReportDateFilter filter)
        {
            IQueryable<CallDm> callQuery = CallBo.GetDateFilteredQuery(filter);

            return callQuery.Count();
        }

        public List<Tuple<string, decimal>> GetRevenues(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            List<Tuple<string, decimal>> revenues = new List<Tuple<string, decimal>>();

            if (filter.ReportDateId.HasValue)
            {
                if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past30Days)
                {
                    for (DateTime currentDate = DateTime.Now.Date.AddMonths(-1); currentDate <= DateTime.Now.Date; currentDate = currentDate.AddDays(1))
                    {
                        DateTime dateAfter = currentDate.AddDays(1);
                        decimal revenue = invoiceQuery.Where(a => a.CreatedDate >= currentDate && a.CreatedDate < dateAfter)
                                                      .Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold)
                                                      .Select(a => a.TotalPrice.Value).DefaultIfEmpty(0).Sum();
                        revenues.Add(new Tuple<string, decimal>(currentDate.ToString("dd.MM"), revenue));
                    }

                    return revenues;
                }
                else
                {
                }
            }

            return null;
        }

        public List<Tuple<string, int, int>> GetCalls(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);
            IQueryable<CallDm> callQuery = CallBo.GetDateFilteredQuery(filter);

            List<Tuple<string, int, int>> calls = new List<Tuple<string, int, int>>();

            if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past30Days)
            {
                for (DateTime currentDate = DateTime.Now.Date.AddMonths(-1); currentDate <= DateTime.Now.Date; currentDate = currentDate.AddDays(1))
                {
                    DateTime dateAfter = currentDate.AddDays(1);
                    int callCount = callQuery.Where(a => a.CreatedDate >= currentDate && a.CreatedDate < dateAfter)
                                         .Count();

                    int callSuceededCount = invoiceQuery.Where(a => a.CreatedDate >= currentDate && a.CreatedDate < dateAfter)
                                                      .Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold)
                                                      .Count();

                    calls.Add(new Tuple<string, int, int>(currentDate.ToString("dd.MM"), callCount, callSuceededCount));
                }

                return calls;
            }
            else
            {
            }

            return null;
        }
    }
}

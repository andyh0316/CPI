using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.Filters;
using System;
using System.Linq;

namespace Cpi.Application.BusinessObjects.Other
{
    public class FinanceBo
    {
        public enum ReportDates
        {
            Today = 1,
            Yesterday = 2,
            ThisMonth = 3,
            Last30Days = 4,
            ThisYear = 5,
            AllTime = 6,
            SelectDates = 7
        }

        private InvoiceBo InvoiceBo;
        public FinanceBo(InvoiceBo InvoiceBo)
        {
            this.InvoiceBo = InvoiceBo;
        }

        public decimal GetRevenue(FinanceFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = GetFilteredQuery(filter);

            invoiceQuery = invoiceQuery.Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            decimal revenue = invoiceQuery.Select(a => a.TotalPrice.Value).DefaultIfEmpty(0).Sum();

            return revenue;
        }

        private IQueryable<InvoiceDm> GetFilteredQuery(FinanceFilter filter)
        {
            IQueryable<InvoiceDm> query = InvoiceBo.GetListQuery();

            if (filter.ReportDateId.HasValue)
            {
                if (filter.ReportDateId == (int)ReportDates.Today)
                {
                    DateTime dateFrom = DateTime.Now.Date;
                    query = query.Where(a => a.CreatedDate >= dateFrom);
                }
                else if (filter.ReportDateId == (int)ReportDates.Yesterday)
                {
                    DateTime dateFrom = DateTime.Now.Date.AddDays(-1);
                    DateTime dateTo = DateTime.Now.Date;
                    query = query.Where(a => a.CreatedDate >= dateFrom && a.CreatedDate < dateTo);
                }
                else if (filter.ReportDateId == (int)ReportDates.ThisMonth)
                {
                    DateTime dateNow = DateTime.Now;
                    DateTime dateFrom = new DateTime(dateNow.Year, dateNow.Month, 1);
                    query = query.Where(a => a.CreatedDate >= dateFrom);
                }
                else if (filter.ReportDateId == (int)ReportDates.Last30Days)
                {
                    DateTime dateFrom = DateTime.Now.Date.AddMonths(-1);
                    query = query.Where(a => a.CreatedDate >= dateFrom);
                }
                else if (filter.ReportDateId == (int)ReportDates.AllTime)
                {
                    // do nothing
                }
                else if (filter.ReportDateId == (int)ReportDates.SelectDates)
                {
                    if (filter.DateFrom.HasValue)
                    {
                        query = query.Where(a => a.CreatedDate >= filter.DateFrom.Value);
                    }

                    if (filter.DateTo.HasValue)
                    {
                        DateTime dateTo = filter.DateTo.Value.AddDays(1);
                        query = query.Where(a => a.CreatedDate < dateTo);
                    }
                }
            }

            return query;
        }

        //public decimal GetRevenueForThisMonth()
        //{
        //    IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetListQuery();

        //    DateTime dateFrom = DateTime.Now.AddMonths(-1).Date;
        //    invoiceQuery = invoiceQuery.Where(a => a.CreatedDate >= dateFrom && a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

        //    decimal revenue = invoiceQuery.Select(a => a.TotalPrice.Value).DefaultIfEmpty(0).Sum();

        //    return revenue;
        //}

        //public decimal GetCompletedInvoiceCount()
        //{
        //    IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetListQuery();

        //    invoiceQuery = invoiceQuery.Where(a => a.StatusId == LookUpInvoiceStatusDm.ID_COMPLETED);

        //    return invoiceQuery.Count();
        //}

        //public decimal GetTotalInvoiceCount()
        //{
        //    IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetListQuery();

        //    return invoiceQuery.Count();
        //}
    }
}

using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using System;
using System.Linq;

namespace Cpi.Application.BusinessObjects.Other
{
    public class FinanceBo
    {
        private InvoiceBo InvoiceBo;
        public FinanceBo(InvoiceBo InvoiceBo)
        {
            this.InvoiceBo = InvoiceBo;
        }

        public decimal GetRevenueForToday()
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetListQuery();

            DateTime dateFrom = DateTime.Now.Date;
            invoiceQuery = invoiceQuery.Where(a => a.CreatedDate >= dateFrom && a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            decimal revenue = invoiceQuery.Select(a => a.TotalPrice.Value).DefaultIfEmpty(0).Sum();

            return revenue;
        }

        public decimal GetRevenueForThisMonth()
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetListQuery();

            DateTime dateFrom = DateTime.Now.AddMonths(-1).Date;
            invoiceQuery = invoiceQuery.Where(a => a.CreatedDate >= dateFrom && a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            decimal revenue = invoiceQuery.Select(a => a.TotalPrice.Value).DefaultIfEmpty(0).Sum();

            return revenue;
        }

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

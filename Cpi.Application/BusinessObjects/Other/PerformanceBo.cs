using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.DataTransferObjects;
using Cpi.Application.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cpi.Application.BusinessObjects.Other
{
    public class PerformanceBo
    {
        private InvoiceBo InvoiceBo;
        private UserBo UserBo;
        public PerformanceBo(InvoiceBo InvoiceBo, UserBo UserBo)
        {
            this.InvoiceBo = InvoiceBo;
            this.UserBo = UserBo;
        }

        public List<Tuple<string, int>> GetPerformanceForOperators(ClassFilter.Performance filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetListQuery();

            invoiceQuery = InvoiceBo.GetDateFilteredQuery(invoiceQuery, filter.ReportDateFilter);

            invoiceQuery = invoiceQuery.Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            IQueryable<UserDm> operatorQuery = UserBo.GetListQuery().Where(a => a.UserOccupationId == (int)LookUpUserOccupationDm.LookUpIds.Operator);

            var preDtos = (from a in operatorQuery
                           join b in invoiceQuery
                           on a.Id equals b.OperatorId into bGroup
                           select new
                           {
                               Nickname = a.Nickname,
                               ProductSold = bGroup.SelectMany(b => b.InvoiceCommodities.Select(c => c.Quantity)).DefaultIfEmpty(0).Sum().Value,
                               FirstInvoiceDate = bGroup.OrderBy(b => b.Date).Select(b => b.Date).FirstOrDefault(),
                               LastInvoiceDate = bGroup.OrderByDescending(b => b.Date).Select(b => b.Date).FirstOrDefault()
                           }).ToList();

            List<Tuple<string, int>> dtos = new List<Tuple<string, int>>();

            foreach (var preDto in preDtos)
            {
                int productSold = preDto.ProductSold;

                if (filter.AveragePerMonth)
                {
                    if (preDto.FirstInvoiceDate.HasValue && preDto.LastInvoiceDate.HasValue)
                    {
                        double months = preDto.LastInvoiceDate.Value.Subtract(preDto.FirstInvoiceDate.Value).TotalDays / 30;
                        if (months > 0)
                        {
                            productSold = (int)(preDto.ProductSold / months);
                        }
                    }
                }

                Tuple<string, int> tuple = new Tuple<string, int>(preDto.Nickname, productSold);
                dtos.Add(tuple);
            }

            return dtos.OrderByDescending(a => a.Item2).ToList();
        }

        public List<Tuple<string, int, int>> GetPerformanceForDeliverStaff(ClassFilter.Performance filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetListQuery();

            invoiceQuery = InvoiceBo.GetDateFilteredQuery(invoiceQuery, filter.ReportDateFilter);

            invoiceQuery = invoiceQuery.Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            IQueryable<UserDm> operatorQuery = UserBo.GetListQuery().Where(a => a.UserOccupationId == (int)LookUpUserOccupationDm.LookUpIds.Delivery);

            List<Tuple<string, int, int>> dtos = (from a in operatorQuery
                                                  join b in invoiceQuery
                                                  on a.Id equals b.DeliveryStaffId into bGroup
                                                  select new
                                                  {
                                                      Nickname = a.Nickname,
                                                      ProductSold = bGroup.SelectMany(c => c.InvoiceCommodities.Select(d => d.Quantity)).DefaultIfEmpty(0).Sum().Value,
                                                      InvoiceSold = bGroup.Count(),
                                                  }).OrderByDescending(a => a.ProductSold).ToList()
                                                 .Select(a => new Tuple<string, int, int>(a.Nickname, a.ProductSold, a.InvoiceSold)).ToList();

            return dtos;
        }
    }
}

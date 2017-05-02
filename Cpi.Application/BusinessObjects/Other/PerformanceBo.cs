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

        public List<Tuple<string, int>> GetPerformanceForOperators(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            invoiceQuery = invoiceQuery.Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            IQueryable<UserDm> operatorQuery = UserBo.GetListQuery().Where(a => a.UserOccupationId == (int)LookUpUserOccupationDm.LookUpIds.Operator);

            List<Tuple<string, int>> dtos = (from a in operatorQuery
                                             join b in invoiceQuery
                                             on a.Id equals b.OperatorId into bGroup
                                             select new
                                             {
                                                 Nickname = a.Nickname,
                                                 ProductSold = bGroup.SelectMany(c => c.InvoiceCommodities.Select(d => d.Quantity)).DefaultIfEmpty(0).Sum().Value
                                             })
                                            .OrderByDescending(a => a.ProductSold).ToList()
                                            .Select(a => new Tuple<string, int>(a.Nickname, a.ProductSold)).ToList();

            return dtos;
        }

        public List<Tuple<string, int, int>> GetPerformanceForDeliverStaff(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

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

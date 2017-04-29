using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.DataTransferObjects;
using Cpi.Application.Filters;
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

        public List<EmployeePerformanceDto> GetPerformanceForOperators(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            invoiceQuery = invoiceQuery.Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            IQueryable<UserDm> operatorQuery = UserBo.GetListQuery().Where(a => a.UserOccupationId == (int)LookUpUserOccupationDm.LookUpIds.Operator);

            List<EmployeePerformanceDto> dtos = (from a in operatorQuery
                                                 join b in invoiceQuery
                                                 on a.Id equals b.OperatorId into bGroup
                                                 select new EmployeePerformanceDto
                                                 {
                                                     Nickname = a.Nickname,
                                                     PerformanceCount = bGroup.SelectMany(c => c.InvoiceCommodities.Select(d => d.Quantity)).DefaultIfEmpty(0).Sum()
                                                 }).OrderByDescending(a => a.PerformanceCount).ToList();

            return dtos;
        }

        public List<EmployeePerformanceDto> GetPerformanceForDeliverStaff(ReportDateFilter filter)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetDateFilteredQuery(filter);

            invoiceQuery = invoiceQuery.Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold);

            IQueryable<UserDm> operatorQuery = UserBo.GetListQuery().Where(a => a.UserOccupationId == (int)LookUpUserOccupationDm.LookUpIds.Delivery);

            List<EmployeePerformanceDto> dtos = (from a in operatorQuery
                                                 join b in invoiceQuery
                                                 on a.Id equals b.DeliveryStaffId into bGroup
                                                 select new EmployeePerformanceDto
                                                 {
                                                     Nickname = a.Nickname,
                                                     PerformanceCount = bGroup.Count()
                                                 }).OrderByDescending(a => a.PerformanceCount).ToList();

            return dtos;
        }
    }
}

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
        private CallBo CallBo;
        public PerformanceBo(InvoiceBo InvoiceBo, UserBo UserBo, CallBo CallBo)
        {
            this.InvoiceBo = InvoiceBo;
            this.UserBo = UserBo;
            this.CallBo = CallBo;
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

        public List<Tuple<string, int>> GetCallForWeekDays(ClassFilter.Performance filter)
        {
            IQueryable<CallDm> callQuery = CallBo.GetListQuery();

            callQuery = CallBo.GetDateFilteredQuery(callQuery, filter.ReportDateFilter);
            List<CallDm> calls = callQuery.ToList();

            List<Tuple<string, int>> dtos = new List<Tuple<string, int>>();
            dtos.Add(new Tuple<string, int>("Mon", calls.Where(a => a.Date.Value.DayOfWeek == DayOfWeek.Monday).Count()));
            dtos.Add(new Tuple<string, int>("Tue", calls.Where(a => a.Date.Value.DayOfWeek == DayOfWeek.Tuesday).Count()));
            dtos.Add(new Tuple<string, int>("Wed", calls.Where(a => a.Date.Value.DayOfWeek == DayOfWeek.Wednesday).Count()));
            dtos.Add(new Tuple<string, int>("Thu", calls.Where(a => a.Date.Value.DayOfWeek == DayOfWeek.Thursday).Count()));
            dtos.Add(new Tuple<string, int>("Fri", calls.Where(a => a.Date.Value.DayOfWeek == DayOfWeek.Friday).Count()));
            dtos.Add(new Tuple<string, int>("Sat", calls.Where(a => a.Date.Value.DayOfWeek == DayOfWeek.Saturday).Count()));
            dtos.Add(new Tuple<string, int>("Sun", calls.Where(a => a.Date.Value.DayOfWeek == DayOfWeek.Sunday).Count()));


            //List<Tuple<string, int, int>> dtos = (from a in operatorQuery
            //                                      join b in invoiceQuery
            //                                      on a.Id equals b.DeliveryStaffId into bGroup
            //                                      select new
            //                                      {
            //                                          Nickname = a.Nickname,
            //                                          ProductSold = bGroup.SelectMany(c => c.InvoiceCommodities.Select(d => d.Quantity)).DefaultIfEmpty(0).Sum().Value,
            //                                          InvoiceSold = bGroup.Count(),
            //                                      }).OrderByDescending(a => a.ProductSold).ToList()
            //                                     .Select(a => new Tuple<string, int, int>(a.Nickname, a.ProductSold, a.InvoiceSold)).ToList();

            return dtos;
        }

        public List<Tuple<string, int>> GetCallForTimes(ClassFilter.Performance filter)
        {
            TimeSpan startTime = new TimeSpan(6, 0, 0);
            TimeSpan endTime = new TimeSpan(20, 0, 0);

            IQueryable<CallDm> callQuery = CallBo.GetListQuery();

            callQuery = CallBo.GetDateFilteredQuery(callQuery, filter.ReportDateFilter);
            List<CallDm> calls = callQuery.ToList();

            List<Tuple<string, int>> dtos = new List<Tuple<string, int>>();

            TimeSpan currentTime = startTime;
            TimeSpan timeRange = new TimeSpan(0, 30, 0);

            while (currentTime <= endTime)
            {
                int callCount = calls.Where(a => a.CreatedDate.Value.TimeOfDay >= currentTime && a.CreatedDate.Value.TimeOfDay < currentTime.Add(timeRange)).Count();
                string displayTimes = string.Format("{0} - {1}", new DateTime(currentTime.Ticks).ToString("HH:mm"), new DateTime(currentTime.Add(timeRange).Ticks).ToString("HH:mm"));
                dtos.Add(new Tuple<string, int>(displayTimes, callCount));

                currentTime = currentTime.Add(timeRange);
            }

            return dtos;
        }
    }
}

using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using System;
using System.Linq;

namespace Cpi.Application.BusinessObjects.Other
{
    public class FinanceBo
    {
        private CallBo CallBo;
        public FinanceBo(CallBo CallBo)
        {
            this.CallBo = CallBo;
        }

        //public decimal GetRevenueForToday()
        //{
        //    IQueryable<CallDm> callQuery = CallBo.GetListQuery();

        //    DateTime dateFrom = DateTime.Now.Date.ToUniversalTime();
        //    callQuery = callQuery.Where(a => a.CompletionDate >= dateFrom);

        //    decimal revenue = callQuery.Select(a => a.TotalPrice.Value).DefaultIfEmpty(0).Sum();

        //    return revenue;
        //}

        //public decimal GetRevenueForThisMonth()
        //{
        //    IQueryable<CallDm> callQuery = CallBo.GetListQuery();

        //    DateTime dateFrom = DateTime.Now.AddMonths(-1).Date.ToUniversalTime();
        //    callQuery = callQuery.Where(a => a.CompletionDate >= dateFrom);

        //    decimal revenue = callQuery.Select(a => a.TotalPrice.Value).DefaultIfEmpty(0).Sum();

        //    return revenue;
        //}

        //public decimal GetCompletedCallCount()
        //{
        //    IQueryable<CallDm> callQuery = CallBo.GetListQuery();

        //    callQuery = callQuery.Where(a => a.StatusId == LookUpCallStatusDm.ID_COMPLETED);

        //    return callQuery.Count();
        //}

        //public decimal GetTotalCallCount()
        //{
        //    IQueryable<CallDm> callQuery = CallBo.GetListQuery();

        //    return callQuery.Count();
        //}
    }
}

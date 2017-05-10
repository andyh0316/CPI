﻿using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using System.Linq;
using System;

namespace Cpi.Application.BusinessObjects
{
    public class ExpenseBo : BaseBo<ExpenseDm>
    {
        //public IQueryable<ExpenseDm> GetListBaseQuery(ListFilter.Expense filter)
        //{
        //    //IQueryable<ExpenseDm> query = GetListQuery().Include(a => a.Address).Include(a => a.Operator.UserOccupation).Include(a => a.Operator.UserRole).Include(a => a.DeliveryStaff.UserOccupation).Include(a => a.DeliveryStaff.UserRole).Include(a => a.Status).Include(a => a.Commodities);
        //    IQueryable<ExpenseDm> query = GetListQuery();

        //    if (!string.IsNullOrEmpty(filter.SearchString))
        //    {
        //        query = query.Where(a => a.CustomerName.StartsWith(filter.SearchString) ||
        //                                 a.CustomerPhone.StartsWith(filter.SearchString));
        //    }

        //    if (filter.AdvancedSearch != null)
        //    {

        //        if (filter.AdvancedSearch.StatusId.HasValue)
        //        {
        //            query = query.Where(a => a.StatusId == filter.AdvancedSearch.StatusId.Value);
        //        }

        //        if (filter.AdvancedSearch.CreatedTodayOnly)
        //        {
        //            DateTime dateFrom = DateTime.Now.Date;
        //            query = query.Where(a => a.CreatedDate >= dateFrom);
        //        }

        //        if (filter.AdvancedSearch.CreatedDateFrom.HasValue)
        //        {
        //            query = query.Where(a => a.CreatedDate >= filter.AdvancedSearch.CreatedDateFrom.Value);
        //        }

        //        if (filter.AdvancedSearch.CreatedDateTo.HasValue)
        //        {
        //            DateTime dateTo = filter.AdvancedSearch.CreatedDateTo.Value.AddDays(1);
        //            query = query.Where(a => a.CreatedDate < dateTo);
        //        }
        //    }

        //    return query;
        //}

        //public IQueryable<ExpenseDm> GetDateFilteredQuery(ReportDateFilter filter)
        //{
        //    IQueryable<ExpenseDm> query = GetListQuery();

        //    if (filter.ReportDateId.HasValue)
        //    {
        //        if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Today)
        //        {
        //            DateTime dateFrom = DateTime.Now.Date;
        //            query = query.Where(a => a.CreatedDate >= dateFrom);
        //        }
        //        else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Yesterday)
        //        {
        //            DateTime dateFrom = DateTime.Now.Date.AddDays(-1);
        //            DateTime dateTo = DateTime.Now.Date;
        //            query = query.Where(a => a.CreatedDate >= dateFrom && a.CreatedDate < dateTo);
        //        }
        //        else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past7Days)
        //        {
        //            DateTime dateFrom = DateTime.Now.Date.AddDays(-6);
        //            query = query.Where(a => a.CreatedDate >= dateFrom);
        //        }
        //        else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past30Days)
        //        {
        //            DateTime dateFrom = DateTime.Now.Date.AddDays(-29);
        //            query = query.Where(a => a.CreatedDate >= dateFrom);
        //        }
        //        else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.PastYear)
        //        {
        //            DateTime dateFrom = DateTime.Now.Date.AddYears(-1);
        //            query = query.Where(a => a.CreatedDate >= dateFrom);
        //        }
        //        else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.AllTimeOrSelectDateRange)
        //        {
        //            if (filter.DateFrom.HasValue)
        //            {
        //                query = query.Where(a => a.CreatedDate >= filter.DateFrom.Value);
        //            }

        //            if (filter.DateTo.HasValue)
        //            {
        //                DateTime dateTo = filter.DateTo.Value.AddDays(1);
        //                query = query.Where(a => a.CreatedDate < dateTo);
        //            }
        //        }
        //    }

        //    return query;
        //}
    }
}

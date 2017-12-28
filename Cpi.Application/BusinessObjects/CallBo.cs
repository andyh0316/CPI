using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using System.Linq;
using System;
using Cpi.Application.Helpers;
using Cpi.Application.DataModels.LookUp;

namespace Cpi.Application.BusinessObjects
{
    public class CallBo : BaseBo<CallDm>
    {
        public IQueryable<CallDm> GetListQuery()
        {
            IQueryable<CallDm> query = base.GetListQuery();

            if (UserHelper.GetRoleId() != (int)LookUpUserRoleDm.LookUpIds.老子)
            {
                DateTime globalFilteredDate = CommonHelper.GetGlobalFilteredDate();
                query = query.Where(a => a.Date >= globalFilteredDate);
            }

            return query;
        }

        public IQueryable<CallDm> GetListBaseQuery(ListFilter.Call filter)
        {
            IQueryable<CallDm> query = GetListQuery();

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                query = query.Where(a => a.CustomerName.Contains(filter.SearchString) ||
                                         a.CustomerPhone.Contains(filter.SearchString));
            }

            if (filter.AdvancedSearch != null)
            {
                if (filter.AdvancedSearch.StatusId.HasValue)
                {
                    query = query.Where(a => a.StatusId == filter.AdvancedSearch.StatusId.Value);
                }

                if (filter.AdvancedSearch.ReportDateFilter != null)
                {
                    query = GetDateFilteredQuery(query, filter.AdvancedSearch.ReportDateFilter);
                }
            }

            return query;
        }

        public bool CallWithPhoneExistsToday(string phoneNumber)
        {
            DateTime today = DateTime.Now.Date;
            return GetListQuery().Any(a => a.CustomerPhone == phoneNumber && a.Date >= today);
        }

        public IQueryable<CallDm> GetDateFilteredQuery(IQueryable<CallDm> query, ReportDateFilter filter)
        {
            if (filter != null && filter.ReportDateId.HasValue)
            {
                if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Today)
                {
                    DateTime dateFrom = DateTime.Now.Date;
                    query = query.Where(a => a.Date >= dateFrom);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Yesterday)
                {
                    DateTime dateFrom = DateTime.Now.Date.AddDays(-1);
                    DateTime dateTo = DateTime.Now.Date;
                    query = query.Where(a => a.Date >= dateFrom && a.Date < dateTo);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past7Days)
                {
                    DateTime dateFrom = DateTime.Now.Date.AddDays(-6);
                    query = query.Where(a => a.Date >= dateFrom);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past30Days)
                {
                    DateTime dateFrom = DateTime.Now.Date.AddDays(-29);
                    query = query.Where(a => a.Date >= dateFrom);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.PastYear)
                {
                    DateTime dateFrom = DateTime.Now.Date.AddYears(-1);
                    query = query.Where(a => a.Date >= dateFrom);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.AllTimeOrSelectDateRange)
                {
                    if (filter.DateFrom.HasValue)
                    {
                        query = query.Where(a => a.Date >= filter.DateFrom.Value);
                    }

                    if (filter.DateTo.HasValue)
                    {
                        DateTime dateTo = filter.DateTo.Value.AddDays(1);
                        query = query.Where(a => a.Date < dateTo);
                    }
                }
            }

            return query;
        }

        public Tuple<DateTime, DateTime, bool> GetDateFilteredInfo(IQueryable<CallDm> query, ReportDateFilter filter)
        {
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            bool splitByMonth = false;

            // we don't have today or yesterday here because if its one day we return null to indicate that the graphs don't need to show
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
                CallDm earliestEntity = query.Where(a => a.Date.HasValue).OrderBy(a => a.Date.Value).FirstOrDefault();

                if (earliestEntity == null) // if theres no invoices with created date
                {
                    return null;
                }

                DateTime earliestEntityDate = earliestEntity.Date.Value; // this is guaranteed to have value because of the query and check above
                dateFrom = earliestEntityDate;

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

                // if dateTo is more than a month away from dateFrom: then we have to split by month
                if (dateTo.AddMonths(-1) >= dateFrom)
                {
                    splitByMonth = true;
                    dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, 1);
                    dateTo = new DateTime(dateTo.Year, dateTo.Month, DateTime.DaysInMonth(dateTo.Year, dateTo.Month));
                }
            }
            else
            {
                return null;
            }

            return new Tuple<DateTime, DateTime, bool>(dateFrom, dateTo, splitByMonth);
        }

        public IQueryable<CallDm> GetQueryByDate(DateTime date)
        {
            DateTime startDate = date.Date;
            DateTime endDate = startDate.AddDays(1);
            return GetListQuery().Where(a => a.Date >= startDate && a.Date < endDate);
        }
    }
}

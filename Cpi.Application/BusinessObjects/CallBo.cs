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
                query = query.Where(a => a.CreatedDate >= globalFilteredDate);
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
            return GetListQuery().Any(a => a.CustomerPhone == phoneNumber && a.CreatedDate >= today);
        }
    }
}

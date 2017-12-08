using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using System.Linq;
using System;
using Cpi.Application.Helpers;
using Cpi.Application.DataModels.LookUp;

namespace Cpi.Application.BusinessObjects
{
    public class ExpenseBo : BaseBo<ExpenseDm>
    {
        public IQueryable<ExpenseDm> GetListQuery()
        {
            IQueryable<ExpenseDm> query = base.GetListQuery();

            if (UserHelper.GetRoleId() != (int)LookUpUserRoleDm.LookUpIds.老子)
            {
                DateTime globalFilteredDate = CommonHelper.GetGlobalFilteredDate();
                query = query.Where(a => a.CreatedDate >= globalFilteredDate);
            }

            return query;
        }

        public IQueryable<ExpenseDm> GetListBaseQuery(ListFilter.Expense filter)
        {
            IQueryable<ExpenseDm> query = GetListQuery();

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                query = query.Where(a => a.Name.Contains(filter.SearchString) ||
                                         a.Expense.ToString().StartsWith(filter.SearchString));
            }

            if (filter.AdvancedSearch != null)
            {
                if (filter.AdvancedSearch.ReportDateFilter != null)
                {
                    query = GetDateFilteredQuery(query, filter.AdvancedSearch.ReportDateFilter);
                }

                if (filter.AdvancedSearch.LocationId.HasValue)
                {
                    query = query.Where(a => a.LocationId == filter.AdvancedSearch.LocationId);
                }
            }

            return query;
        }
    }
}

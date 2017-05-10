using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using System.Linq;
using System;

namespace Cpi.Application.BusinessObjects
{
    public class ExpenseBo : BaseBo<ExpenseDm>
    {
        public IQueryable<ExpenseDm> GetListBaseQuery(ListFilter.Expense filter)
        {
            IQueryable<ExpenseDm> query = GetListQuery();

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                query = query.Where(a => a.Name.StartsWith(filter.SearchString) ||
                                         a.Expense.ToString().StartsWith(filter.SearchString));
            }

            if (filter.AdvancedSearch != null)
            {
                if (filter.AdvancedSearch.ReportDateFilter != null)
                {
                    query = GetDateFilteredQuery(query, filter.AdvancedSearch.ReportDateFilter);
                }
            }

            return query;
        }
    }
}

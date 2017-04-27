using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using System.Linq;
using System;

namespace Cpi.Application.BusinessObjects
{
    public class InvoiceBo : BaseBo<InvoiceDm>
    {
        public IQueryable<InvoiceDm> GetListBaseQuery(ListFilter.Invoice filter)
        {
            IQueryable<InvoiceDm> query = GetListQuery();

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                query = query.Where(a => a.CustomerPhone.StartsWith(filter.SearchString) ||
                                         a.Operator.Nickname.StartsWith(filter.SearchString) ||
                                         a.DeliveryStaff.Nickname.StartsWith(filter.SearchString));
            }

            if (filter.AdvancedSearch != null)
            {

                if (filter.AdvancedSearch.StatusId.HasValue)
                {
                    query = query.Where(a => a.StatusId == filter.AdvancedSearch.StatusId.Value);
                }

                if (filter.AdvancedSearch.CreatedTodayOnly)
                {
                    DateTime dateFrom = DateTime.Now.Date;
                    query = query.Where(a => a.CreatedDate >= dateFrom);
                }

                if (filter.AdvancedSearch.CreatedDateFrom.HasValue)
                {
                    query = query.Where(a => a.CreatedDate >= filter.AdvancedSearch.CreatedDateFrom.Value);
                }

                if (filter.AdvancedSearch.CreatedDateTo.HasValue)
                {
                    // add one day minus one second to cover towards the end of day since CreatedDate contains time
                    DateTime dateTo = filter.AdvancedSearch.CreatedDateTo.Value.AddDays(1).AddSeconds(-1);
                    query = query.Where(a => a.CreatedDate <= dateTo);
                }
            }

            return query;
        }
    }
}

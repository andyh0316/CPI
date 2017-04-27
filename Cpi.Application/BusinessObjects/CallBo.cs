using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using System.Linq;
using System;

namespace Cpi.Application.BusinessObjects
{
    public class CallBo : BaseBo<CallDm>
    {
        public IQueryable<CallDm> GetListBaseQuery(ListFilter.Call filter)
        {
            //IQueryable<CallDm> query = GetListQuery().Include(a => a.Address).Include(a => a.Operator.UserOccupation).Include(a => a.Operator.UserRole).Include(a => a.DeliveryStaff.UserOccupation).Include(a => a.DeliveryStaff.UserRole).Include(a => a.Status).Include(a => a.Commodities);
            IQueryable<CallDm> query = GetListQuery();

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                query = query.Where(a => a.CustomerName.StartsWith(filter.SearchString) ||
                                         a.CustomerPhone.StartsWith(filter.SearchString));
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

        public bool CallWithPhoneExistsToday(string phoneNumber)
        {
            DateTime today = DateTime.Now.Date;
            return GetListQuery().Any(a => a.CustomerPhone == phoneNumber && a.CreatedDate >= today);
        }
    }
}

using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using System.Linq;
using System.Data.Entity;

namespace Cpi.Application.BusinessObjects
{
    public class CallBo : BaseBo<CallDm>
    {
        public IQueryable<CallDm> GetListBaseQuery(ListFilter.Call filter)
        {
            //IQueryable<CallDm> query = GetListQuery().Include(a => a.Address).Include(a => a.Operator.UserOccupation).Include(a => a.Operator.UserRole).Include(a => a.DeliveryStaff.UserOccupation).Include(a => a.DeliveryStaff.UserRole).Include(a => a.Status).Include(a => a.Commodities);
            IQueryable<CallDm> query = GetListQuery().Include(a => a.Address);

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                query = query.Where(a => a.CustomerName.StartsWith(filter.SearchString) ||
                                         a.CustomerPhone.StartsWith(filter.SearchString) ||
                                         a.Operator.Name.StartsWith(filter.SearchString) ||
                                         a.DeliveryStaff.Name.StartsWith(filter.SearchString));
            }

            return query;
        }
    }
}

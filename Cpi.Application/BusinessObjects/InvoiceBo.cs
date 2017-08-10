using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using System.Linq;
using System;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.Helpers;

namespace Cpi.Application.BusinessObjects
{
    public class InvoiceBo : BaseBo<InvoiceDm>
    {
        public IQueryable<InvoiceDm> GetListQuery()
        {
            IQueryable<InvoiceDm> query = base.GetListQuery();

            if (UserHelper.GetRoleId() != (int)LookUpUserRoleDm.LookUpIds.老子)
            {
                DateTime globalFilteredDate = CommonHelper.GetGlobalFilteredDate();
                query = query.Where(a => a.CreatedDate >= globalFilteredDate);
            }

            return query;
        }

        public IQueryable<InvoiceDm> GetListBaseQuery(ListFilter.Invoice filter)
        {
            IQueryable<InvoiceDm> query = GetListQuery();

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                query = query.Where(a => a.CustomerPhone.StartsWith(filter.SearchString) ||
                                         a.Operator.Nickname.StartsWith(filter.SearchString) ||
                                         a.Operator.Fullname.StartsWith(filter.SearchString) ||
                                         a.DeliveryStaff.Nickname.StartsWith(filter.SearchString) ||
                                         a.DeliveryStaff.Fullname.StartsWith(filter.SearchString));
            }

            if (filter.AdvancedSearch != null)
            {
                if (filter.AdvancedSearch.LocationId.HasValue)
                {
                    query = query.Where(a => a.LocationId == filter.AdvancedSearch.LocationId.Value);
                }

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
    }
}

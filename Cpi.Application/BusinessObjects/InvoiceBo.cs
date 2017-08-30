using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using System.Linq;
using System;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.Helpers;
using System.Collections.Generic;
using Cpi.Application.DataTransferObjects;

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

        public List<InvoiceSummaryDto> GetDailyInvoiceSummary(DateTime date)
        {
            IQueryable<InvoiceDm> invoices = GetQueryByCreateDate(date);

            List<InvoiceSummaryDto> dtos = invoices.SelectMany(a => a.InvoiceCommodities).GroupBy(a => a.Invoice.Location).OrderBy(a => a.Key.DisplayOrder).ToList().Select(a => new InvoiceSummaryDto
            {
                Location = a.Key.Name,
                Commodities = a.GroupBy(b => b.Commodity).Select(b => new Tuple<string, int> 
                    (
                        b.Key.Name,
                        b.Select(c => c.Quantity.Value).DefaultIfEmpty(0).Sum()
                    )
                ).ToList(),
                TotalPrice = a.Select(b => b.Invoice.TotalPrice.Value).DefaultIfEmpty(0).Sum()
            }).ToList();

            return dtos;
        }
    }
}

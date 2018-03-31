using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.DataTransferObjects;
using Cpi.Application.Filters;
using Cpi.Application.Helpers;
using Cpi.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cpi.Application.BusinessObjects
{
    public class UserBo : BaseBo<UserDm>
    {
        private InvoiceBo InvoiceBo;
        public UserBo(InvoiceBo InvoiceBo)
        {
            this.InvoiceBo = InvoiceBo;
        }

        public IQueryable<UserDto> GetListBaseQuery(ListFilter.User filter)
        {
            IQueryable<UserDm> query = GetListQuery();
             
            if (UserHelper.GetRoleId() != (int)LookUpUserRoleDm.LookUpIds.Laozi)
            {
                query = query.Where(a => a.UserRoleId != (int)LookUpUserRoleDm.LookUpIds.Laozi);
            }

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                query = query.Where(a => a.Nickname.StartsWith(filter.SearchString) ||
                                         a.Fullname.StartsWith(filter.SearchString));
            }

            //if (filter.AdvancedSearch != null)
            //{
            //    if (filter.AdvancedSearch.StatusId.HasValue)
            //    {
            //        query = query.Where(a => a.StatusId == filter.AdvancedSearch.StatusId.Value);
            //    }

            //    if (filter.AdvancedSearch.ReportDateFilter != null)
            //    {
            //        query = GetDateFilteredQuery(query, filter.AdvancedSearch.ReportDateFilter);
            //    }
            //}

            IQueryable<UserDto> dtoQuery = query.Select(a => new UserDto
            {
                Id = a.Id,
                Username = a.Username,
                Fullname = a.Fullname,
                Nickname = a.Nickname,
                UserRole = a.UserRole.Name,
                UserOccupation = a.UserOccupation.Name,
                LastLoginDate = a.LastLoginDate,
                Salary = a.Salary,
                StartDate = a.StartDate,
                VacationDaysTaken = a.VacationDaysTaken,
                WorkDays = a.WorkDays.ToList()
            });

            return dtoQuery;
        }

        public UserDm GetByUsername(string username)
        {
            return GetListQuery().Where(a => a.Username == username).SingleOrDefault();
        }

        public List<CpiSelectListItem> GetSearchDropDownList(int? occupationId = null)
        {
            var query = GetListQuery();
            if (occupationId.HasValue)
            {
                query = query.Where(a => a.UserOccupationId == occupationId);
            }

            return query.OrderBy(a => a.Nickname).Select(a => new CpiSelectListItem
            {
                Id = a.Id,
                Name = a.Nickname
            }).ToList();
        }

        public List<UserSalaryDto> GetUserSalaries(DateTime dateFrom, DateTime dateTo)
        {
            IQueryable<InvoiceDm> invoiceQuery = InvoiceBo.GetListQuery().Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold).Where(a => a.Date >= dateFrom && a.Date <= dateTo);

            IQueryable<UserDm> userQuery = GetListQuery().Where(a => a.UserRoleId != (int)LookUpUserRoleDm.LookUpIds.Laozi)
                .OrderBy(a => a.UserOccupationId == (int)LookUpUserOccupationDm.LookUpIds.Operator)
                .ThenBy(a => a.UserOccupationId == (int)LookUpUserOccupationDm.LookUpIds.Delivery);

            List<UserSalaryDto> salaries = (from a in userQuery
                                            join b in invoiceQuery
                                            on a.Id equals b.OperatorId into bGroup
                                            join c in invoiceQuery
                                            on a.Id equals c.DeliveryStaffId into cGroup
                                            //from bSub in bGroup.DefaultIfEmpty()
                                            //from cSub in cGroup.DefaultIfEmpty()
                                            select new UserSalaryDto
                                            {
                                                UserFullname = a.Fullname,
                                                UserNickname = a.Nickname,
                                                Occupation = a.UserOccupation.Name,
                                                Salary = a.Salary,
                                                AmountSold = bGroup.SelectMany(b => b.InvoiceCommodities.Select(c => c.Quantity)).DefaultIfEmpty(0).Sum(),
                                                AmountDelivered = cGroup.Count(),
                                                SoldBonus = bGroup.SelectMany(b => b.InvoiceCommodities.Select(c => c.Quantity)).DefaultIfEmpty(0).Sum(),
                                                DeliveredBonus = cGroup.Count()
                                                //Delivered = cGroup.SelectMany(b => b.InvoiceCommodities.Select(c => c.Quantity)).DefaultIfEmpty(0).Sum()
                                            }).ToList();

            foreach (UserSalaryDto salary in salaries)
            {
                salary.TotalPay = salary.Salary / 2 + salary.DeliveredBonus + salary.SoldBonus;
            }

            return salaries;
        }
    }
}

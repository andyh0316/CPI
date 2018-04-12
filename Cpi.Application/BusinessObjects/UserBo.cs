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
            List<InvoiceDm> invoices = InvoiceBo.GetListQuery().Where(a => a.StatusId == (int)LookUpInvoiceStatusDm.LookUpIds.Sold).Where(a => a.Date >= dateFrom && a.Date <= dateTo).ToList();

            List<UserDm> users = GetListQuery().Where(a => a.UserRoleId != (int)LookUpUserRoleDm.LookUpIds.Laozi)
                .OrderBy(a => a.UserOccupationId == (int)LookUpUserOccupationDm.LookUpIds.Operator)
                .ThenBy(a => a.UserOccupationId == (int)LookUpUserOccupationDm.LookUpIds.Delivery)
                .ToList();

            List<UserSalaryDto> salaries = (from a in users
                                            join b in invoices
                                            on a.Id equals b.OperatorId into bGroup
                                            join c in invoices
                                            on a.Id equals c.DeliveryStaffId into cGroup
                                            select new UserSalaryDto
                                            {
                                                UserFullname = a.Fullname,
                                                UserNickname = a.Nickname,
                                                Occupation = (a.UserOccupationId.HasValue) ? a.UserOccupation.Name : null,
                                                Salary = a.Salary,
                                                AmountSold = bGroup.SelectMany(b => b.InvoiceCommodities.Select(c => c.Quantity)).DefaultIfEmpty(0).Sum(),
                                                //AmountDelivered = cGroup.GroupBy(a => a.DeliveryDistance).Select(a => new Tuple<string, int>(a.Key.Name, a.Count())).ToList(),
                                                SoldBonus = bGroup.SelectMany(b => b.InvoiceCommodities.Select(c => c.Quantity)).DefaultIfEmpty(0).Sum(),
                                                DeliveryBonuses = cGroup.GroupBy(a => a.DeliveryDistance).OrderBy(a => a.Key.DisplayOrder).Select(a => new UserSalaryDeliveryBonusDto
                                                {
                                                    DeliveryDistance = a.Key.Name,
                                                    AmountDelivered = a.Count(),
                                                    Bonus = (a.Key.Id == (int)LookUpDeliveryDistanceDm.LookUpIds._0_to_15) ? a.Count() * 1 :
                                                            (a.Key.Id == (int)LookUpDeliveryDistanceDm.LookUpIds._15_to_30) ? a.Count() * 2 :
                                                            (a.Key.Id == (int)LookUpDeliveryDistanceDm.LookUpIds._30_to_40) ? a.Count() * 3 :
                                                            (a.Key.Id == (int)LookUpDeliveryDistanceDm.LookUpIds._40_or_more) ? a.Count() * 4 :
                                                            a.Count() * 1
                                                }).ToList()
                                            }).ToList();

            foreach (UserSalaryDto salary in salaries)
            {
                salary.TotalPay = salary.Salary / 2 + salary.DeliveryBonuses.Select(a => a.Bonus).DefaultIfEmpty(0).Sum() + salary.SoldBonus;
            }

            return salaries;
        }
    }
}

using AutoMapper;
using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.BusinessObjects.LookUp;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.DataTransferObjects;
using Cpi.Application.Filters;
using Cpi.Application.Helpers;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Manage.Controllers
{
    [CpiAuthenticate((int)LookUpPermissionDm.LookUpIds.User)]
    public class UserController : BaseController
    {
        private UserBo UserBo;
        private LookUpBo LookUpBo;
        public UserController(UserBo UserBo, LookUpBo LookUpBo)
        {
            this.UserBo = UserBo;
            this.LookUpBo = LookUpBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(ListFilter.User filter)
        {
            IQueryable<UserDto> query = UserBo.GetListBaseQuery(filter);
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter, query.Count());
            List<UserDto> listItems = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortObjects).ToList();
            return JsonModel(new { ListItems = listItems, ListLoadCalculator = listLoadCalculator });
        }

        [HttpGet]
        public ContentResult GetListData()
        {
            var model = new
            {
                Occupations = LookUpBo.GetList<LookUpUserOccupationDm>().ToList(),
                Permissions = LookUpBo.GetList<LookUpPermissionDm>().ToList(),
                WeekDays = LookUpBo.GetList<LookUpWeekDayDm>()
            };

            return JsonModel(model);
        }

        [HttpGet]
        public ContentResult GetUser(int id)
        {
            UserDm user;
            if (id == 0)
            {
                user = new UserDm
                {
                    Salary = 0,
                    VacationDaysTaken = 0,
                    StartDate = new DateTime(DateTime.Now.Date.Ticks),
                    UserRoleId = (int)LookUpUserRoleDm.LookUpIds.Staff,
                };
            }
            else
            {
                user = UserBo.GetById(id);
            }

            var model = new
            {
                User = user,
                Occupations = LookUpBo.GetList<LookUpUserOccupationDm>().ToList(),
                UserRoles = LookUpBo.GetList<LookUpUserRoleDm>().Where(a => a.Id != (int)LookUpUserRoleDm.LookUpIds.Laozi).ToList(),
                WeekDays = LookUpBo.GetList<LookUpWeekDayDm>()
            };

            return JsonModel(model);
        }

        [HttpPost]
        public ContentResult SaveUser(UserDm user)
        {
            if (!ModelState.IsValid)
            {
                return JsonModelState(ModelState);
            }

            UserHelper.CheckPermissionForEntity(user, (int)LookUpPermissionDm.LookUpIds.User);

            UserDm trackedUser = (user.Id == 0) ? new UserDm() : UserBo.GetById(user.Id);
            Mapper.Map(user, trackedUser);

            List<LookUpWeekDayDm> allWorkDays = LookUpBo.GetList<LookUpWeekDayDm>();
            user.WorkDays = (user.WorkDays) ?? new List<LookUpWeekDayDm>();
            trackedUser.WorkDays = (trackedUser.WorkDays) ?? new List<LookUpWeekDayDm>();
            trackedUser.WorkDays.Clear();
            trackedUser.WorkDays = allWorkDays.Where(m => user.WorkDays.Select(d => d.Id).Contains(m.Id)).ToList();

            if (user.Id == 0)
            {
                UserBo.Add(user);
            }

            UserBo.Commit();

            return JsonModel(null);
        }

        [CpiAuthenticate((int)LookUpPermissionDm.LookUpIds.User, (int)LookUpPermissionDm.ActionIds.Delete)]
        [HttpGet]
        public ContentResult DeleteUser(int id)
        {
            UserDm trackedUser = UserBo.GetById(id);
            UserBo.Remove(trackedUser);
            UserBo.Commit();

            return JsonModel(null);
        }

        public ActionResult LoginAsUser(int id)
        {
            // only Cobro superusers can do this
            if (UserHelper.GetRoleId() != (int)LookUpUserRoleDm.LookUpIds.Laozi)
            {
                throw new Exception("Unauthorized");
            }

            UserDm loggingInUser = UserBo.GetById(id);

            UserHelper.Logout();
            UserHelper.Login(loggingInUser);

            return RedirectToAction("Index", "Call", new { area = "Call" });
        }

        [HttpGet]
        public ContentResult GetSalarySheet()
        {
            DateTime today = DateTime.Today;
            DateTime dateFrom;
            DateTime dateTo;

            dateFrom = new DateTime(today.Year, today.Month, 1);
            dateTo = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

            var model = new
            {
                DateFrom = dateFrom,
                DateTo = dateTo
            };

            return JsonModel(model);
        }

        [HttpPost]
        public ContentResult GetSalarySheet(DateTime dateFrom, DateTime dateTo)
        {
            List<UserSalaryDto> userSalaries = UserBo.GetUserSalaries(dateFrom, dateTo);

            var model = new
            {
                UserSalaries = userSalaries,
                DateFrom = dateFrom,
                DateTo = dateTo,
                TotalDeliveredBonus = userSalaries.Select(a => a.DeliveredBonus).Sum(),
                TotalSoldBonus = userSalaries.Select(a => a.SoldBonus).Sum(),
                TotalSalary = userSalaries.Select(a => a.Salary / 2).Sum(),
                TotalAmountDelivered = userSalaries.Select(a => a.AmountDelivered).Sum(),
                TotalAmountSold = userSalaries.Select(a => a.AmountSold).Sum()
            };

            return JsonModel(model);
        }
    }
}

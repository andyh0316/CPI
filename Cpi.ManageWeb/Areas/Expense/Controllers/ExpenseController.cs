using AutoMapper;
using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.BusinessObjects.LookUp;
using Cpi.Application.BusinessObjects.Other;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.Filters;
using Cpi.Application.Helpers;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Expense.Controllers
{
    [CpiAuthenticate((int)LookUpPermissionDm.LookUpIds.Expense)]
    public class ExpenseController : BaseController
    {
        private ExpenseBo ExpenseBo;
        private FinanceBo FinanceBo;
        private LookUpBo LookUpBo;
        private UserBo UserBo;
        public ExpenseController(ExpenseBo ExpenseBo, FinanceBo FinanceBo, LookUpBo LookUpBo, UserBo UserBo)
        {
            this.ExpenseBo = ExpenseBo;
            this.FinanceBo = FinanceBo;
            this.LookUpBo = LookUpBo;
            this.UserBo = UserBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(ListFilter.Expense filter)
        {
            IQueryable<ExpenseDm> query = ExpenseBo.GetListBaseQuery(filter);
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter, query.Count());
            List<ExpenseDm> listItems = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortObjects).ToList();
            return JsonModel(new { ListItems = listItems, ListLoadCalculator = listLoadCalculator });
        }

        [HttpGet]
        public ContentResult GetListData()
        {
            var model = new
            {
                ReportDates = ReportDateFilter.GetSelectList(),
                ReportDateIdEnums = EnumHelper.GetEnumIntList(typeof(ReportDateFilter.ReportDateIdEnums)),
                Locations = LookUpBo.GetList<LookUpLocationDm>(),
                LocationEnums = EnumHelper.GetEnumIntList(typeof(LookUpLocationDm.LookUpIds)),
                Users = UserBo.GetSearchDropDownList(),
                TodayDate = DateTime.Now.Date,
            };

            return JsonModel(model);
        }

        [HttpPost]
        public ContentResult SaveList(List<ExpenseDm> expenses)
        {
            if (!ModelState.IsValid)
            {
                return JsonModelState(ModelState);
            }

            UserHelper.CheckPermissionForEntities(expenses, (int)LookUpPermissionDm.LookUpIds.Expense);

            List<ExpenseDm> trackedExpenses = ExpenseBo.GetListByIds(expenses.Where(a => a.Id > 0).Select(a => a.Id).ToList(), true).ToList();

            foreach (ExpenseDm expense in expenses)
            {
                ExpenseDm trackedExpense = (expense.Id > 0) ? trackedExpenses.Find(a => a.Id == expense.Id) : new ExpenseDm();

                Mapper.Map(expense, trackedExpense);

                if (expense.Id == 0)
                {
                    ExpenseBo.Add(trackedExpense);
                }
                else if (expense.Deleted)
                {
                    ExpenseBo.Remove(trackedExpense);
                }
            }

            ExpenseBo.Commit();

            return JsonModel(null);
        }
    }
}

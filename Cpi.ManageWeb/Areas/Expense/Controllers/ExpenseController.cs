using AutoMapper;
using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using Cpi.Application.BusinessObjects.LookUp;
using Cpi.Application.DataModels.LookUp;
using System.IO;
using System.Data;
using Cpi.Application.Models;
using Cpi.Application.Helpers;
using Cpi.Application.BusinessObjects.Other;

namespace Cpi.ManageWeb.Areas.Expense.Controllers
{
    [CpiAuthenticate((int)LookUpUserRoleDm.LookUpIds.老子, (int)LookUpUserRoleDm.LookUpIds.Admin)]
    public class ExpenseController : BaseController
    {
        private ExpenseBo ExpenseBo;
        private FinanceBo FinanceBo;
        private LookUpBo LookUpBo;
        public ExpenseController(ExpenseBo ExpenseBo, FinanceBo FinanceBo, LookUpBo LookUpBo)
        {
            this.ExpenseBo = ExpenseBo;
            this.FinanceBo = FinanceBo;
            this.LookUpBo = LookUpBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(ListFilter.Expense filter)
        {
            IQueryable<ExpenseDm> query = ExpenseBo.GetListBaseQuery(filter);
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
            List<ExpenseDm> records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortColumn, filter.SortDesc).ToList();
            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        }

        [HttpGet]
        public ContentResult GetListData()
        {
            var model = new
            {
                ReportDates = ReportDateFilter.GetSelectList(),
                ReportDateIdEnums = EnumHelper.GetEnumIntList(typeof(ReportDateFilter.ReportDateIdEnums)),
                Locations = LookUpBo.GetList<LookUpLocationDm>()
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

            List<ExpenseDm> trackedExpenses = ExpenseBo.GetListByIds(expenses.Where(a => a.Id > 0).Select(a => a.Id).ToList(), true).ToList();

            foreach (ExpenseDm expense in expenses)
            {
                ExpenseDm trackedExpense = (expense.Id > 0) ? trackedExpenses.Find(a => a.Id == expense.Id) : new ExpenseDm();

                Mapper.Map(expense, trackedExpense);

                if (trackedExpense.Id > 0)
                {
                    SetModified(trackedExpense);
                }
                else
                {
                    SetCreated(trackedExpense);
                    ExpenseBo.Add(trackedExpense);
                }
            }

            ExpenseBo.Commit();

            return JsonModel(null);
        }
    }
}

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

namespace Cpi.ManageWeb.Areas.Expense.Controllers
{
    [CpiAuthenticate]
    public class ExpenseController : BaseController
    {
        private ExpenseBo ExpenseBo;
        public ExpenseController(ExpenseBo ExpenseBo)
        {
            this.ExpenseBo = ExpenseBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ContentResult GetList(ListFilter.Expense filter)
        //{
        //    IQueryable<ExpenseDm> query = ExpenseBo.GetListBaseQuery(filter).Include(a => a.Status).Include(a => a.ExpenseCommodities.Select(b => b.Commodity));
        //    ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
        //    List<ExpenseDm> records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortColumn, filter.SortDesc).ToList();
        //    return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        //}

        //[HttpGet]
        //public ContentResult GetListData()
        //{
        //    var model = new
        //    {
        //        Commodities = LookUpBo.GetList<LookUpCommodityDm>().ToList(),
        //        ExpenseStatuses = LookUpBo.GetList<LookUpExpenseStatusDm>().ToList(),
        //        Users = UserBo.GetListQuery().OrderBy(a => a.Nickname).ThenBy(a => a.Fullname).Select(a => new CpiSelectListItem
        //        {
        //            Id = a.Id,
        //            Name = a.Nickname + " (" + a.Fullname + ")"
        //        }).ToList(),
        //        ExpenseStatusIdEnums = EnumHelper.GetEnumIntList(typeof(LookUpExpenseStatusDm.LookUpIds))
        //    };

        //    return JsonModel(model);
        //}

        //[HttpPost]
        //public ContentResult SaveList(List<ExpenseDm> expenses)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return JsonModelState(ModelState);
        //    }

        //    List<ExpenseDm> trackedExpenses = ExpenseBo.GetListByIds(expenses.Where(a => a.Id > 0).Select(a => a.Id).ToList(), true).ToList();
        //    List<LookUpCommodityDm> allCommodities = LookUpBo.GetList<LookUpCommodityDm>();

        //    foreach (ExpenseDm expense in expenses)
        //    {
        //        ExpenseDm trackedExpense = (expense.Id > 0) ? trackedExpenses.Find(a => a.Id == expense.Id) : new ExpenseDm();

        //        Mapper.Map(expense, trackedExpense);

        //        expense.ExpenseCommodities = (expense.ExpenseCommodities) ?? new List<ExpenseCommodityDm>();
        //        trackedExpense.ExpenseCommodities = (trackedExpense.ExpenseCommodities) ?? new List<ExpenseCommodityDm>();

        //        // save each expenseCommodity
        //        foreach (ExpenseCommodityDm expenseCommodity in expense.ExpenseCommodities)
        //        {
        //            ExpenseCommodityDm trackedExpenseCommodity = (expenseCommodity.Id > 0) ? trackedExpense.ExpenseCommodities.Find(a => a.Id == expenseCommodity.Id) : new ExpenseCommodityDm();

        //            Mapper.Map(expenseCommodity, trackedExpenseCommodity);

        //            if (trackedExpenseCommodity.Id > 0)
        //            {
        //                if (trackedExpenseCommodity.Quantity == 0) // deleting
        //                {
        //                    ExpenseCommodityBo.Remove(trackedExpenseCommodity);
        //                }
        //                else
        //                {
        //                    SetModified(trackedExpenseCommodity);
        //                }
        //            }
        //            else
        //            {
        //                SetCreated(trackedExpenseCommodity);
        //                trackedExpense.ExpenseCommodities.Add(trackedExpenseCommodity);
        //            }
        //        }

        //        if (trackedExpense.Id > 0)
        //        {
        //            SetModified(trackedExpense);
        //        }
        //        else
        //        {
        //            SetCreated(trackedExpense);

        //            // once theres commodities enterable from the website to here: we will need to check if thers already commodity, if so add new commodities to existing
        //            if (!ExpenseBo.ExpenseWithPhoneExistsToday(trackedExpense.CustomerPhone))
        //            {
        //                ExpenseBo.Add(trackedExpense);
        //            }
        //        }


        //    }

        //    ExpenseBo.Commit();

        //    return JsonModel(null);
        //}
    }
}

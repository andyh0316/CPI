using AutoMapper;
using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.Filters;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Manage.Controllers
{
    [CpiAuthenticate((int)LookUpUserRoleDm.LookUpIds.老子, (int)LookUpUserRoleDm.LookUpIds.Admin)]
    public class CommodityController : BaseController
    {
        private CommodityBo CommodityBo;
        public CommodityController(CommodityBo CommodityBo)
        {
            this.CommodityBo = CommodityBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(ListFilter.Commodity filter)
        {
            IQueryable<CommodityDm> query = CommodityBo.GetListBaseQuery();
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
            List<CommodityDm> records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortObjects).ToList();
            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        }

        [HttpPost]
        public ContentResult SaveList(List<CommodityDm> Commodities)
        {
            if (!ModelState.IsValid)
            {
                return JsonModelState(ModelState);
            }

            List<CommodityDm> trackedCommodities = CommodityBo.GetListByIds(Commodities.Where(a => a.Id > 0).Select(a => a.Id).ToList(), true).ToList();

            foreach (CommodityDm commodity in Commodities)
            {
                CommodityDm trackedCommodity = (commodity.Id > 0) ? trackedCommodities.Find(a => a.Id == commodity.Id) : new CommodityDm();
                Mapper.Map(commodity, trackedCommodity);
            }

            CommodityBo.Commit();

            return JsonModel(null);
        }
    }
}

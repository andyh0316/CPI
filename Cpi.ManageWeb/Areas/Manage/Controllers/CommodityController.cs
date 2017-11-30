using AutoMapper;
using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
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
    [CpiAuthenticate((int)LookUpPermissionDm.LookUpIds.Commodity)]
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
        public ContentResult SaveList(List<CommodityDm> commodities)
        {
            if (!ModelState.IsValid)
            {
                return JsonModelState(ModelState);
            }

            UserHelper.CheckPermissionForEntities(commodities, (int)LookUpPermissionDm.LookUpIds.Commodity);

            List<CommodityDm> trackedCommodities = CommodityBo.GetListByIds(commodities.Where(a => a.Id > 0).Select(a => a.Id).ToList(), true).ToList();

            foreach (CommodityDm commodity in commodities)
            {
                CommodityDm trackedCommodity = (commodity.Id > 0) ? trackedCommodities.Find(a => a.Id == commodity.Id) : new CommodityDm();
                Mapper.Map(commodity, trackedCommodity);
            }

            CommodityBo.Commit();

            return JsonModel(null);
        }
    }
}

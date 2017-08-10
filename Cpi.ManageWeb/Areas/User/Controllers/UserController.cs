using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using Cpi.Application.DataTransferObjects;
using Cpi.Application.Filters;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.User.Controllers
{
    [CpiAuthenticate((int)LookUpUserRoleDm.LookUpIds.老子, (int)LookUpUserRoleDm.LookUpIds.Admin)]
    public class UserController : BaseController
    {
        private UserBo UserBo;
        public UserController(UserBo UserBo)
        {
            this.UserBo = UserBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(ListFilter.User filter)
        {
            IQueryable<UserDto> query = UserBo.GetListBaseQuery(filter);
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
            List<UserDto> records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortColumn, filter.SortDesc).ToList();
            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        }

        [HttpGet]
        public ContentResult GetListData()
        {
            var model = new
            {
            };

            return JsonModel(model);
        }

        [HttpGet]
        public ContentResult GetUser(int id)
        {
            UserDm user;
            if (id == 0)
            {
                user = new UserDm();
            }
            else
            {
                user = UserBo.GetById(id);
            }

            return JsonModel(user);
        }
    }
}

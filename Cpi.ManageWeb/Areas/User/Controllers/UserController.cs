﻿using AutoMapper;
using Cobro.Compass.Web.Attributes;
using Cpi.Application.BusinessObjects;
using Cpi.Application.BusinessObjects.LookUp;
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
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
            List<UserDto> records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, filter.SortColumn, filter.SortDesc).ToList();
            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        }

        [HttpGet]
        public ContentResult GetListData()
        {
            var model = new
            {
                Occupations = LookUpBo.GetList<LookUpUserOccupationDm>().ToList(),
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

        [HttpGet]
        public ContentResult GetUserData()
        {
            var model = new
            {
                Occupations = LookUpBo.GetList<LookUpUserOccupationDm>().ToList(),
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

            UserDm trackedUser = (user.Id == 0) ? new UserDm() : UserBo.GetById(user.Id);
            Mapper.Map(user, trackedUser);

            if (user.Id == 0)
            {
                UserBo.Add(user);
            }

            UserBo.Commit();

            return JsonModel(null);
        }

        [HttpGet]
        public ContentResult DeleteUser(int id)
        {
            UserDm trackedUser = UserBo.GetById(id);
            UserBo.Remove(trackedUser);
            UserBo.Commit();

            return JsonModel(null);
        }
    }
}

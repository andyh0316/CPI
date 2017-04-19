﻿using Cpi.Application.BusinessObjects;
using Cpi.ManageWeb.Controllers.Base;
using System.Web.Mvc;
using System.Linq;

namespace Cpi.ManageWeb.Controllers
{
    public class SearchDropDownController : BaseController
    {
        private UserBo UserBo;
        public SearchDropDownController(UserBo userBo)
        {
            UserBo = userBo;
        }

        [HttpGet]
        public ContentResult Users(string searchString)
        {
            var results = UserBo.SearchDropDownQuery(searchString).Select(a => new
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();

            return JsonModel(results);
        }
    }
}
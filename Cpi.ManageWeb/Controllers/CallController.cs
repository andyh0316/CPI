using Cpi.Application.BusinessObjects;
using Cpi.Application.DataModels;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Controllers
{
    public class CallController : BaseController
    {
        [Inject]
        public CallBo CallBo { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(int page = 1)
        {
            IQueryable<CallDm> query = CallBo.GetListQuery();
            Pagination pagination = new Pagination(page, query.Count());
            List<CallDm> records = GetPagedSortedQuery(query, pagination.Skip, pagination.Take, "Name", false).ToList();
            return JsonModel(new { Records = records, Pagination = pagination });
        }
    }
}

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

namespace Cpi.ManageWeb.Areas.Invoice.Controllers
{
    public class InvoiceController : BaseController
    {
        [Inject]
        public InvoiceBo InvoiceBo { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(int page = 1)
        {
            IQueryable<InvoiceDm> query = InvoiceBo.GetListQuery();
            Pagination pagination = new Pagination(page, query.Count());
            List<InvoiceDm> records = GetPagedSortedQuery(query, pagination.Skip, pagination.Take, "CustomerName", false).ToList();
            return JsonModel(new { Records = records, Pagination = pagination });
        }

        //[HttpGet]
        //public ContentResult GetImport()
        //{

        //}

        
    }
}

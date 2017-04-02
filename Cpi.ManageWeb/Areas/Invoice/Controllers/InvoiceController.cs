using Cpi.Application.BusinessObjects;
using Cpi.Application.DataModels;
using Cpi.Application.Filters;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Invoice.Controllers
{
    public class InvoiceController : BaseController
    {
        private InvoiceBo InvoiceBo;
        public InvoiceController(InvoiceBo invoiceBo)
        {
            InvoiceBo = invoiceBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList(ListFilter.Invoice filter)
        {
            IQueryable<InvoiceDm> query = InvoiceBo.GetListBaseQuery();
            ListLoadCalculator listLoadCalculator = new ListLoadCalculator(filter.Loads, query.Count());
            List<InvoiceDm> records = GetLoadedSortedQuery(query, listLoadCalculator.Skip, listLoadCalculator.Take, "CustomerName", false).ToList();
            return JsonModel(new { Records = records, ListLoadCalculator = listLoadCalculator });
        } 

        //[HttpGet]
        //public ContentResult GetImport()
        //{

        //}

        
    }
}

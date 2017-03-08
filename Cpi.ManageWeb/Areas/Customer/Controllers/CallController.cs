using Cpi.ManageWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Customer.Controllers
{
    public class CallController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetList()
        {

        }
    }
}

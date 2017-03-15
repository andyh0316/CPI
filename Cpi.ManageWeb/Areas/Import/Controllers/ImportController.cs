using Cpi.Application.DataModels;
using Cpi.ManageWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Import.Controllers
{
    public class ImportController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}

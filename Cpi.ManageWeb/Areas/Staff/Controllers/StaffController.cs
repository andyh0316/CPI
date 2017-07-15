using Cobro.Compass.Web.Attributes;
using Cpi.Application.DataModels.LookUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Areas.Staff.Controllers
{
    [CpiAuthenticate((int)LookUpUserRoleDm.LookUpIds.老子, (int)LookUpUserRoleDm.LookUpIds.Admin)]
    public class StaffController : Controller
    {
        //
        // GET: /Staff/Staff/

        public ActionResult Index()
        {
            return View();
        }

    }
}

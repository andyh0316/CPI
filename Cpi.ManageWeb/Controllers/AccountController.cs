using Cpi.Application.Helpers;
using Cpi.ManageWeb.Controllers.Base;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LogOff()
        {
            UserHelper.Logout();

            return RedirectToAction("Index", "Public");
        }

        public ContentResult ExtendSession()
        {
            return JsonModel(null);
        }
    }
}

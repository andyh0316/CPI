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

        public ContentResult ExtendSession()
        {
            return JsonModel(null);
        }
    }
}

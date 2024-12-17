using System.Web.Mvc;

namespace Project_64132989.Areas.Admin.Controllers
{
    [Authorize]
    public class Home64132989Controller : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}

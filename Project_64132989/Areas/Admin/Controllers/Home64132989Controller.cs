using System.Web.Mvc;
using Project_64132989.Models.Data;
using System.Linq;

namespace Project_64132989.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Home64132989Controller : Controller
    {
        private readonly Model64132989DbContext db = new Model64132989DbContext();

        public ActionResult Index()
        {
            return View();
        }
    }
}

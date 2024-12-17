using System.Web.Mvc;

namespace Project_64132989.Controllers
{
    [AllowAnonymous]
    public class Home64132989Controller : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Hash()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                ViewBag.HashResult = "Input cannot be empty.";
                return View();
            }

            string hashedInput = BCrypt.Net.BCrypt.HashPassword(input);
            ViewBag.HashResult = hashedInput;

            return View();
        }
    }
}
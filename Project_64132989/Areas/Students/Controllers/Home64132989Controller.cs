using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132989.Areas.Students.Controllers
{
    [Authorize(Roles = "Student")]
    public class Home64132989Controller : Controller
    {
        // GET: Students/Home64132989
        public ActionResult Index()
        {
            return View();
        }
    }
}
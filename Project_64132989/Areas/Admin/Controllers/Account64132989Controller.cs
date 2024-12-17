using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132989.Areas.Admin.Controllers
{
    [Authorize]
    public class Account64132989Controller : Controller
    {
        // GET: Admin/Account64132989
        public ActionResult Index()
        {
            return View();
        }
    }
}
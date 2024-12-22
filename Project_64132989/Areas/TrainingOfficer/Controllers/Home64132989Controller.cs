using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132989.Areas.TrainingOfficer.Controllers
{
    [Authorize(Roles = "TrainingOfficer")]
    public class Home64132989Controller : Controller
    {
        // GET: TrainingOfficer/Home64132989
        public ActionResult Index()
        {
            return View();
        }
    }
}
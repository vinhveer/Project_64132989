using Project_64132989.Models.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Net;
using System.Web.Mvc;

namespace Project_64132989.Controllers
{
    [Authorize]
    public class Profiles64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: Profiles64132989/Details/5
        public ActionResult Index()
        {
            string id = User.Identity.Name;

            Debug.WriteLine("User ID: " + id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // GET: Profiles64132989/Edit/5
        public ActionResult Edit()
        {
            string id = User.Identity.Name;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.Users, "user_id", "email", profile.user_id);
            return View(profile);
        }

        // POST: Profiles64132989/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,first_name,last_name,date_of_birth,gender,phone_number,address,avatar_path")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.Users, "user_id", "email", profile.user_id);
            return View(profile);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

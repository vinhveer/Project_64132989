using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_64132989.Models.Data;

namespace Project_64132989.Areas.TrainingOfficer.Controllers
{
    public class Cours64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: TrainingOfficer/Cours64132989
        public ActionResult Index()
        {
            var courses = db.Courses.Include(c => c.Cours1).Include(c => c.Department);
            return View(courses.ToList());
        }

        // GET: TrainingOfficer/Cours64132989/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            return View(cours);
        }

        // GET: TrainingOfficer/Cours64132989/Create
        public ActionResult Create()
        {
            ViewBag.prerequisite_course_id = new SelectList(db.Courses, "course_id", "course_name");
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name");
            return View();
        }

        // POST: TrainingOfficer/Cours64132989/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "course_id,course_name,description,credits,department_id,course_type,prerequisite_course_id,status")] Cours cours)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(cours);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.prerequisite_course_id = new SelectList(db.Courses, "course_id", "course_name", cours.prerequisite_course_id);
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", cours.department_id);
            return View(cours);
        }

        // GET: TrainingOfficer/Cours64132989/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            ViewBag.prerequisite_course_id = new SelectList(db.Courses, "course_id", "course_name", cours.prerequisite_course_id);
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", cours.department_id);
            return View(cours);
        }

        // POST: TrainingOfficer/Cours64132989/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "course_id,course_name,description,credits,department_id,course_type,prerequisite_course_id,status")] Cours cours)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cours).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.prerequisite_course_id = new SelectList(db.Courses, "course_id", "course_name", cours.prerequisite_course_id);
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", cours.department_id);
            return View(cours);
        }

        // GET: TrainingOfficer/Cours64132989/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            return View(cours);
        }

        // POST: TrainingOfficer/Cours64132989/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Cours cours = db.Courses.Find(id);
            db.Courses.Remove(cours);
            db.SaveChanges();
            return RedirectToAction("Index");
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

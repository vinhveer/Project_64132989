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
    public class AdminClasses64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: TrainingOfficer/AdminClasses64132989
        public ActionResult Index()
        {
            var adminClasses = db.AdminClasses.Include(a => a.Department).Include(a => a.Teacher);
            return View(adminClasses.ToList());
        }

        // GET: TrainingOfficer/AdminClasses64132989/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminClass adminClass = db.AdminClasses.Find(id);
            if (adminClass == null)
            {
                return HttpNotFound();
            }
            return View(adminClass);
        }

        // GET: TrainingOfficer/AdminClasses64132989/Create
        public ActionResult Create()
        {
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name");
            ViewBag.advisor_teacher_id = new SelectList(db.Teachers, "user_id", "department_id");
            return View();
        }

        // POST: TrainingOfficer/AdminClasses64132989/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "class_id,class_name,department_id,advisor_teacher_id,created_date,status")] AdminClass adminClass)
        {
            if (ModelState.IsValid)
            {
                db.AdminClasses.Add(adminClass);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", adminClass.department_id);
            ViewBag.advisor_teacher_id = new SelectList(db.Teachers, "user_id", "department_id", adminClass.advisor_teacher_id);
            return View(adminClass);
        }

        // GET: TrainingOfficer/AdminClasses64132989/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminClass adminClass = db.AdminClasses.Find(id);
            if (adminClass == null)
            {
                return HttpNotFound();
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", adminClass.department_id);
            ViewBag.advisor_teacher_id = new SelectList(db.Teachers, "user_id", "department_id", adminClass.advisor_teacher_id);
            return View(adminClass);
        }

        // POST: TrainingOfficer/AdminClasses64132989/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "class_id,class_name,department_id,advisor_teacher_id,created_date,status")] AdminClass adminClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adminClass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", adminClass.department_id);
            ViewBag.advisor_teacher_id = new SelectList(db.Teachers, "user_id", "department_id", adminClass.advisor_teacher_id);
            return View(adminClass);
        }

        // GET: TrainingOfficer/AdminClasses64132989/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminClass adminClass = db.AdminClasses.Find(id);
            if (adminClass == null)
            {
                return HttpNotFound();
            }
            return View(adminClass);
        }

        // POST: TrainingOfficer/AdminClasses64132989/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AdminClass adminClass = db.AdminClasses.Find(id);
            db.AdminClasses.Remove(adminClass);
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

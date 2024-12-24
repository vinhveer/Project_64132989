using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_64132989.Models.Data;

namespace Project_64132989.Areas.Students.Controllers
{
    public class StudentCourseRegistrations64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: Students/StudentCourseRegistrations64132989
        public ActionResult Index()
        {
            var studentCourseRegistrations = db.StudentCourseRegistrations.Include(s => s.CourseOffering).Include(s => s.Student);
            return View(studentCourseRegistrations.ToList());
        }

        // GET: Students/StudentCourseRegistrations64132989/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentCourseRegistration studentCourseRegistration = db.StudentCourseRegistrations.Find(id);
            if (studentCourseRegistration == null)
            {
                return HttpNotFound();
            }
            return View(studentCourseRegistration);
        }

        // GET: Students/StudentCourseRegistrations64132989/Create
        public ActionResult Create()
        {
            ViewBag.offering_id = new SelectList(db.CourseOfferings, "offering_id", "course_id");
            ViewBag.student_id = new SelectList(db.Students, "user_id", "program_id");
            return View();
        }

        // POST: Students/StudentCourseRegistrations64132989/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "registration_id,student_id,offering_id,registration_date,status")] StudentCourseRegistration studentCourseRegistration)
        {
            if (ModelState.IsValid)
            {
                db.StudentCourseRegistrations.Add(studentCourseRegistration);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.offering_id = new SelectList(db.CourseOfferings, "offering_id", "course_id", studentCourseRegistration.offering_id);
            ViewBag.student_id = new SelectList(db.Students, "user_id", "program_id", studentCourseRegistration.student_id);
            return View(studentCourseRegistration);
        }

        // GET: Students/StudentCourseRegistrations64132989/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentCourseRegistration studentCourseRegistration = db.StudentCourseRegistrations.Find(id);
            if (studentCourseRegistration == null)
            {
                return HttpNotFound();
            }
            ViewBag.offering_id = new SelectList(db.CourseOfferings, "offering_id", "course_id", studentCourseRegistration.offering_id);
            ViewBag.student_id = new SelectList(db.Students, "user_id", "program_id", studentCourseRegistration.student_id);
            return View(studentCourseRegistration);
        }

        // POST: Students/StudentCourseRegistrations64132989/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "registration_id,student_id,offering_id,registration_date,status")] StudentCourseRegistration studentCourseRegistration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentCourseRegistration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.offering_id = new SelectList(db.CourseOfferings, "offering_id", "course_id", studentCourseRegistration.offering_id);
            ViewBag.student_id = new SelectList(db.Students, "user_id", "program_id", studentCourseRegistration.student_id);
            return View(studentCourseRegistration);
        }

        // GET: Students/StudentCourseRegistrations64132989/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentCourseRegistration studentCourseRegistration = db.StudentCourseRegistrations.Find(id);
            if (studentCourseRegistration == null)
            {
                return HttpNotFound();
            }
            return View(studentCourseRegistration);
        }

        // POST: Students/StudentCourseRegistrations64132989/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            StudentCourseRegistration studentCourseRegistration = db.StudentCourseRegistrations.Find(id);
            db.StudentCourseRegistrations.Remove(studentCourseRegistration);
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

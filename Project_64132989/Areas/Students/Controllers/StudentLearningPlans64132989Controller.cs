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
    public class StudentLearningPlans64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: Students/StudentLearningPlans64132989
        public ActionResult Index()
        {
            var studentLearningPlans = db.StudentLearningPlans.Include(s => s.Cours).Include(s => s.Semester).Include(s => s.Student);
            return View(studentLearningPlans.ToList());
        }

        // GET: Students/StudentLearningPlans64132989/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentLearningPlan studentLearningPlan = db.StudentLearningPlans.Find(id);
            if (studentLearningPlan == null)
            {
                return HttpNotFound();
            }
            return View(studentLearningPlan);
        }

        // GET: Students/StudentLearningPlans64132989/Create
        public ActionResult Create()
        {
            ViewBag.course_id = new SelectList(db.Courses, "course_id", "course_name");
            ViewBag.semester_id = new SelectList(db.Semesters, "semester_id", "semester_name");
            ViewBag.student_id = new SelectList(db.Students, "user_id", "program_id");
            return View();
        }

        // POST: Students/StudentLearningPlans64132989/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "learning_plan_id,course_id,semester_id,student_id,planned_date")] StudentLearningPlan studentLearningPlan)
        {
            if (ModelState.IsValid)
            {
                db.StudentLearningPlans.Add(studentLearningPlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.course_id = new SelectList(db.Courses, "course_id", "course_name", studentLearningPlan.course_id);
            ViewBag.semester_id = new SelectList(db.Semesters, "semester_id", "semester_name", studentLearningPlan.semester_id);
            ViewBag.student_id = new SelectList(db.Students, "user_id", "program_id", studentLearningPlan.student_id);
            return View(studentLearningPlan);
        }

        // GET: Students/StudentLearningPlans64132989/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentLearningPlan studentLearningPlan = db.StudentLearningPlans.Find(id);
            if (studentLearningPlan == null)
            {
                return HttpNotFound();
            }
            ViewBag.course_id = new SelectList(db.Courses, "course_id", "course_name", studentLearningPlan.course_id);
            ViewBag.semester_id = new SelectList(db.Semesters, "semester_id", "semester_name", studentLearningPlan.semester_id);
            ViewBag.student_id = new SelectList(db.Students, "user_id", "program_id", studentLearningPlan.student_id);
            return View(studentLearningPlan);
        }

        // POST: Students/StudentLearningPlans64132989/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "learning_plan_id,course_id,semester_id,student_id,planned_date")] StudentLearningPlan studentLearningPlan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentLearningPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.course_id = new SelectList(db.Courses, "course_id", "course_name", studentLearningPlan.course_id);
            ViewBag.semester_id = new SelectList(db.Semesters, "semester_id", "semester_name", studentLearningPlan.semester_id);
            ViewBag.student_id = new SelectList(db.Students, "user_id", "program_id", studentLearningPlan.student_id);
            return View(studentLearningPlan);
        }

        // GET: Students/StudentLearningPlans64132989/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentLearningPlan studentLearningPlan = db.StudentLearningPlans.Find(id);
            if (studentLearningPlan == null)
            {
                return HttpNotFound();
            }
            return View(studentLearningPlan);
        }

        // POST: Students/StudentLearningPlans64132989/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            StudentLearningPlan studentLearningPlan = db.StudentLearningPlans.Find(id);
            db.StudentLearningPlans.Remove(studentLearningPlan);
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

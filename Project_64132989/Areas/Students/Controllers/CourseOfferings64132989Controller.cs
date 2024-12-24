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
    public class CourseOfferings64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: Students/CourseOfferings64132989
        public ActionResult Index()
        {
            var courseOfferings = db.CourseOfferings.Include(c => c.Cours).Include(c => c.Room).Include(c => c.Semester).Include(c => c.Teacher);
            return View(courseOfferings.ToList());
        }

        // GET: Students/CourseOfferings64132989/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseOffering courseOffering = db.CourseOfferings.Find(id);
            if (courseOffering == null)
            {
                return HttpNotFound();
            }
            return View(courseOffering);
        }

        // GET: Students/CourseOfferings64132989/Create
        public ActionResult Create()
        {
            ViewBag.course_id = new SelectList(db.Courses, "course_id", "course_name");
            ViewBag.room_id = new SelectList(db.Rooms, "room_id", "room_name");
            ViewBag.semester_id = new SelectList(db.Semesters, "semester_id", "semester_name");
            ViewBag.teacher_user_id = new SelectList(db.Teachers, "user_id", "department_id");
            return View();
        }

        // POST: Students/CourseOfferings64132989/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "offering_id,course_id,semester_id,teacher_user_id,room_id,max_capacity,status")] CourseOffering courseOffering)
        {
            if (ModelState.IsValid)
            {
                db.CourseOfferings.Add(courseOffering);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.course_id = new SelectList(db.Courses, "course_id", "course_name", courseOffering.course_id);
            ViewBag.room_id = new SelectList(db.Rooms, "room_id", "room_name", courseOffering.room_id);
            ViewBag.semester_id = new SelectList(db.Semesters, "semester_id", "semester_name", courseOffering.semester_id);
            ViewBag.teacher_user_id = new SelectList(db.Teachers, "user_id", "department_id", courseOffering.teacher_user_id);
            return View(courseOffering);
        }

        // GET: Students/CourseOfferings64132989/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseOffering courseOffering = db.CourseOfferings.Find(id);
            if (courseOffering == null)
            {
                return HttpNotFound();
            }
            ViewBag.course_id = new SelectList(db.Courses, "course_id", "course_name", courseOffering.course_id);
            ViewBag.room_id = new SelectList(db.Rooms, "room_id", "room_name", courseOffering.room_id);
            ViewBag.semester_id = new SelectList(db.Semesters, "semester_id", "semester_name", courseOffering.semester_id);
            ViewBag.teacher_user_id = new SelectList(db.Teachers, "user_id", "department_id", courseOffering.teacher_user_id);
            return View(courseOffering);
        }

        // POST: Students/CourseOfferings64132989/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "offering_id,course_id,semester_id,teacher_user_id,room_id,max_capacity,status")] CourseOffering courseOffering)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseOffering).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.course_id = new SelectList(db.Courses, "course_id", "course_name", courseOffering.course_id);
            ViewBag.room_id = new SelectList(db.Rooms, "room_id", "room_name", courseOffering.room_id);
            ViewBag.semester_id = new SelectList(db.Semesters, "semester_id", "semester_name", courseOffering.semester_id);
            ViewBag.teacher_user_id = new SelectList(db.Teachers, "user_id", "department_id", courseOffering.teacher_user_id);
            return View(courseOffering);
        }

        // GET: Students/CourseOfferings64132989/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseOffering courseOffering = db.CourseOfferings.Find(id);
            if (courseOffering == null)
            {
                return HttpNotFound();
            }
            return View(courseOffering);
        }

        // POST: Students/CourseOfferings64132989/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CourseOffering courseOffering = db.CourseOfferings.Find(id);
            db.CourseOfferings.Remove(courseOffering);
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

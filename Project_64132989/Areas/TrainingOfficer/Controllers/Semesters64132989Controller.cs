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
    public class Semesters64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: TrainingOfficer/Semesters64132989
        public ActionResult Index()
        {
            return View(db.Semesters.ToList());
        }

        // GET: TrainingOfficer/Semesters64132989/ActiveSemester
        public ActionResult ActiveSemester()
        {
            // Lấy học kỳ hiện tại
            var currentSemester = db.Semesters
                .Where(s => s.status == 1)
                .OrderByDescending(s => s.semester_id)
                .FirstOrDefault();

            // Lấy danh sách tất cả học kỳ cho dropdown
            ViewBag.AllSemesters = db.Semesters
                .OrderByDescending(s => s.semester_id)
                .ToList();

            return View(currentSemester);
        }

        [HttpPost]
        public ActionResult ActiveSemester(int semester_id)
        {
            try
            {
                // Cập nhật tất cả học kỳ thành không hoạt động
                var allSemesters = db.Semesters.ToList();
                foreach (var semester in allSemesters)
                {
                    semester.status = 0;
                }

                // Cập nhật học kỳ được chọn thành hoạt động
                var selectedSemester = db.Semesters.Find(semester_id);
                if (selectedSemester != null)
                {
                    selectedSemester.status = 1;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không tìm thấy học kỳ" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetSemesterList(int offset, int limit, string search, string sort, string order)
        {
            var query = db.Semesters.AsQueryable();

            // Xử lý tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s =>
                    s.semester_name.ToLower().Contains(search) ||
                    s.semester_id.ToString().Contains(search));
            }

            // Đếm tổng số bản ghi thỏa điều kiện
            int total = query.Count();

            // Xử lý sắp xếp
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "semesterId":
                        query = order == "asc" ? query.OrderBy(s => s.semester_id) : query.OrderByDescending(s => s.semester_id);
                        break;
                    case "semesterName":
                        query = order == "asc" ? query.OrderBy(s => s.semester_name) : query.OrderByDescending(s => s.semester_name);
                        break;
                    case "registrationStartDate":
                        query = order == "asc" ? query.OrderBy(s => s.registration_start_date) : query.OrderByDescending(s => s.registration_start_date);
                        break;
                    case "registrationEndDate":
                        query = order == "asc" ? query.OrderBy(s => s.registration_end_date) : query.OrderByDescending(s => s.registration_end_date);
                        break;
                    case "status":
                        query = order == "asc" ? query.OrderBy(s => s.status) : query.OrderByDescending(s => s.status);
                        break;
                    case "courseRegistrationStart":
                        query = order == "asc" ? query.OrderBy(s => s.course_registration_start) : query.OrderByDescending(s => s.course_registration_start);
                        break;
                    case "courseRegistrationEnd":
                        query = order == "asc" ? query.OrderBy(s => s.course_registration_end) : query.OrderByDescending(s => s.course_registration_end);
                        break;
                }
            }

            // Phân trang và chọn dữ liệu
            var semesters = query
                .Skip(offset)
                .Take(limit)
                .Select(s => new
                {
                    semesterId = s.semester_id,
                    semesterName = s.semester_name,
                    registrationStartDate = s.registration_start_date,
                    registrationEndDate = s.registration_end_date,
                    status = s.status,
                    courseRegistrationStart = s.course_registration_start,
                    courseRegistrationEnd = s.course_registration_end
                })
                .ToList();

            return Json(new
            {
                total = total,
                rows = semesters
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: TrainingOfficer/Semesters64132989/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Semester semester = db.Semesters.Find(id);
            if (semester == null)
            {
                return HttpNotFound();
            }
            return View(semester);
        }

        // GET: TrainingOfficer/Semesters64132989/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingOfficer/Semesters64132989/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "semester_id,semester_name,registration_start_date,registration_end_date,status,course_registration_start,course_registration_end")] Semester semester)
        {
            if (ModelState.IsValid)
            {
                db.Semesters.Add(semester);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(semester);
        }

        // GET: TrainingOfficer/Semesters64132989/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Semester semester = db.Semesters.Find(id);
            if (semester == null)
            {
                return HttpNotFound();
            }
            return View(semester);
        }

        // POST: TrainingOfficer/Semesters64132989/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "semester_id,semester_name,registration_start_date,registration_end_date,status,course_registration_start,course_registration_end")] Semester semester)
        {
            if (ModelState.IsValid)
            {
                db.Entry(semester).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(semester);
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

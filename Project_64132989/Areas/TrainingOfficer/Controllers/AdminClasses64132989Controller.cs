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

        [HttpPost]
        public JsonResult GetAdminClassList(int offset, int limit, string search, string sort, string order)
        {
            try
            {
                if (offset < 0 || limit <= 0)
                {
                    return Json(new { success = false, message = "Tham số phân trang không hợp lệ" });
                }

                var query = db.AdminClasses
                    .Include(a => a.Department)
                    .Include(a => a.Teacher)
                    .Include(a => a.Teacher.User)
                    .Include(a => a.Teacher.User.Profile)
                    .AsQueryable();

                // Tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(a =>
                        a.class_id.ToLower().Contains(search) ||
                        a.class_name.ToLower().Contains(search) ||
                        (a.Department != null && a.Department.department_name.ToLower().Contains(search)) ||
                        (a.Teacher != null && a.Teacher.User != null && a.Teacher.User.Profile != null &&
                            (a.Teacher.User.Profile.first_name + " " + a.Teacher.User.Profile.last_name)
                            .ToLower().Contains(search))
                    );
                }

                // Đếm tổng số bản ghi
                int total = query.Count();

                // Sắp xếp
                if (!string.IsNullOrEmpty(sort))
                {
                    switch (sort)
                    {
                        case "classId":
                            query = order == "asc" ? query.OrderBy(a => a.class_id) : query.OrderByDescending(a => a.class_id);
                            break;
                        case "className":
                            query = order == "asc" ? query.OrderBy(a => a.class_name) : query.OrderByDescending(a => a.class_name);
                            break;
                        case "departmentName":
                            query = order == "asc"
                                ? query.OrderBy(a => a.Department != null ? a.Department.department_name : "")
                                : query.OrderByDescending(a => a.Department != null ? a.Department.department_name : "");
                            break;
                        case "advisorTeacher":
                            query = order == "asc"
                                ? query.OrderBy(a => a.Teacher.User.Profile.first_name)
                                : query.OrderByDescending(a => a.Teacher.User.Profile.first_name);
                            break;
                        case "createdDate":
                            query = order == "asc" ? query.OrderBy(a => a.created_date) : query.OrderByDescending(a => a.created_date);
                            break;
                        case "status":
                            query = order == "asc" ? query.OrderBy(a => a.status) : query.OrderByDescending(a => a.status);
                            break;
                        default:
                            query = order == "asc" ? query.OrderBy(a => a.class_id) : query.OrderByDescending(a => a.class_id);
                            break;
                    }
                }

                // Thực thi query và lấy dữ liệu
                var adminClasses = query.Skip(offset).Take(limit).ToList();

                // Xử lý và chuyển đổi dữ liệu sau khi đã lấy từ database
                var classes = adminClasses.Select(a => new
                {
                    state = false,
                    classId = a.class_id,
                    className = a.class_name,
                    departmentName = a.Department?.department_name ?? "Không có khoa",
                    advisorTeacher = a.Teacher?.User?.Profile != null
                        ? $"{a.Teacher.User.Profile.first_name} {a.Teacher.User.Profile.last_name}"
                        : "Chưa phân công",
                    createdDate = a.created_date?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Chưa cập nhật",
                    status = a.status == 1 ? "Hoạt động" : "Không hoạt động",
                    actions = ""
                }).ToList();

                return Json(new
                {
                    total = total,
                    rows = classes,
                    success = true,
                    currentUser = "vinhveer",
                    currentDate = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi tải dữ liệu: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
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

            ViewBag.TeacherName = adminClass.Teacher.User.Profile.first_name + " " + adminClass.Teacher.User.Profile.last_name;
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

        [HttpPost]
        public JsonResult Delete(string classId)
        {
            try
            {
                var adminClass = db.AdminClasses.Find(classId);
                if (adminClass == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy lớp" });
                }

                // Kiểm tra xem lớp có sinh viên không
                if (adminClass.Students.Any())
                {
                    return Json(new { success = false, message = "Không thể xóa lớp đang có sinh viên" });
                }

                db.AdminClasses.Remove(adminClass);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteMultiple(string[] classIds)
        {
            try
            {
                if (classIds == null || !classIds.Any())
                {
                    return Json(new { success = false, message = "Không có lớp nào được chọn" });
                }

                var classesToDelete = db.AdminClasses.Where(c => classIds.Contains(c.class_id)).ToList();

                // Kiểm tra xem có lớp nào đang có sinh viên không
                foreach (var adminClass in classesToDelete)
                {
                    if (adminClass.Students.Any())
                    {
                        return Json(new { success = false, message = $"Lớp {adminClass.class_name} đang có sinh viên, không thể xóa" });
                    }
                }

                db.AdminClasses.RemoveRange(classesToDelete);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
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

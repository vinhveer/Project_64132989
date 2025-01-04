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
    [Authorize(Roles = "TrainingOfficer")]
    public class TeacherAssignments64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            // Lấy danh sách giảng viên được phân công cho học phần này
            var assignments = db.TeacherAssignments
                .Where(t => t.course_id == id)
                .Include(t => t.Teacher.User.Profile)
                .Include(t => t.Teacher.Department)
                .ToList();

            ViewBag.CourseId = id;
            ViewBag.CourseName = db.Courses
                .Where(c => c.course_id == id)
                .Select(c => c.course_name)
                .FirstOrDefault();

            return View(assignments);
        }

        [HttpPost]
        public JsonResult Create(TeacherAssignment assignment)
        {
            try
            {
                // Kiểm tra trùng lặp
                var exists = db.TeacherAssignments.Any(t =>
                    t.teacher_id == assignment.teacher_id &&
                    t.course_id == assignment.course_id);

                if (exists)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Giảng viên này đã được phân công cho học phần này"
                    });
                }

                db.TeacherAssignments.Add(assignment);
                db.SaveChanges();

                // Lấy thông tin giảng viên vừa được phân công để trả về
                var teacherInfo = db.Teachers
                    .Where(t => t.user_id == assignment.teacher_id)
                    .Select(t => new {
                        t.user_id,
                        fullName = t.User.Profile.last_name + " " + t.User.Profile.first_name,
                        department = t.Department.department_name
                    })
                    .FirstOrDefault();

                return Json(new
                {
                    success = true,
                    message = "Thêm phân công giảng dạy thành công",
                    data = teacherInfo
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(long id)
        {
            try
            {
                var assignment = db.TeacherAssignments.Find(id);
                if (assignment == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Không tìm thấy phân công giảng dạy"
                    });
                }

                db.TeacherAssignments.Remove(assignment);
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Xóa phân công giảng dạy thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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

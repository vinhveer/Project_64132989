using Project_64132989.Models.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Project_64132989.Areas.TrainingOfficer.Controllers
{
    [Authorize(Roles = "TrainingOfficer")]
    public class StudentLearningPlans64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: Students/StudentLearningPlans64132989
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetSemesters()
        {
            var semesters = db.Semesters
                .OrderByDescending(s => s.semester_id)
                .Select(s => new
                {
                    id = s.semester_id,
                    name = s.semester_name
                })
                .ToList();

            return Json(semesters, JsonRequestBehavior.AllowGet);
        }

        [HttpGet] // Using GET for easier debugging as requested
        public JsonResult GetStudentLearningPlanList(int offset, int limit, string search, string sort, string order, int? semesterId = null)
        {
            var query = db.StudentLearningPlans
                .Include(s => s.Student)
                .Include(s => s.Cours)
                .Include(s => s.Semester)
                .AsQueryable();

            // Thêm filter theo học kỳ
            if (semesterId.HasValue)
            {
                query = query.Where(s => s.semester_id == semesterId.Value);
            }

            // Search functionality
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s =>
                    s.student_id.ToLower().Contains(search) ||
                    s.course_id.ToLower().Contains(search) ||
                    s.Cours.course_name.ToLower().Contains(search) ||
                    s.Semester.semester_name.ToLower().Contains(search));
            }

            int total = query.Count();

            // Sorting
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "studentId":
                        query = order == "asc" ? query.OrderBy(s => s.student_id) : query.OrderByDescending(s => s.student_id);
                        break;
                    case "courseId":
                        query = order == "asc" ? query.OrderBy(s => s.course_id) : query.OrderByDescending(s => s.course_id);
                        break;
                    case "courseName":
                        query = order == "asc" ? query.OrderBy(s => s.Cours.course_name) : query.OrderByDescending(s => s.Cours.course_name);
                        break;
                    case "semesterName":
                        query = order == "asc" ? query.OrderBy(s => s.Semester.semester_name) : query.OrderByDescending(s => s.Semester.semester_name);
                        break;
                    case "plannedDate":
                        query = order == "asc" ? query.OrderBy(s => s.planned_date) : query.OrderByDescending(s => s.planned_date);
                        break;
                }
            }

            // Pagination and data selection
            var learningPlans = query
                .Skip(offset)
                .Take(limit)
                .Select(s => new
                {
                    learningPlanId = s.learning_plan_id,
                    studentId = s.student_id,
                    courseId = s.course_id,
                    courseName = s.Cours.course_name,
                    credits = s.Cours.credits,
                    semesterId = s.semester_id,
                    semesterName = s.Semester.semester_name,
                    plannedDate = s.planned_date,
                    courseType = s.Cours.course_type,
                    prerequisiteCourseId = s.Cours.prerequisite_course_id
                })
                .ToList();

            return Json(new
            {
                total = total,
                rows = learningPlans
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCourses()
        {
            var courses = db.Courses
                .OrderBy(c => c.course_name)
                .Select(c => new
                {
                    id = c.course_id,
                    name = c.course_name,
                    credits = c.credits
                })
                .ToList();

            return Json(courses, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPlan(long? id)
        {
            if (!id.HasValue)
                return Json(null, JsonRequestBehavior.AllowGet);

            var plan = db.StudentLearningPlans
                .Select(p => new
                {
                    p.learning_plan_id,
                    p.student_id,
                    p.course_id,
                    p.Cours.course_name,
                    p.semester_id,
                    p.planned_date
                })
                .FirstOrDefault(p => p.learning_plan_id == id.Value);

            if (plan == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                learningPlanId = plan.learning_plan_id,
                studentId = plan.student_id,
                courseId = plan.course_id,
                courseName = plan.course_name,
                semesterId = plan.semester_id,
                plannedDate = plan.planned_date
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Create(string studentId, List<string> courseIds, int semesterId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Kiểm tra sinh viên tồn tại
                    var student = db.Students
                        .AsNoTracking()
                        .FirstOrDefault(s => s.user_id == studentId);

                    if (student == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"Không tìm thấy sinh viên có mã {studentId}"
                        });
                    }

                    // Kiểm tra khóa học tồn tại
                    var existingCourses = db.Courses
                        .AsNoTracking()
                        .Where(c => courseIds.Contains(c.course_id))
                        .Select(c => c.course_id)
                        .ToList();

                    if (existingCourses.Count != courseIds.Count)
                    {
                        var nonExistingCourses = courseIds.Except(existingCourses);
                        return Json(new
                        {
                            success = false,
                            message = "Không tìm thấy khóa học: " + string.Join(", ", nonExistingCourses)
                        });
                    }

                    // Kiểm tra trùng lặp
                    var existingPlans = db.StudentLearningPlans
                        .AsNoTracking()
                        .Where(p => p.student_id == studentId && courseIds.Contains(p.course_id))
                        .Select(p => p.course_id)
                        .ToList();

                    if (existingPlans.Any())
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Các học phần sau đã có trong kế hoạch: " + string.Join(", ", existingPlans)
                        });
                    }

                    // Kiểm tra học kỳ tồn tại
                    var semester = db.Semesters
                        .AsNoTracking()
                        .FirstOrDefault(s => s.semester_id == semesterId);

                    if (semester == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Không tìm thấy học kỳ được chọn"
                        });
                    }
                    var currentDate = DateTime.UtcNow;

                    var plans = courseIds.Select((courseId, index) => new StudentLearningPlan
                    {
                        learning_plan_id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + index,
                        student_id = studentId,
                        course_id = courseId,
                        semester_id = semesterId,
                        planned_date = currentDate
                    }).ToList();

                    // Thêm từng plan một
                    foreach (var plan in plans)
                    {
                        db.StudentLearningPlans.Add(plan);
                    }

                    // Lưu thay đổi
                    db.SaveChanges();
                    transaction.Commit();

                    return Json(new
                    {
                        success = true,
                        message = "Đã thêm kế hoạch học tập thành công"
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new
                    {
                        success = false,
                        message = "Có lỗi xảy ra khi thêm kế hoạch học tập: " + ex.Message + ex.InnerException
                    });
                }
            }
        }

        [HttpPost]
        public JsonResult EditPlan(StudentLearningPlan model)
        {
            try
            {
                var plan = db.StudentLearningPlans.Find(model.learning_plan_id);
                if (plan == null)
                    return Json(new { success = false, message = "Không tìm thấy kế hoạch học tập" });

                // Chỉ cho phép thay đổi học kỳ
                plan.semester_id = model.semester_id;

                db.SaveChanges();
                return Json(new { success = true });
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
                var plan = db.StudentLearningPlans.Find(id);
                if (plan == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy kế hoạch học tập." });
                }

                db.StudentLearningPlans.Remove(plan);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra khi xóa kế hoạch học tập. {ex.Message}" });
            }
        }

        [HttpPost]
        public JsonResult DeleteMultiple(long[] ids)
        {
            try
            {
                var plans = db.StudentLearningPlans.Where(p => ids.Contains(p.learning_plan_id));
                db.StudentLearningPlans.RemoveRange(plans);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra khi xóa các kế hoạch học tập. {ex.Message}" });
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

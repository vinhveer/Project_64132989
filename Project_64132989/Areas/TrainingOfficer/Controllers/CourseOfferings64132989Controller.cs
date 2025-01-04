using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_64132989.Models.Data;

namespace Project_64132989.Areas.TrainingOfficer.Controllers
{
    [Authorize(Roles = "TrainingOfficer")]
    public class CourseOfferings64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: TrainingOfficer/CourseOfferings64132989
        public ActionResult Index()
        {
            var courseOfferings = db.CourseOfferings.Include(c => c.Cours).Include(c => c.Room).Include(c => c.Semester).Include(c => c.Teacher);
            return View(courseOfferings.ToList());
        }

        [HttpPost]
        public JsonResult CreateCourseOfferings(long semesterId, int maxStudentsPerClass)
        {
            try
            {
                var currentDate = DateTime.UtcNow; // 2024-12-29 20:24:08
                var currentUser = User.Identity.Name ?? "trittntu";

                // Kiểm tra học kỳ tồn tại
                var semester = db.Semesters.Find(semesterId);
                if (semester == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy học kỳ!" });
                }

                // Gọi stored procedure
                db.Database.ExecuteSqlCommand(
                    "EXEC [dbo].[CreateCourseOfferings] @semester_id, @max_students_per_class",
                    new SqlParameter("@semester_id", semesterId),
                    new SqlParameter("@max_students_per_class", maxStudentsPerClass)
                );

                return Json(new
                {
                    success = true,
                    message = "Đã tạo lớp học phần cho học kỳ thành công!"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: TrainingOfficer/CourseOfferings64132989/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy thông tin lớp học phần với Include các thông tin liên quan
            CourseOffering courseOffering = db.CourseOfferings
                .Include(c => c.Cours)
                .Include(c => c.Teacher.User.Profile)
                .Include(c => c.Room)
                .Include(c => c.Semester)
                .FirstOrDefault(c => c.offering_id == id);

            if (courseOffering == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách lịch học có sắp xếp
            var schedules = db.Schedules
                .Include(s => s.TimeSlot)
                .Where(s => s.offering_id == id)
                .OrderBy(s => s.day_of_week)
                .ThenBy(s => s.TimeSlot.start_time)
                .ToList();

            // Lấy danh sách tiết học để dùng trong dropdowns
            var timeSlots = db.TimeSlots
                .OrderBy(t => t.slot_id)
                .ToList(); // Lấy toàn bộ thông tin TimeSlot

            // Truyền dữ liệu qua ViewBag
            ViewBag.schedules = schedules;
            ViewBag.TimeSlots = timeSlots;

            // Thêm thông tin người dùng và thời gian hiện tại
            ViewBag.CurrentDate = DateTime.UtcNow; // 2024-12-29 19:57:39
            ViewBag.CurrentUser = User.Identity.Name ?? "trittntu";

            return View(courseOffering);
        }

        [HttpPost]
        public JsonResult GetCourseOfferingList(int offset, int limit, string search, string sort, string order, int? semesterId = null)
        {
            try
            {
                if (offset < 0 || limit <= 0)
                {
                    return Json(new { success = false, message = "Tham số phân trang không hợp lệ" });
                }

                var query = db.CourseOfferings
                    .Include(c => c.Cours)
                    .Include(c => c.Room)
                    .Include(c => c.Semester)
                    .Include(c => c.Teacher)
                    .AsQueryable();

                // Lọc theo học kỳ nếu có
                if (semesterId.HasValue)
                {
                    query = query.Where(c => c.semester_id == semesterId.Value);
                }

                // Tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(c =>
                        c.offering_id.ToString().Contains(search) ||
                        c.course_id.ToLower().Contains(search) ||
                        c.Cours.course_name.ToLower().Contains(search) ||
                        c.teacher_user_id.ToLower().Contains(search) ||
                        (c.Room != null && c.Room.room_name.ToLower().Contains(search))
                    );
                }

                // Đếm tổng số bản ghi
                int total = query.Count();

                // Sắp xếp
                if (!string.IsNullOrEmpty(sort))
                {
                    switch (sort)
                    {
                        case "offeringId":
                            query = order == "asc" ? query.OrderBy(c => c.offering_id) : query.OrderByDescending(c => c.offering_id);
                            break;
                        case "courseId":
                            query = order == "asc" ? query.OrderBy(c => c.course_id) : query.OrderByDescending(c => c.course_id);
                            break;
                        case "courseName":
                            query = order == "asc" ? query.OrderBy(c => c.Cours.course_name) : query.OrderByDescending(c => c.Cours.course_name);
                            break;
                        case "semesterName":
                            query = order == "asc" ? query.OrderBy(c => c.Semester.semester_name) : query.OrderByDescending(c => c.Semester.semester_name);
                            break;
                        case "teacherId":
                            query = order == "asc" ? query.OrderBy(c => c.teacher_user_id) : query.OrderByDescending(c => c.teacher_user_id);
                            break;
                        case "roomName":
                            query = order == "asc" ? query.OrderBy(c => c.Room.room_name) : query.OrderByDescending(c => c.Room.room_name);
                            break;
                        case "maxCapacity":
                            query = order == "asc" ? query.OrderBy(c => c.max_capacity) : query.OrderByDescending(c => c.max_capacity);
                            break;
                        case "status":
                            query = order == "asc" ? query.OrderBy(c => c.status) : query.OrderByDescending(c => c.status);
                            break;
                        default:
                            query = order == "asc" ? query.OrderBy(c => c.offering_id) : query.OrderByDescending(c => c.offering_id);
                            break;
                    }
                }

                // Phân trang và lấy dữ liệu
                var offerings = query.Skip(offset).Take(limit).ToList();

                // Chuyển đổi dữ liệu
                var offeringList = offerings.Select(c => new
                {
                    state = false,
                    offeringId = c.offering_id,
                    courseId = c.course_id,
                    courseName = c.Cours.course_name,
                    semesterId = c.semester_id,
                    semesterName = c.Semester.semester_name,
                    teacherId = c.teacher_user_id,
                    roomName = c.Room?.room_name ?? "Chưa phân phòng",
                    maxCapacity = c.max_capacity,
                    status = c.status == 1 ? "Hoạt động" : "Không hoạt động",
                    actions = ""
                }).ToList();

                return Json(new { total, rows = offeringList, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: TrainingOfficer/CourseOfferings64132989/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.semester_id = new SelectList(db.Semesters, "semester_id", "semester_name");
            ViewBag.room_id = new SelectList(db.Rooms, "room_id", "room_name");
            return View();
        }

        // POST: TrainingOfficer/CourseOfferings64132989/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(CourseOffering offering)
        {
            try
            {
                offering.status = 1;

                db.CourseOfferings.Add(offering);
                db.SaveChanges();

                return Json(new { success = true, message = "Tạo lớp học phần thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: TrainingOfficer/CourseOfferings64132989/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var courseOffering = db.CourseOfferings
                .Include(c => c.Cours)
                .Include(c => c.Teacher)
                .Include(c => c.Room)
                .FirstOrDefault(c => c.offering_id == id);

            if (courseOffering == null)
            {
                return HttpNotFound();
            }

            ViewBag.semester_id = new SelectList(db.Semesters, "semester_id", "semester_name", courseOffering.semester_id);
            ViewBag.room_id = new SelectList(db.Rooms, "room_id", "room_name", courseOffering.room_id);

            // Prepare initial select2 data
            ViewBag.InitialCourse = new
            {
                id = courseOffering.course_id,
                text = courseOffering.Cours.course_id + " - " + courseOffering.Cours.course_name
            };

            ViewBag.InitialTeacher = new
            {
                id = courseOffering.teacher_user_id,
                text = courseOffering.teacher_user_id + " - " + courseOffering.Teacher.User.Profile.last_name + " " + courseOffering.Teacher.User.Profile.first_name
            };

            return View(courseOffering);
        }

        // POST: TrainingOfficer/CourseOfferings64132989/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(CourseOffering courseOffering)
        {
            try
            {
                var existingOffering = db.CourseOfferings.Find(courseOffering.offering_id);
                if (existingOffering == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy lớp học phần" });
                }

                // Update the properties
                existingOffering.course_id = courseOffering.course_id;
                existingOffering.semester_id = courseOffering.semester_id;
                existingOffering.teacher_user_id = courseOffering.teacher_user_id;
                existingOffering.room_id = courseOffering.room_id;
                existingOffering.max_capacity = courseOffering.max_capacity;
                existingOffering.status = courseOffering.status;

                db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật lớp học phần thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: TrainingOfficer/CourseOfferings64132989/Delete/5
        [HttpPost]
        public JsonResult Delete(long id)
        {
            try
            {
                var courseOffering = db.CourseOfferings.Find(id);
                if (courseOffering == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy lớp học phần" });
                }

                // 1. Xóa StudentCourseRegistrations liên quan
                var registrations = db.StudentCourseRegistrations
                    .Where(r => r.offering_id == id);
                db.StudentCourseRegistrations.RemoveRange(registrations);

                // 2. Xóa Schedules liên quan
                var schedules = db.Schedules
                    .Where(s => s.offering_id == id);
                db.Schedules.RemoveRange(schedules);

                // 3. Xóa CourseOffering
                db.CourseOfferings.Remove(courseOffering);
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa lớp học phần thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: TrainingOfficer/CourseOfferings64132989/DeleteMultiple
        [HttpPost]
        public JsonResult DeleteMultiple(List<long> ids)
        {
            try
            {
                // 1. Xóa StudentCourseRegistrations liên quan
                var registrations = db.StudentCourseRegistrations
                    .Where(r => ids.Contains(r.offering_id));
                db.StudentCourseRegistrations.RemoveRange(registrations);

                // 2. Xóa Schedules liên quan
                var schedules = db.Schedules
                    .Where(s => ids.Contains(s.offering_id));
                db.Schedules.RemoveRange(schedules);

                // 3. Xóa CourseOfferings
                var offerings = db.CourseOfferings
                    .Where(c => ids.Contains(c.offering_id));
                db.CourseOfferings.RemoveRange(offerings);

                db.SaveChanges();

                return Json(new { success = true, message = "Xóa các lớp học phần thành công" });
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

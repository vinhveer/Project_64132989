using Project_64132989.Models.Data;
using System;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Project_64132989.Areas.Students.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentCourseRegistrations64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: Students/StudentCourseRegistrations64132989
        public ActionResult Index()
        {
            var semester_id = Session["SemesterId"];

            var semesters = db.Semesters.Find(semester_id);

            if (semesters.registration_start_date > DateTime.Now || semesters.registration_end_date < DateTime.Now)
            {
                return RedirectToAction("Error", "StudentCourseRegistrations64132989");
            }

            var studentCourseRegistrations = db.StudentCourseRegistrations.Include(s => s.CourseOffering).Include(s => s.Student);
            return View(studentCourseRegistrations.ToList());
        }

        public ActionResult Error()
        {
            var semester_id = Session["SemesterId"];

            var semesters = db.Semesters.Find(semester_id);
            return View(semesters);
        }

        // GET: Lấy danh sách học phần trong KHHT học kỳ hiện tại
        [HttpGet]
        public JsonResult GetPlannedCoursesCurrentSemester()
        {
            try
            {
                var studentId = User.Identity.Name; // Lấy tạm từ User.Identity.Name

                // Lấy học kỳ hiện tại
                var currentSemester = db.Semesters
                    .Where(s => s.status == 1)
                    .OrderByDescending(s => s.semester_id)
                    .FirstOrDefault();

                if (currentSemester == null)
                    return Json(new { success = false, message = "Không tìm thấy học kỳ hiện tại" }, JsonRequestBehavior.AllowGet);

                // Lấy danh sách học phần trong KHHT
                var plannedCourses = (from plan in db.StudentLearningPlans
                                      join course in db.Courses on plan.course_id equals course.course_id
                                      where plan.student_id == studentId &&
                                            plan.semester_id == currentSemester.semester_id
                                      select new
                                      {
                                          course_id = course.course_id,
                                          course_name = course.course_name,
                                          credits = course.credits,
                                          description = course.description,
                                          semester_id = currentSemester.semester_id,
                                          semester_name = currentSemester.semester_name,
                                          // Kiểm tra xem học phần đã đăng ký chưa
                                          is_registered = db.StudentCourseRegistrations.Any(r => r.student_id == studentId && r.CourseOffering.course_id == course.course_id && r.CourseOffering.semester_id == currentSemester.semester_id)
                                      }).ToList();

                return Json(new { success = true, data = plannedCourses }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCourseOfferings(string courseId)
        {
            try
            {
                var currentSemester = db.Semesters
                    .Where(s => s.status == 1)
                    .OrderByDescending(s => s.semester_id)
                    .FirstOrDefault();

                if (currentSemester == null)
                    return Json(new { success = false, message = "Không tìm thấy học kỳ hiện tại" }, JsonRequestBehavior.AllowGet);

                var offerings = db.CourseOfferings
                    .Include(o => o.Cours)
                    .Include(o => o.Teacher.User.Profile)
                    .Include(o => o.Room)
                    .Where(o => o.course_id == courseId &&
                               o.semester_id == currentSemester.semester_id)
                    .Select(o => new
                    {
                        offering_id = o.offering_id,
                        course_id = o.course_id,
                        course_name = o.Cours.course_name,
                        room_name = o.Room.room_name == null ? "Chưa thiết đặt phòng" : o.Room.room_name,
                        teacher_name = o.Teacher.User.Profile.first_name + " " + o.Teacher.User.Profile.last_name,
                        max_capacity = o.max_capacity,
                        current_enrollment = o.StudentCourseRegistrations.Count()
                    })
                    .ToList()
                    .Select(o => new
                    {
                        o.offering_id,
                        o.course_id,
                        o.course_name,
                        o.room_name,
                        o.teacher_name,
                        o.max_capacity,
                        o.current_enrollment
                    })
                    .ToList();

                return Json(new { success = true, data = offerings }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetScheduleById(string id)
        {
            // Parse id sang integer
            if (!int.TryParse(id, out int offeringId))
            {
                return Json(new { success = false, message = "Mã lớp học phần không hợp lệ" }, JsonRequestBehavior.AllowGet);
            }

            var schedules = db.Schedules
                 .Include(s => s.CourseOffering)
                 .Include(s => s.CourseOffering.Room)
                 .Where(s => s.CourseOffering.offering_id == offeringId)
                 .Select(s => new
                 {
                     s.day_of_week,
                     s.slot_id,
                     room_name = s.CourseOffering.Room.room_name
                 })
                 .ToList();

            if (!schedules.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy lịch học cho lớp này" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, data = schedules }, JsonRequestBehavior.AllowGet);
        }


        // GET: Lấy danh sách lớp học phần đã đăng ký trong kỳ hiện tại
        [HttpGet]
        public JsonResult GetRegisteredCourses()
        {
            try
            {
                var studentId = User.Identity.Name;

                var currentSemester = db.Semesters
                    .Where(s => s.status == 1)
                    .OrderByDescending(s => s.semester_id)
                    .FirstOrDefault();

                if (currentSemester == null)
                    return Json(new { success = false, message = "Không tìm thấy học kỳ hiện tại" }, JsonRequestBehavior.AllowGet);

                var registeredCourses = (from r in db.StudentCourseRegistrations
                                         where r.student_id == studentId &&
                                               r.status == 1 &&
                                               r.CourseOffering.semester_id == currentSemester.semester_id
                                         select new
                                         {
                                             offering_id = r.offering_id,
                                             registration_id = r.registration_id,
                                             course_id = r.CourseOffering.course_id,
                                             course_name = r.CourseOffering.Cours.course_name,
                                             credits = r.CourseOffering.Cours.credits,
                                             teacher_name = r.CourseOffering.Teacher.User.Profile.first_name + " " + r.CourseOffering.Teacher.User.Profile.last_name,
                                             room_name = r.CourseOffering.Room.room_name,
                                             schedules = db.Schedules
                                                 .Where(s => s.offering_id == r.offering_id)
                                                 .Select(s => new
                                                 {
                                                     day_of_week = s.day_of_week,
                                                     slot = new
                                                     {
                                                         slot_id = s.TimeSlot.slot_id,
                                                         start_time = s.TimeSlot.start_time,
                                                         end_time = s.TimeSlot.end_time
                                                     }
                                                 }).ToList()
                                         }).ToList();

                return Json(new { success = true, data = registeredCourses }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCourseSchedule(long offeringId)
        {
            try
            {
                var schedules = db.Schedules
                    .Include(s => s.TimeSlot)
                    .Where(s => s.offering_id == offeringId)
                    .AsEnumerable()  // Chuyển về xử lý trong memory
                    .Select(s => new
                    {
                        day_of_week = s.day_of_week,
                        start_time = DateTime.ParseExact(s.TimeSlot.start_time.ToString(), "HH:mm:ss",
                                                      CultureInfo.InvariantCulture).ToString("HH:mm"),
                        end_time = DateTime.ParseExact(s.TimeSlot.end_time.ToString(), "HH:mm:ss",
                                                    CultureInfo.InvariantCulture).ToString("HH:mm")
                    })
                    .ToList();

                return Json(new { success = true, data = schedules }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Đăng ký lớp học phần
        [HttpPost]
        public JsonResult EnrollCourse(long offeringId)
        {
            try
            {
                var studentId = User.Identity.Name; // Sau này thay bằng User.Identity.Name
                var currentDate = DateTime.UtcNow;

                // 1. Kiểm tra thời gian đăng ký học phần
                var currentSemester = db.Semesters
                    .Where(s => s.status == 1 &&
                               currentDate >= s.course_registration_start &&
                               currentDate <= s.course_registration_end)
                    .FirstOrDefault();

                if (currentSemester == null)
                    return Json(new { success = false, message = "Không trong thời gian đăng ký học phần" });

                // 2. Kiểm tra lớp học phần tồn tại và thuộc học kỳ hiện tại
                var offering = db.CourseOfferings
                    .Include(o => o.Cours)
                    .FirstOrDefault(o => o.offering_id == offeringId &&
                                       o.semester_id == currentSemester.semester_id);

                if (offering == null)
                    return Json(new { success = false, message = "Lớp học phần không tồn tại hoặc không thuộc học kỳ hiện tại" });

                // 3. Kiểm tra học phần có trong KHHT không
                var isPlanned = db.StudentLearningPlans
                    .Any(p => p.student_id == studentId &&
                             p.course_id == offering.course_id &&
                             p.semester_id == currentSemester.semester_id);

                if (!isPlanned)
                    return Json(new { success = false, message = "Học phần này không có trong kế hoạch học tập của học kỳ" });

                // 4. Kiểm tra đã đăng ký lớp học phần nào của môn này chưa
                var existingRegistration = db.StudentCourseRegistrations
                    .FirstOrDefault(r => r.student_id == studentId &&
                                       r.CourseOffering.course_id == offering.course_id &&
                                       r.CourseOffering.semester_id == currentSemester.semester_id &&
                                       r.status == 1);

                if (existingRegistration != null)
                    return Json(new { success = false, message = "Bạn đã đăng ký lớp học phần khác của môn học này" });

                // 5. Kiểm tra số lượng đăng ký
                var currentEnrollment = db.StudentCourseRegistrations
                    .Count(r => r.offering_id == offeringId && r.status == 1);

                if (currentEnrollment >= offering.max_capacity)
                    return Json(new { success = false, message = "Lớp học phần đã đầy" });

                // 6. Kiểm tra trùng lịch
                var newSchedules = db.Schedules
                    .Where(s => s.offering_id == offeringId)
                    .Select(s => new { s.day_of_week, s.slot_id })
                    .ToList();

                var existingSchedules = (from r in db.StudentCourseRegistrations
                                         join o in db.CourseOfferings on r.offering_id equals o.offering_id
                                         join s in db.Schedules on o.offering_id equals s.offering_id
                                         where r.student_id == studentId &&
                                               r.status == 1 &&
                                               o.semester_id == currentSemester.semester_id
                                         select new { s.day_of_week, s.slot_id })
                                        .ToList();

                var hasConflict = newSchedules.Any(n =>
                    existingSchedules.Any(e => e.day_of_week == n.day_of_week && e.slot_id == n.slot_id));

                if (hasConflict)
                    return Json(new { success = false, message = "Lịch học bị trùng với lớp học phần khác đã đăng ký" });

                // 7. Thực hiện đăng ký
                var registration = new StudentCourseRegistration
                {
                    student_id = studentId,
                    offering_id = offeringId,
                    registration_date = currentDate,
                    status = 1
                };

                db.StudentCourseRegistrations.Add(registration);
                db.SaveChanges();

                return Json(new { success = true, message = "Đăng ký thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: Hủy đăng ký lớp học phần
        [HttpPost]
        public JsonResult UnenrollCourse(long registrationId)
        {
            try
            {
                var studentId = User.Identity.Name; // Sau này thay bằng User.Identity.Name
                var currentDate = DateTime.UtcNow; // 2024-12-29 18:37:06

                // 1. Kiểm tra thời gian đăng ký học phần
                var currentSemester = db.Semesters
                    .Where(s => s.status == 1 &&
                               currentDate >= s.course_registration_start &&
                               currentDate <= s.course_registration_end)
                    .FirstOrDefault();

                if (currentSemester == null)
                    return Json(new { success = false, message = "Không trong thời gian đăng ký học phần" });

                // 2. Kiểm tra đăng ký tồn tại và thuộc về sinh viên hiện tại
                var registration = db.StudentCourseRegistrations
                    .FirstOrDefault(r => r.registration_id == registrationId &&
                                       r.student_id == studentId);

                if (registration == null)
                    return Json(new { success = false, message = "Không tìm thấy đăng ký lớp học phần" });

                // 3. Xóa đăng ký khỏi bảng
                db.StudentCourseRegistrations.Remove(registration);
                db.SaveChanges();

                return Json(new { success = true, message = "Hủy đăng ký thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
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

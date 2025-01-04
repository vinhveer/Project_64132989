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
    public class Schedules64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: Students/Schedules64132989
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetMySchedule()
        {
            try
            {
                string studentId = User.Identity.Name;

                var schedules = db.Schedules
                    .Include(s => s.CourseOffering)
                    .Include(s => s.TimeSlot)
                    .Where(s => s.CourseOffering.StudentCourseRegistrations
                        .Any(r => r.student_id == studentId))
                    .AsEnumerable()  // Chuyển về xử lý trong memory
                    .Select(s => new
                    {
                        course_id = s.CourseOffering.course_id,
                        course_name = s.CourseOffering.Cours.course_name,
                        teacher_name = s.CourseOffering.teacher_user_id,
                        room_name = s.CourseOffering.Room.room_name,
                        day_of_week = s.day_of_week,
                        slot_id = s.slot_id,
                        start_time = DateTime.ParseExact(s.TimeSlot.start_time.ToString(), "HH:mm:ss",
                                                       CultureInfo.InvariantCulture).ToString("HH:mm"),
                        end_time = DateTime.ParseExact(s.TimeSlot.end_time.ToString(), "HH:mm:ss",
                                                     CultureInfo.InvariantCulture).ToString("HH:mm")
                    })
                    .OrderBy(s => s.day_of_week)
                    .ThenBy(s => s.slot_id)
                    .ToList();

                return Json(new { success = true, data = schedules }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
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

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
    public class Schedules64132989Controller : Controller // Đổi tên controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: Lấy danh sách lịch học của một lớp
        [HttpGet]
        public JsonResult GetSchedules(long offeringId)
        {
            try
            {
                var schedules = db.Schedules
                    .Include(s => s.TimeSlot)
                    .Include(s => s.CourseOffering)
                    .Include(s => s.CourseOffering.Room)
                    .Where(s => s.offering_id == offeringId)
                    .Select(s => new
                    {
                        schedule_id = s.schedule_id,
                        day_of_week = s.day_of_week,
                        slot = new
                        {
                            slot_id = s.TimeSlot.slot_id,
                            start_time = s.TimeSlot.start_time,
                            end_time = s.TimeSlot.end_time,
                            session = s.TimeSlot.session
                        },
                        room_name = s.CourseOffering.Room.room_name,
                    })
                    .ToList();

                return Json(new { success = true, data = schedules }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Thêm lịch học mới
        [HttpPost]
        public JsonResult AddSchedule(long offeringId, byte dayOfWeek, byte slotId) // Sửa kiểu dữ liệu
        {
            try
            {
                var currentDate = DateTime.UtcNow; // 2024-12-29 19:39:33
                var currentUser = User.Identity.Name ?? "trittntu"; // Lấy user hiện tại

                // Kiểm tra lịch học trùng
                var existingSchedule = db.Schedules
                    .Any(s => s.offering_id == offeringId &&
                             s.day_of_week == dayOfWeek &&
                             s.slot_id == slotId);

                if (existingSchedule)
                {
                    return Json(new { success = false, message = "Lịch học này đã tồn tại!" });
                }

                var schedule = new Schedule
                {
                    offering_id = offeringId,
                    day_of_week = dayOfWeek,
                    slot_id = slotId,
                    created_at = currentDate,
                    created_by = currentUser,
                    status = 1
                };

                db.Schedules.Add(schedule);
                db.SaveChanges();

                return Json(new { success = true, message = "Thêm lịch học thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Cập nhật lịch học
        [HttpPost]
        public JsonResult UpdateSchedule(long scheduleId, byte dayOfWeek, byte slotId) // Sửa kiểu dữ liệu
        {
            try
            {
                var schedule = db.Schedules.Find(scheduleId);
                if (schedule == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy lịch học!" });
                }

                // Kiểm tra lịch học trùng
                var existingSchedule = db.Schedules
                    .Any(s => s.schedule_id != scheduleId &&
                             s.offering_id == schedule.offering_id &&
                             s.day_of_week == dayOfWeek &&
                             s.slot_id == slotId);

                if (existingSchedule)
                {
                    return Json(new { success = false, message = "Lịch học này đã tồn tại!" });
                }

                schedule.day_of_week = dayOfWeek;
                schedule.slot_id = slotId;

                db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật lịch học thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Xóa lịch học
        [HttpPost]
        public JsonResult DeleteSchedule(long scheduleId)
        {
            try
            {
                var schedule = db.Schedules.Find(scheduleId);
                if (schedule == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy lịch học!" });
                }

                db.Schedules.Remove(schedule);
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa lịch học thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Tự động thêm lịch học
        [HttpPost]
        public JsonResult AutoSchedule(long offeringId, int credits)
        {
            try
            {
                // Gọi stored procedure
                db.Database.ExecuteSqlCommand(
                    "EXEC [dbo].[sp_ScheduleCourseOffering] @offering_id, @credits",
                    new SqlParameter("@offering_id", offeringId),
                    new SqlParameter("@credits", credits)
                );

                return Json(new { success = true, message = "Tự động thêm lịch học thành công!" });
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

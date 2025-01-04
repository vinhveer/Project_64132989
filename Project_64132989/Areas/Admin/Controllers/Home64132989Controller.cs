using Project_64132989.Models.Data;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

namespace Project_64132989.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Home64132989Controller : Controller
    {
        private readonly Model64132989DbContext db = new Model64132989DbContext();

        public ActionResult Index()
        {
            return View();
        }

        // 1. Thống kê số lượng người dùng theo vai trò
        public JsonResult GetUserStatistics()
        {
            var stats = new
            {
                Students = db.Users.Count(u => u.user_type == 1),
                Teachers = db.Users.Count(u => u.user_type == 2),
                TrainingOfficers = db.Users.Count(u => u.user_type == 3),
                Admins = db.Users.Count(u => u.user_type == 4)
            };
            return Json(stats, JsonRequestBehavior.AllowGet);
        }

        // 2. Thống kê sinh viên theo ngành và giới tính
        public JsonResult GetStudentsByDepartmentAndGender()
        {
            var query = from s in db.Students
                        join p in db.Profiles on s.user_id equals p.user_id
                        join prog in db.TrainingPrograms on s.program_id equals prog.program_id
                        join dept in db.Departments on prog.department_id equals dept.department_id
                        group new { p } by new { dept.department_name } into deptGroup
                        select new
                        {
                            Department = deptGroup.Key.department_name,
                            Total = deptGroup.Count(),
                            Male = deptGroup.Count(x => x.p.gender == 0),
                            Female = deptGroup.Count(x => x.p.gender == 1)
                        };

            return Json(query.ToList(), JsonRequestBehavior.AllowGet);
        }

        // 3. Thống kê giảng viên theo ngành và giới tính
        public JsonResult GetTeachersByDepartmentAndGender()
        {
            var query = from t in db.Teachers
                        join p in db.Profiles on t.user_id equals p.user_id
                        join dept in db.Departments on t.department_id equals dept.department_id
                        group new { p } by new { dept.department_name } into deptGroup
                        select new
                        {
                            Department = deptGroup.Key.department_name,
                            Total = deptGroup.Count(),
                            Male = deptGroup.Count(x => x.p.gender == 0),
                            Female = deptGroup.Count(x => x.p.gender == 1)
                        };

            return Json(query.ToList(), JsonRequestBehavior.AllowGet);
        }

        // 4. Thống kê số lượng tài khoản theo vai trò
        public JsonResult GetAccountByGender()
        {
            var query = from u in db.Users
                        join p in db.Profiles on u.user_id equals p.user_id
                        group p by p.gender into g
                        select new
                        {
                            Gender = g.Key == 0 ? "Nam" : "Nữ",
                            Count = g.Count()
                        };

            return Json(query.ToList(), JsonRequestBehavior.AllowGet);
        }

        // 5. Thống kê số lượng sinh viên theo phòng ban
        public JsonResult GetStudentsByDepartment()
        {
            var query = from s in db.Students
                        join prog in db.TrainingPrograms on s.program_id equals prog.program_id
                        join dept in db.Departments on prog.department_id equals dept.department_id
                        group s by dept.department_name into g
                        select new
                        {
                            Department = g.Key,
                            Count = g.Count()
                        };

            return Json(query.ToList(), JsonRequestBehavior.AllowGet);
        }


        // 5. Thống kê số lượng sinh viên theo chương trình đào tạo
        public JsonResult GetStudentsByTrainingProgram()
        {
            var query = from s in db.Students
                        join prog in db.TrainingPrograms on s.program_id equals prog.program_id
                        group s by new { prog.program_id, prog.program_name } into g
                        select new
                        {
                            ProgramName = g.Key.program_name,
                            StudentCount = g.Count()
                        };

            return Json(query.ToList(), JsonRequestBehavior.AllowGet);
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
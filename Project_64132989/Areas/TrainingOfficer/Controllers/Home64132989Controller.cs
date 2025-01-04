using Project_64132989.Models.Data;
using System.Linq;
using System.Web.Mvc;

namespace Project_64132989.Areas.TrainingOfficer.Controllers
{
    [Authorize(Roles = "TrainingOfficer")]
    public class Home64132989Controller : Controller
    {
        private readonly Model64132989DbContext _db = new Model64132989DbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetBasicStatistics()
        {
            var data = new
            {
                labels = new[] { "Course Offerings", "Courses", "Training Programs", "Admin Classes" },
                datasets = new[] {
                    new {
                        data = new[] {
                            _db.CourseOfferings.Count(),
                            _db.Courses.Count(),
                            _db.TrainingPrograms.Count(),
                            _db.AdminClasses.Count()
                        },
                        backgroundColor = new[] { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0" }
                    }
                }
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDepartmentCourses()
        {
            var data = _db.Departments
                .Select(d => new {
                    department = d.department_name,
                    courses = d.Courses.Count()
                })
                .OrderByDescending(x => x.courses);

            return Json(new
            {
                labels = data.Select(x => x.department),
                datasets = new[] {
                    new {
                        label = "Number of Courses",
                        data = data.Select(x => x.courses),
                        backgroundColor = "#36A2EB"
                    }
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPlannedCourses()
        {
            var data = _db.Courses
                .Select(c => new {
                    course = c.course_name,
                    plans = c.StudentLearningPlans.Count()
                })
                .OrderByDescending(x => x.plans)
                .Take(10);

            return Json(new
            {
                labels = data.Select(x => x.course),
                datasets = new[] {
                    new {
                        label = "Learning Plans",
                        data = data.Select(x => x.plans),
                        backgroundColor = "#FFCE56"
                    }
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTeachersPerDepartment()
        {
            var data = _db.Departments
                .Select(d => new {
                    department = d.department_name,
                    teachers = d.Teachers.Count()
                })
                .OrderByDescending(x => x.teachers);

            return Json(new
            {
                labels = data.Select(x => x.department),
                datasets = new[] {
                    new {
                        label = "Number of Teachers",
                        data = data.Select(x => x.teachers),
                        backgroundColor = "#4BC0C0"
                    }
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProgramStudents()
        {
            var data = _db.TrainingPrograms
                .Select(p => new {
                    program = p.program_name,
                    students = p.Students.Count()
                })
                .OrderByDescending(x => x.students);

            return Json(new
            {
                labels = data.Select(x => x.program),
                datasets = new[] {
                    new {
                        label = "Number of Students",
                        data = data.Select(x => x.students),
                        backgroundColor = "#9966FF"
                    }
                }
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
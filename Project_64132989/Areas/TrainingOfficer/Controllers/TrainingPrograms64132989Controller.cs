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
    public class TrainingPrograms64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: TrainingOfficer/TrainingPrograms64132989
        public ActionResult Index()
        {
            var trainingPrograms = db.TrainingPrograms.Include(t => t.Department);
            return View(trainingPrograms.ToList());
        }

        [HttpPost]
        public JsonResult GetTrainingProgramList(int offset, int limit, string search, string sort, string order)
        {
            try
            {
                // Kiểm tra tham số phân trang
                if (offset < 0 || limit <= 0)
                {
                    return Json(new { success = false, message = "Tham số phân trang không hợp lệ" });
                }

                // Khởi tạo query
                var query = db.TrainingPrograms
                    .Include(t => t.Department)
                    .AsQueryable();

                // Tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(t =>
                        t.program_id.ToLower().Contains(search) ||
                        t.program_name.ToLower().Contains(search) ||
                        t.version.ToLower().Contains(search) ||
                        (t.Department != null && t.Department.department_name.ToLower().Contains(search))
                    );
                }

                // Đếm tổng số bản ghi
                int total = query.Count();

                // Sắp xếp
                if (!string.IsNullOrEmpty(sort))
                {
                    switch (sort)
                    {
                        case "programId":
                            query = order == "asc" ? query.OrderBy(t => t.program_id) : query.OrderByDescending(t => t.program_id);
                            break;
                        case "programName":
                            query = order == "asc" ? query.OrderBy(t => t.program_name) : query.OrderByDescending(t => t.program_name);
                            break;
                        case "departmentName":
                            query = order == "asc"
                                ? query.OrderBy(t => t.Department != null ? t.Department.department_name : "")
                                : query.OrderByDescending(t => t.Department != null ? t.Department.department_name : "");
                            break;
                        case "totalCredits":
                            query = order == "asc" ? query.OrderBy(t => t.total_credits) : query.OrderByDescending(t => t.total_credits);
                            break;
                        case "version":
                            query = order == "asc" ? query.OrderBy(t => t.version) : query.OrderByDescending(t => t.version);
                            break;
                        case "startYear":
                            query = order == "asc" ? query.OrderBy(t => t.start_year) : query.OrderByDescending(t => t.start_year);
                            break;
                        case "status":
                            query = order == "asc" ? query.OrderBy(t => t.status) : query.OrderByDescending(t => t.status);
                            break;
                        default:
                            query = order == "asc" ? query.OrderBy(t => t.program_id) : query.OrderByDescending(t => t.program_id);
                            break;
                    }
                }

                // Phân trang và lấy dữ liệu
                var trainingPrograms = query.Skip(offset).Take(limit).ToList();

                // Chuyển đổi dữ liệu
                var programs = trainingPrograms.Select(t => new
                {
                    state = false,
                    programId = t.program_id,
                    programName = t.program_name,
                    departmentName = t.Department?.department_name ?? "Không có khoa",
                    totalCredits = t.total_credits,
                    version = t.version,
                    startYear = t.start_year,
                    endYear = t.end_year,
                    status = t.status == 1 ? "Hoạt động" : "Không hoạt động",
                    actions = ""
                }).ToList();

                return Json(new
                {
                    total = total,
                    rows = programs,
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

        // GET: TrainingOfficer/TrainingPrograms64132989/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingProgram trainingProgram = db.TrainingPrograms.Find(id);
            if (trainingProgram == null)
            {
                return HttpNotFound();
            }
            return View(trainingProgram);
        }

        // GET: TrainingOfficer/TrainingPrograms64132989/Create
        public ActionResult Create()
        {
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name");
            return View();
        }

        // POST: TrainingOfficer/TrainingPrograms64132989/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "program_id,department_id,program_name,total_credits,version,start_year,end_year,status")] TrainingProgram trainingProgram)
        {
            if (ModelState.IsValid)
            {
                db.TrainingPrograms.Add(trainingProgram);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", trainingProgram.department_id);
            return View(trainingProgram);
        }

        // GET: TrainingOfficer/TrainingPrograms64132989/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingProgram trainingProgram = db.TrainingPrograms.Find(id);
            if (trainingProgram == null)
            {
                return HttpNotFound();
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", trainingProgram.department_id);
            return View(trainingProgram);
        }

        // POST: TrainingOfficer/TrainingPrograms64132989/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "program_id,department_id,program_name,total_credits,version,start_year,end_year,status")] TrainingProgram trainingProgram)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingProgram).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", trainingProgram.department_id);
            return View(trainingProgram);
        }

        // GET: TrainingOfficer/TrainingPrograms64132989/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingProgram trainingProgram = db.TrainingPrograms.Find(id);
            if (trainingProgram == null)
            {
                return HttpNotFound();
            }
            return View(trainingProgram);
        }

        // POST: TrainingOfficer/TrainingPrograms64132989/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TrainingProgram trainingProgram = db.TrainingPrograms.Find(id);
            db.TrainingPrograms.Remove(trainingProgram);
            db.SaveChanges();
            return RedirectToAction("Index");
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

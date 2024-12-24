using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_64132989.Models.Data;

namespace Project_64132989.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Departments64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: TrainingOfficer/Departments64132989
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDepartmentList(int offset, int limit, string search, string sort, string order)
        {
            var query = db.Departments.AsQueryable();

            // Xử lý tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(d =>
                    d.department_id.ToLower().Contains(search) ||
                    d.department_name.ToLower().Contains(search) ||
                    (d.description != null && d.description.ToLower().Contains(search))
                );
            }

            // Đếm tổng số bản ghi thỏa mãn điều kiện tìm kiếm
            int total = query.Count();

            // Xử lý sắp xếp
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "departmentId":
                        query = order == "asc" ? query.OrderBy(d => d.department_id)
                                             : query.OrderByDescending(d => d.department_id);
                        break;
                    case "departmentName":
                        query = order == "asc" ? query.OrderBy(d => d.department_name)
                                             : query.OrderByDescending(d => d.department_name);
                        break;
                    case "description":
                        query = order == "asc" ? query.OrderBy(d => d.description)
                                             : query.OrderByDescending(d => d.description);
                        break;
                    case "status":
                        query = order == "asc" ? query.OrderBy(d => d.status)
                                             : query.OrderByDescending(d => d.status);
                        break;
                }
            }

            // Phân trang và chọn dữ liệu
            var departments = query
                .Skip(offset)
                .Take(limit)
                .Select(d => new
                {
                    departmentId = d.department_id,
                    departmentName = d.department_name,
                    description = d.description,
                    status = d.status,
                    teacherCount = d.Teachers.Count,
                    courseCount = d.Courses.Count,
                    classCount = d.AdminClasses.Count,
                    programCount = d.TrainingPrograms.Count
                })
                .ToList();

            // Trả về kết quả dạng JSON
            return Json(new
            {
                total = total,
                rows = departments
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: TrainingOfficer/Departments64132989/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: TrainingOfficer/Departments64132989/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingOfficer/Departments64132989/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "department_id,department_name,description,status")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: TrainingOfficer/Departments64132989/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: TrainingOfficer/Departments64132989/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "department_id,department_name,description,status")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: TrainingOfficer/Departments64132989/Delete/5
        public ActionResult Delete(string departmentId)
        {
            if (departmentId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(departmentId);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: TrainingOfficer/Departments64132989/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
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

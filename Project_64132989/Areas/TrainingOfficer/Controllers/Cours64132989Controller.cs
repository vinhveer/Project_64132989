using OfficeOpenXml.Style;
using OfficeOpenXml;
using Project_64132989.Models.Data;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace Project_64132989.Areas.TrainingOfficer.Controllers
{
    public class Cours64132989Controller : Controller
    {
        private Model64132989DbContext db = new Model64132989DbContext();

        // GET: TrainingOfficer/Cours64132989
        public ActionResult Index()
        {
            var courses = db.Courses.Include(c => c.Cours1).Include(c => c.Department);
            return View(courses.ToList());
        }

        [HttpPost]
        public JsonResult GetCourseList(int offset, int limit, string search, string sort, string order)
        {
            try
            {
                if (offset < 0 || limit <= 0)
                {
                    return Json(new { success = false, message = "Tham số phân trang không hợp lệ" });
                }

                var query = db.Courses
                    .Include(c => c.Department)
                    .Include(c => c.Cours1)
                    .AsQueryable();

                // Tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(c =>
                        c.course_id.ToLower().Contains(search) ||
                        c.course_name.ToLower().Contains(search) ||
                        (c.Department != null && c.Department.department_name.ToLower().Contains(search)) ||
                        (c.description != null && c.description.ToLower().Contains(search))
                    );
                }

                // Đếm tổng số bản ghi
                int total = query.Count();

                // Sắp xếp
                if (!string.IsNullOrEmpty(sort))
                {
                    switch (sort)
                    {
                        case "courseId":
                            query = order == "asc" ? query.OrderBy(c => c.course_id) : query.OrderByDescending(c => c.course_id);
                            break;
                        case "courseName":
                            query = order == "asc" ? query.OrderBy(c => c.course_name) : query.OrderByDescending(c => c.course_name);
                            break;
                        case "departmentName":
                            query = order == "asc"
                                ? query.OrderBy(c => c.Department != null ? c.Department.department_name : "")
                                : query.OrderByDescending(c => c.Department != null ? c.Department.department_name : "");
                            break;
                        case "credits":
                            query = order == "asc" ? query.OrderBy(c => c.credits) : query.OrderByDescending(c => c.credits);
                            break;
                        case "courseType":
                            query = order == "asc" ? query.OrderBy(c => c.course_type) : query.OrderByDescending(c => c.course_type);
                            break;
                        case "prerequisite":
                            query = order == "asc" ? query.OrderBy(c => c.Cours1.course_name) : query.OrderByDescending(c => c.Cours1.course_name);
                            break;
                        case "status":
                            query = order == "asc" ? query.OrderBy(c => c.status) : query.OrderByDescending(c => c.status);
                            break;
                        default:
                            query = order == "asc" ? query.OrderBy(c => c.course_id) : query.OrderByDescending(c => c.course_id);
                            break;
                    }
                }

                // Thực thi query và lấy dữ liệu
                var courses = query.Skip(offset).Take(limit).ToList();

                // Xử lý và chuyển đổi dữ liệu
                var courseList = courses.Select(c => new
                {
                    state = false,
                    courseId = c.course_id,
                    courseName = c.course_name,
                    departmentName = c.Department?.department_name ?? "Không có khoa",
                    credits = c.credits,
                    courseType = GetCourseType(c.course_type),
                    prerequisite = c.Cours1?.course_name ?? "Không có",
                    description = c.description ?? "Không có mô tả",
                    status = c.status == 1 ? "Hoạt động" : "Không hoạt động",
                    actions = ""
                }).ToList();

                return Json(new
                {
                    total = total,
                    rows = courseList,
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

        // Hàm phụ trợ để chuyển đổi course_type sang text
        private string GetCourseType(byte courseType)
        {
            switch (courseType)
            {
                case 1:
                    return "Bắt buộc";
                case 2:
                    return "Tự chọn";
                case 3:
                    return "Thay thế";
                default:
                    return "Không xác định";
            }
        }

        // GET: TrainingOfficer/Cours64132989/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            return View(cours);
        }

        // GET: TrainingOfficer/Cours64132989/Create
        public ActionResult Create()
        {
            ViewBag.prerequisite_course_id = new SelectList(db.Courses, "course_id", "course_name");
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name");
            return View();
        }

        // POST: TrainingOfficer/Cours64132989/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "course_id,course_name,description,credits,department_id,course_type,prerequisite_course_id,status")] Cours cours)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(cours);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.prerequisite_course_id = new SelectList(db.Courses, "course_id", "course_name", cours.prerequisite_course_id);
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", cours.department_id);
            return View(cours);
        }

        // GET: TrainingOfficer/Cours64132989/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            ViewBag.prerequisite_course_id = new SelectList(db.Courses, "course_id", "course_name", cours.prerequisite_course_id);
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", cours.department_id);
            return View(cours);
        }

        // POST: TrainingOfficer/Cours64132989/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "course_id,course_name,description,credits,department_id,course_type,prerequisite_course_id,status")] Cours cours)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cours).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.prerequisite_course_id = new SelectList(db.Courses, "course_id", "course_name", cours.prerequisite_course_id);
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "department_name", cours.department_id);
            return View(cours);
        }

        // GET: TrainingOfficer/Cours64132989/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            return View(cours);
        }

        // POST: TrainingOfficer/Cours64132989/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Cours cours = db.Courses.Find(id);
            db.Courses.Remove(cours);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UploadCourseList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadCourseList(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                ViewBag.ErrorMessage = "Vui lòng chọn file để tải lên.";
                return View();
            }

            try
            {
                // Kiểm tra định dạng file
                var supportedTypes = new[] { ".xls", ".xlsx" };
                var fileExt = Path.GetExtension(file.FileName)?.ToLower();
                if (!supportedTypes.Contains(fileExt))
                {
                    ViewBag.ErrorMessage = "Chỉ hỗ trợ file Excel (.xls, .xlsx).";
                    return View();
                }

                // Lưu file tạm thời
                var filePath = Server.MapPath("~/Uploads/Excel/" + Guid.NewGuid() + fileExt);

                // Đảm bảo thư mục tồn tại
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                file.SaveAs(filePath);

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension?.Rows ?? 0;

                    if (rowCount < 2)
                    {
                        ViewBag.ErrorMessage = "File không chứa dữ liệu.";
                        return View();
                    }

                    // Bắt đầu transaction
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            for (int row = 2; row <= rowCount; row++)
                            {
                                // Đọc các trường bắt buộc từ file
                                var courseId = worksheet.Cells[row, 1]?.Text?.Trim();
                                var courseName = worksheet.Cells[row, 2]?.Text?.Trim();
                                var creditsText = worksheet.Cells[row, 3]?.Text?.Trim();
                                var departmentId = worksheet.Cells[row, 4]?.Text?.Trim();
                                var courseTypeText = worksheet.Cells[row, 5]?.Text?.Trim();

                                // Kiểm tra các trường bắt buộc
                                if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(courseName) ||
                                    string.IsNullOrEmpty(creditsText) || string.IsNullOrEmpty(departmentId) ||
                                    string.IsNullOrEmpty(courseTypeText))
                                {
                                    ViewBag.ErrorMessage = $"Dòng {row}: Thiếu thông tin bắt buộc. Vui lòng kiểm tra lại file.";
                                    transaction.Rollback();
                                    return View();
                                }

                                // Chuyển đổi dữ liệu
                                if (!int.TryParse(creditsText, out int credits) ||
                                    !byte.TryParse(courseTypeText, out byte courseType))
                                {
                                    ViewBag.ErrorMessage = $"Dòng {row}: Dữ liệu không hợp lệ cho Credits hoặc Course Type.";
                                    transaction.Rollback();
                                    return View();
                                }

                                // Kiểm tra trùng lặp course_id
                                if (db.Courses.Any(c => c.course_id == courseId))
                                {
                                    ViewBag.ErrorMessage = $"Dòng {row}: Course ID '{courseId}' đã tồn tại trong hệ thống.";
                                    transaction.Rollback();
                                    return View();
                                }

                                // Đọc các trường không bắt buộc
                                var description = worksheet.Cells[row, 6]?.Text?.Trim();
                                var prerequisiteCourseId = worksheet.Cells[row, 7]?.Text?.Trim();
                                var statusText = worksheet.Cells[row, 8]?.Text?.Trim();
                                byte? status = null;
                                if (!string.IsNullOrEmpty(statusText))
                                {
                                    if (byte.TryParse(statusText, out byte statusValue))
                                    {
                                        status = statusValue;
                                    }
                                }

                                if (!db.Departments.Any(d => d.department_id == departmentId))
                                {
                                    ViewBag.ErrorMessage = $"Dòng {row}: Mã phòng ban '{departmentId}' không tồn tại trong hệ thống.";
                                    transaction.Rollback();
                                    return View();
                                }

                                // Tạo Course mới
                                var course = new Cours
                                {
                                    course_id = courseId,
                                    course_name = courseName,
                                    description = description,
                                    credits = credits,
                                    department_id = departmentId,
                                    course_type = courseType,
                                    prerequisite_course_id = string.IsNullOrEmpty(prerequisiteCourseId) ? null : prerequisiteCourseId,
                                    status = status
                                };

                                db.Courses.Add(course);
                            }

                            // Lưu thay đổi vào cơ sở dữ liệu
                            db.SaveChanges();
                            transaction.Commit();

                            // Xóa file tạm sau khi import thành công
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }

                            ViewBag.Message = "Tải lên danh sách khóa học thành công!";
                            return View();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ViewBag.ErrorMessage = $"Có lỗi xảy ra: {ex.Message} {ex.InnerException}";
                            return View();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Có lỗi xảy ra: {ex.Message}";
                return View();
            }
        }

        public ActionResult DownloadTemplate()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Template");

                // Thêm tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "Course ID";
                worksheet.Cells[1, 2].Value = "Course Name";
                worksheet.Cells[1, 3].Value = "Credits";
                worksheet.Cells[1, 4].Value = "Department ID";
                worksheet.Cells[1, 5].Value = "Course Type";
                worksheet.Cells[1, 6].Value = "Description";
                worksheet.Cells[1, 7].Value = "Prerequisite Course ID";
                worksheet.Cells[1, 8].Value = "Status";

                // Format tiêu đề
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Thêm validation và comments
                worksheet.Cells[2, 1].AddComment("Mã khóa học, tối đa 20 ký tự", "Template Help");
                worksheet.Cells[2, 2].AddComment("Tên khóa học, tối đa 200 ký tự", "Template Help");
                worksheet.Cells[2, 3].AddComment("Số tín chỉ, nhập số nguyên", "Template Help");
                worksheet.Cells[2, 4].AddComment("Mã khoa, tối đa 10 ký tự", "Template Help");
                worksheet.Cells[2, 5].AddComment("Loại khóa học, nhập số từ 0-255", "Template Help");
                worksheet.Cells[2, 8].AddComment("Trạng thái, nhập số từ 0-255 (không bắt buộc)", "Template Help");

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Tạo file và trả về
                var fileBytes = package.GetAsByteArray();
                var fileName = $"CourseTemplate_{DateTime.Now:yyyyMMdd}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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

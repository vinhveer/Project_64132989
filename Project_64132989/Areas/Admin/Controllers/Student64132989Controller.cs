using OfficeOpenXml; // Sử dụng EPPlus 4.x hoặc ClosedXML
using Project_64132989.Models.Data;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Project_64132989.Areas.Admin.Controllers
{
    [Authorize]
    public class Student64132989Controller : Controller
    {
        private readonly Model64132989DbContext _context = new Model64132989DbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetStudentList(int offset, int limit, string search, string sort, string order)
        {
            var query = _context.Users
                .Where(u => u.user_type == 1)
                .Include(u => u.Profile)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(u =>
                    u.user_id.ToLower().Contains(search) ||
                    (u.Profile.first_name + " " + u.Profile.last_name).ToLower().Contains(search) ||
                    u.email.ToLower().Contains(search));
            }

            int total = query.Count();

            if (!string.IsNullOrEmpty(sort))
            {
                if (sort == "userId")
                    query = order == "asc" ? query.OrderBy(u => u.user_id) : query.OrderByDescending(u => u.user_id);
                else if (sort == "fullName")
                    query = order == "asc"
                        ? query.OrderBy(u => u.Profile.first_name + " " + u.Profile.last_name)
                        : query.OrderByDescending(u => u.Profile.first_name + " " + u.Profile.last_name);
                else if (sort == "email")
                    query = order == "asc" ? query.OrderBy(u => u.email) : query.OrderByDescending(u => u.email);
            }

            var students = query
                .Skip(offset)
                .Take(limit)
                .Select(u => new
                {
                    userId = u.user_id,
                    fullName = u.Profile.first_name + " " + u.Profile.last_name,
                    email = u.email,
                    phoneNumber = u.Profile.phone_number,
                    dateOfBirth = u.Profile.date_of_birth,
                    gender = u.Profile.gender == 1 ? "Nam" : (u.Profile.gender == 0 ? "Nữ" : "Khác")
                })
                .ToList();

            return Json(new
            {
                total = total,
                rows = students
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string userId)
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.Profile)
                    .FirstOrDefault(u => u.user_id == userId);

                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sinh viên" });
                }

                if (user.Profile != null)
                {
                    _context.Profiles.Remove(user.Profile);
                }

                _context.Users.Remove(user);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        public ActionResult UploadStudentList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadStudentList(HttpPostedFileBase file)
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
                var filePath = Server.MapPath("~/App_Data/Uploads/" + Guid.NewGuid() + fileExt);
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

                    for (int row = 2; row <= rowCount; row++)
                    {
                        // Đọc các trường từ file
                        var userId = worksheet.Cells[row, 1]?.Text?.Trim();
                        var email = worksheet.Cells[row, 2]?.Text?.Trim();
                        var password = worksheet.Cells[row, 3]?.Text?.Trim();
                        var firstName = worksheet.Cells[row, 4]?.Text?.Trim();
                        var lastName = worksheet.Cells[row, 5]?.Text?.Trim();

                        // Kiểm tra các trường bắt buộc
                        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) ||
                            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(firstName) ||
                            string.IsNullOrEmpty(lastName))
                        {
                            ViewBag.ErrorMessage = $"Dòng {row}: Thiếu thông tin bắt buộc. Vui lòng kiểm tra lại file.";
                            return View();
                        }

                        // Kiểm tra trùng lặp user_id
                        if (_context.Users.Any(u => u.user_id == userId))
                        {
                            ViewBag.ErrorMessage = $"Dòng {row}: User ID '{userId}' đã tồn tại trong hệ thống.";
                            return View();
                        }

                        // Đọc các trường không bắt buộc
                        var dobText = worksheet.Cells[row, 6]?.Text?.Trim();
                        var genderText = worksheet.Cells[row, 7]?.Text?.Trim();
                        var phoneNumber = worksheet.Cells[row, 8]?.Text?.Trim();
                        var address = worksheet.Cells[row, 9]?.Text?.Trim();

                        // Tạo User
                        var user = new User
                        {
                            user_id = userId,
                            email = email,
                            password_hash = password,
                            user_type = 1, // Sinh viên
                            created_at = DateTime.Now
                        };

                        // Tạo Profile
                        var profile = new Profile
                        {
                            user_id = userId,
                            first_name = firstName,
                            last_name = lastName,
                            date_of_birth = DateTime.TryParse(dobText, out var dob) ? dob : (DateTime?)null,
                            gender = byte.TryParse(genderText, out var gender) ? gender : (byte?)null,
                            phone_number = phoneNumber,
                            address = address
                        };

                        // Thêm dữ liệu vào các bảng
                        _context.Users.Add(user);
                        _context.Profiles.Add(profile);
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.SaveChanges();
                }

                ViewBag.Message = "Tải lên danh sách thành công!";
                return View();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                ViewBag.ErrorMessage = $"Có lỗi xảy ra: {ex.Message}";
                return View();
            }
        }

        public ActionResult DownloadTemplate()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Template");

                // Chỉ giữ các cột liên quan đến User và Profile
                worksheet.Cells[1, 1].Value = "User ID";
                worksheet.Cells[1, 2].Value = "Email";
                worksheet.Cells[1, 3].Value = "Password";
                worksheet.Cells[1, 4].Value = "First Name";
                worksheet.Cells[1, 5].Value = "Last Name";
                worksheet.Cells[1, 6].Value = "Date of Birth";
                worksheet.Cells[1, 7].Value = "Gender (1=Nam, 0=Nữ)";
                worksheet.Cells[1, 8].Value = "Phone Number";
                worksheet.Cells[1, 9].Value = "Address";

                // Tạo file và trả về
                var fileBytes = package.GetAsByteArray();
                var fileName = $"UserProfileTemplate - {DateTime.Now}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        // GET: Admin/Profiles/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = _context.Profiles.Find(id);

            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using OfficeOpenXml;
using Project_64132989.Areas.Admin.Data;
using Project_64132989.Models.Data;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Project_64132989.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Teacher64132989Controller : Controller
    {
        private readonly Model64132989DbContext _context = new Model64132989DbContext();

        public ActionResult Index()
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AddNewUserModel addNewUserModel, HttpPostedFileBase avatarFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo User
                    User user = new User
                    {
                        user_id = addNewUserModel.user_id,
                        email = addNewUserModel.email,
                        password_hash = BCrypt.Net.BCrypt.HashPassword(addNewUserModel.password),
                        user_type = 2,
                        created_at = DateTime.Now
                    };

                    // Tạo Profile
                    Profile profile = new Profile
                    {
                        user_id = addNewUserModel.user_id,
                        first_name = addNewUserModel.first_name,
                        last_name = addNewUserModel.last_name,
                        date_of_birth = addNewUserModel.date_of_birth,
                        gender = addNewUserModel.gender,
                        phone_number = addNewUserModel.phone_number,
                        address = addNewUserModel.address
                    };

                    // Xử lý upload file avatar
                    if (avatarFile != null && avatarFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(avatarFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Uploads/Avatar"), fileName);
                        avatarFile.SaveAs(path);
                        profile.avatar_path = "/Uploads/Avatar/" + fileName; // Lưu đường dẫn ảnh vào DB
                    }

                    // Lưu vào DB
                    _context.Users.Add(user);
                    _context.Profiles.Add(profile);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                }
            }

            return View(addNewUserModel);
        }

        [HttpPost]
        public JsonResult GetTeacherList(int offset, int limit, string search, string sort, string order)
        {
            var query = _context.Users
                .Where(u => u.user_type == 2)
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
                    gender = u.Profile.gender == 1 ? "Nữ" : (u.Profile.gender == 0 ? "Nam" : "Khác")
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

        public ActionResult UploadTeacherList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadTeacherList(HttpPostedFileBase file)
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
                var filePath = Server.MapPath("~/Uploads/Excel" + Guid.NewGuid() + fileExt);
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
                            password_hash = BCrypt.Net.BCrypt.HashPassword(password),
                            user_type = 2, // Giảng viên
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

        // GET: Admin/Profiles/Edit/5
        public ActionResult Edit(string id)
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
            ViewBag.user_id = new SelectList(_context.Users, "user_id", "email", profile.user_id);
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,first_name,last_name,date_of_birth,gender,phone_number,address,avatar_path")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    var avatar_file = Request.Files[0];

                    if (avatar_file != null && avatar_file.ContentLength > 0)
                    {
                        // Tạo đường dẫn lưu trữ
                        string uploadPath = Server.MapPath("~/Uploads/Avatar/");

                        // Kiểm tra và tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // Tạo tên file mới với GUID
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatar_file.FileName);

                        // Lưu file
                        string filePath = Path.Combine(uploadPath, fileName);
                        avatar_file.SaveAs(filePath);

                        // Gán đường dẫn vào Profile
                        profile.avatar_path = "/Uploads/Avatar/" + fileName;
                    }
                }

                _context.Entry(profile).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(_context.Users, "user_id", "email", profile.user_id);
            return View(profile);
        }

        public ActionResult ChangePassword(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Tạo ViewModel tổng hợp
            var viewModel = new ProfileChangePasswordViewModel
            {
                UserId = user.user_id,
                FirstName = user.Profile?.first_name,
                LastName = user.Profile?.last_name,
                Email = user.email,
                AvatarPath = user.Profile?.avatar_path
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ProfileChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(model.UserId);
                if (user == null)
                {
                    return HttpNotFound();
                }

                try
                {
                    // Lưu mật khẩu mới và gửi trực tiếp cho người dùng
                    string plainPassword = model.NewPassword; // Lấy mật khẩu dạng rõ
                    user.password_hash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
                    _context.SaveChanges();

                    // Gửi email kèm mật khẩu mới
                    string loginLink = Url.Action("Login", "Login64132989", null, Request.Url.Scheme);
                    SendChangePasswordEmail(user.email, plainPassword, loginLink);

                    TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi và gửi đến email của người dùng.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Đã có lỗi xảy ra: {ex.Message}");
                }
            }

            return View(model);
        }

        // Gửi email kèm mật khẩu mới
        private void SendChangePasswordEmail(string email, string newPassword, string loginLink)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(
                        "vinhveer123@gmail.com",
                        "hles xwcu zukc oveu" // Mật khẩu ứng dụng Gmail
                    );

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("vinhveer123@gmail.com"),
                        Subject = "Mật khẩu mới của bạn",
                        Body = $@"
                            <p>Xin chào,</p>
                            <p>Quản trị viên vừa thay đổi mật khẩu của bạn trong hệ thống NTU_UTMS.</p>
                            <p><b>Mật khẩu mới của bạn là:</b> <span style='color:blue;'>{newPassword}</span></p>
                            <p>Bạn có thể đăng nhập bằng mật khẩu mới bằng cách nhấn vào liên kết dưới đây:</p>
                            <a href='{loginLink}'>Đăng nhập ngay</a>
                            <p>Nếu bạn không yêu cầu thay đổi này, vui lòng liên hệ với quản trị viên ngay lập tức.</p>
                            <p>Trân trọng,</p>
                            <p>NTU_UTMS</p>
                        ",
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(email);

                    client.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi khi gửi email: {ex.Message}");
            }
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
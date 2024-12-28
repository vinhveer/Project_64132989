using OfficeOpenXml; // Sử dụng EPPlus 4.x hoặc ClosedXML
using OfficeOpenXml.Style;
using Project_64132989.Areas.Admin.Data;
using Project_64132989.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Project_64132989.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Student64132989Controller : Controller
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
                        user_type = 1,
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

                    Student student = new Student
                    {
                        user_id = addNewUserModel.user_id,
                        program_id = addNewUserModel.program_id
                    };

                    // Lưu vào DB
                    _context.Users.Add(user);
                    _context.Profiles.Add(profile);
                    _context.Students.Add(student);
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
        public JsonResult GetStudentList(int offset, int limit, string search, string sort, string order)
        {
            var query = _context.Users
                .Where(u => u.user_type == 1)
                .Include(u => u.Profile)
                .Include(u => u.Student)
                .Include(u => u.Student.TrainingProgram)
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
                    if (user.Student != null)
                        _context.Students.Remove(user.Student);
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

        [HttpPost]
        public ActionResult DeleteMultiple(string[] userIds)
        {
            try
            {
                if (userIds == null || userIds.Length == 0)
                {
                    return Json(new { success = false, message = "Không có sinh viên nào được chọn" });
                }

                var usersToDelete = _context.Users
                    .Include(u => u.Profile)
                    .Where(u => userIds.Contains(u.user_id))
                    .ToList();

                if (!usersToDelete.Any())
                {
                    return Json(new { success = false, message = "Không tìm thấy sinh viên" });
                }

                // Remove associated profiles first
                foreach (var user in usersToDelete.Where(u => u.Profile != null))
                {
                    if (user.Student != null)
                        _context.Students.Remove(user.Student);

                    _context.Profiles.Remove(user.Profile);
                }

                // Remove users
                _context.Users.RemoveRange(usersToDelete);
                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = $"Đã xóa thành công {usersToDelete.Count} sinh viên"
                });
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
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            for (int row = 2; row <= rowCount; row++)
                            {
                                // Đọc các trường bắt buộc từ file
                                var userId = worksheet.Cells[row, 1]?.Text?.Trim();
                                var email = worksheet.Cells[row, 2]?.Text?.Trim();
                                var password = worksheet.Cells[row, 3]?.Text?.Trim();
                                var firstName = worksheet.Cells[row, 4]?.Text?.Trim();
                                var lastName = worksheet.Cells[row, 5]?.Text?.Trim();
                                var program_id = worksheet.Cells[row, 6]?.Text?.Trim();

                                // Kiểm tra các trường bắt buộc
                                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) ||
                                    string.IsNullOrEmpty(password) || string.IsNullOrEmpty(firstName) ||
                                    string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(program_id))
                                {
                                    ViewBag.ErrorMessage = $"Dòng {row}: Thiếu thông tin bắt buộc. Vui lòng kiểm tra lại file.";
                                    transaction.Rollback();
                                    return View();
                                }

                                // Kiểm tra trùng lặp user_id
                                if (_context.Users.Any(u => u.user_id == userId))
                                {
                                    ViewBag.ErrorMessage = $"Dòng {row}: User ID '{userId}' đã tồn tại trong hệ thống.";
                                    transaction.Rollback();
                                    return View();
                                }

                                // Đọc các trường không bắt buộc
                                var dobText = worksheet.Cells[row, 7]?.Text?.Trim();
                                var genderText = worksheet.Cells[row, 8]?.Text?.Trim();
                                var phoneNumber = worksheet.Cells[row, 9]?.Text?.Trim();
                                var address = worksheet.Cells[row, 10]?.Text?.Trim();

                                // Tạo User
                                var user = new User
                                {
                                    user_id = userId,
                                    email = email,
                                    password_hash = BCrypt.Net.BCrypt.HashPassword(password),
                                    user_type = 1, // Sinh viên
                                    created_at = DateTime.Now
                                };

                                // Tạo Profile
                                var profile = new Profile
                                {
                                    user_id = userId,
                                    first_name = firstName,
                                    last_name = lastName,
                                    date_of_birth = string.IsNullOrEmpty(dobText) ? null :
                                                  DateTime.TryParse(dobText, out var dob) ? dob : (DateTime?)null,
                                    gender = string.IsNullOrEmpty(genderText) ? null :
                                            byte.TryParse(genderText, out var gender) ? gender : (byte?)null,
                                    phone_number = phoneNumber,
                                    address = address
                                };

                                // Tạo Student
                                var student = new Student
                                {
                                    user_id = userId,
                                    program_id = program_id,
                                    academic_status = 1
                                };

                                // Thêm dữ liệu vào các bảng
                                _context.Users.Add(user);
                                _context.Profiles.Add(profile);
                                _context.Students.Add(student);
                            }

                            // Lưu thay đổi vào cơ sở dữ liệu
                            _context.SaveChanges();
                            transaction.Commit();

                            // Xóa file tạm sau khi import thành công
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }

                            ViewBag.Message = "Tải lên danh sách thành công!";
                            return View();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ViewBag.ErrorMessage = $"Có lỗi xảy ra: {ex.Message}";
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
                worksheet.Cells[1, 1].Value = "User ID";
                worksheet.Cells[1, 2].Value = "Email";
                worksheet.Cells[1, 3].Value = "Password";
                worksheet.Cells[1, 4].Value = "First Name";
                worksheet.Cells[1, 5].Value = "Last Name";
                worksheet.Cells[1, 6].Value = "Program ID";
                worksheet.Cells[1, 7].Value = "Date of Birth";
                worksheet.Cells[1, 8].Value = "Gender (1=Nam, 0=Nữ)";
                worksheet.Cells[1, 9].Value = "Phone Number";
                worksheet.Cells[1, 10].Value = "Address";

                // Format tiêu đề
                using (var range = worksheet.Cells[1, 1, 1, 10])
                {
                    range.Style.Font.Bold = true;
                }

                // Tạo file và trả về
                var fileBytes = package.GetAsByteArray();
                var fileName = $"StudentTemplate_{DateTime.Now:yyyyMMdd}.xlsx";
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
            Student student = _context.Students.Find(id);

            AddNewUserModel addNewUserModel = new AddNewUserModel
            {
                user_id = profile.user_id,
                first_name = profile.first_name,
                last_name = profile.last_name,
                date_of_birth = profile.date_of_birth,
                gender = profile.gender,
                phone_number = profile.phone_number,
                address = profile.address,
                avatar_path = profile.avatar_path,
                email = profile.User.email,
                program_name = student.TrainingProgram.program_name
            };

            if (profile == null || student == null)
            {
                return HttpNotFound();
            }
            return View(addNewUserModel);
        }

        // GET: Admin/Profiles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Profile profile = _context.Profiles.Find(id);
            Student student = _context.Students.Find(id);

            // Kiểm tra null cho cả profile và student
            if (profile == null || student == null)
            {
                return Content("không tìm thấy {id}");
            }

            // Lấy thông tin program_name
            var program_name = _context.TrainingPrograms.Find(student.program_id)?.program_name;
            ViewBag.ProgramName = program_name;

            AddNewUserModel addNewUserModel = new AddNewUserModel
            {
                user_id = profile.user_id,
                first_name = profile.first_name,
                last_name = profile.last_name,
                date_of_birth = profile.date_of_birth,
                gender = profile.gender,
                phone_number = profile.phone_number,
                address = profile.address,
                avatar_path = profile.avatar_path,
                email = profile.User.email,
                program_name = program_name
            };

            ViewBag.user_id = new SelectList(_context.Users, "user_id", "email", profile.user_id);
          
            return View(addNewUserModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,first_name,last_name,date_of_birth,gender,phone_number,address,avatar_path,program_id")] AddNewUserModel addNewUserModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.user_id = new SelectList(_context.Users, "user_id", "email", addNewUserModel.user_id);
                return View(addNewUserModel);
            }

            try
            {
                // Tìm profile và student hiện có
                var profile = _context.Profiles.Find(addNewUserModel.user_id);
                var student = _context.Students.Find(addNewUserModel.user_id);

                if (profile == null || student == null)
                {
                    return HttpNotFound();
                }

                // Xử lý upload ảnh đại diện
                if (Request.Files.Count > 0)
                {
                    var avatar_file = Request.Files[0];

                    if (avatar_file != null && avatar_file.ContentLength > 0)
                    {
                        // Kiểm tra định dạng file
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(avatar_file.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("", "Chỉ chấp nhận file ảnh có định dạng: .jpg, .jpeg, .png, .gif");
                            ViewBag.user_id = new SelectList(_context.Users, "user_id", "email", addNewUserModel.user_id);
                            return View(addNewUserModel);
                        }

                        // Kiểm tra kích thước file (ví dụ: tối đa 5MB)
                        if (avatar_file.ContentLength > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("", "Kích thước file không được vượt quá 5MB");
                            ViewBag.user_id = new SelectList(_context.Users, "user_id", "email", addNewUserModel.user_id);
                            return View(addNewUserModel);
                        }

                        string uploadPath = Server.MapPath("~/Uploads/Avatar/");

                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(profile.avatar_path))
                        {
                            string oldFilePath = Server.MapPath("~" + profile.avatar_path);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // Tạo tên file mới với GUID
                        string fileName = Guid.NewGuid().ToString() + fileExtension;
                        string filePath = Path.Combine(uploadPath, fileName);
                        avatar_file.SaveAs(filePath);

                        profile.avatar_path = "/Uploads/Avatar/" + fileName;
                    }
                }

                // Cập nhật thông tin profile
                profile.first_name = addNewUserModel.first_name;
                profile.last_name = addNewUserModel.last_name;
                profile.date_of_birth = addNewUserModel.date_of_birth;
                profile.gender = addNewUserModel.gender;
                profile.phone_number = addNewUserModel.phone_number;
                profile.address = addNewUserModel.address;

                // Cập nhật thông tin student
                student.program_id = addNewUserModel.program_id;

                // Lưu thay đổi
                _context.Entry(profile).State = EntityState.Modified;
                _context.Entry(student).State = EntityState.Modified;
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật thông tin. Vui lòng thử lại.");
                // Log error (should use proper logging mechanism)
                System.Diagnostics.Debug.WriteLine(ex.Message);

                ViewBag.user_id = new SelectList(_context.Users, "user_id", "email", addNewUserModel.user_id);
                return View(addNewUserModel);
            }
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
                    string loginLink = Url.Action("Login", "Login64132989", new { area = "" }, Request.Url.Scheme);
                    SendChangePasswordEmail(user.email, plainPassword, loginLink);

                    TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi và gửi đến email của người dùng.";
                    return View(model);
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
                            <p>NTU_UMS</p>
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

        [HttpGet]
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
                var query = _context.TrainingPrograms
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
                else
                {
                    // Sắp xếp mặc định nếu không có sắp xếp nào được chỉ định
                    query = query.OrderBy(t => t.program_id);
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

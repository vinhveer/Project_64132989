using Project_64132989.Models.Data;
using Project_64132989.Models.Views;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace Project_64132989.Controllers
{
    public class Portal64132989Controller : Controller
    {
        private readonly Model64132989DbContext _context;

        public Portal64132989Controller()
        {
            _context = new Model64132989DbContext();
        }

        // Trang chính của portal
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        // Thay đổi mật khẩu
        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lấy người dùng hiện tại
            var currentUser = _context.Users.FirstOrDefault(u => u.email == User.Identity.Name);

            if (currentUser == null)
            {
                ModelState.AddModelError("", "Người dùng không tồn tại.");
                return View(model);
            }

            // Kiểm tra mật khẩu cũ
            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, currentUser.password_hash))
            {
                ModelState.AddModelError("", "Mật khẩu hiện tại không chính xác.");
                return View(model);
            }

            // Mã hóa mật khẩu mới
            currentUser.password_hash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            try
            {
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("ChangePassword");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(model);
            }
        }

        // Quên mật khẩu
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Nếu email hợp lệ, bạn sẽ nhận được hướng dẫn reset mật khẩu.");
                return View(model);
            }

            // Tạo reset token
            string resetToken = GenerateResetToken();

            // Lưu token vào database
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordExpires = DateTime.UtcNow.AddHours(1); // Token hết hạn sau 1 giờ

            try
            {
                _context.SaveChanges();

                // Gửi email chứa link reset
                string resetLink = Url.Action("ResetPassword", "Portal64132989",
                    new { token = resetToken }, protocol: Request.Url.Scheme);
                SendResetPasswordEmail(user.email, resetLink);

                TempData["SuccessMessage"] = "Hướng dẫn reset mật khẩu đã được gửi tới email của bạn.";
                return RedirectToAction("Alert");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(model);
            }
        }

        // Đặt lại mật khẩu
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string token)
        {
            // Kiểm tra tính hợp lệ của token trong database
            var user = _context.Users.FirstOrDefault(u =>
                u.ResetPasswordToken == token &&
                u.ResetPasswordExpires > DateTime.UtcNow);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Link reset mật khẩu không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u =>
                u.ResetPasswordToken == model.Token &&
                u.ResetPasswordExpires > DateTime.UtcNow);

            if (user == null)
            {
                ModelState.AddModelError("", "Link reset mật khẩu không hợp lệ hoặc đã hết hạn.");
                return View(model);
            }

            // Đặt mật khẩu mới
            user.password_hash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            // Xóa token sau khi reset
            user.ResetPasswordToken = null;
            user.ResetPasswordExpires = null;

            try
            {
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công.";
                return RedirectToAction("Alert");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(model);
            }
        }

        // Sinh token reset password
        private string GenerateResetToken()
        {
            return Guid.NewGuid().ToString("N") +
                   DateTime.UtcNow.Ticks.ToString();
        }

        // Gửi email reset password
        private void SendResetPasswordEmail(string email, string resetLink)
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
                        "hles xwcu zukc oveu"
                    );

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("vinhveer123@gmail.com"),
                        Subject = "Khởi tạo lại mật khẩu đăng nhập",
                        Body = $@"
                            <p>Xin chào {email},</p>
                            <p>Bạn vừa yêu cầu khởi tạo lại mật khẩu đăng nhập hệ thống NTU_UTMS. Nhấn vào liên kết dưới đây để đặt lại mật khẩu:</p>
                            <a href='{resetLink}'>Nhấn vào đây để đặt lại mật khẩu</a>
                            <p><b>Chú ý:</b> Liên kết này sẽ hết hạn sau 1 giờ. Nếu bạn không yêu cầu khởi tạo lại mật khẩu, vui lòng bỏ qua email này.</p>
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

        [AllowAnonymous]
        public ActionResult Alert()
        {
            return View();
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
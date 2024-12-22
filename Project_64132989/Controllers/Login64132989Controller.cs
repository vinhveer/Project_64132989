using Project_64132989.Models.Data;
using Project_64132989.Models.Views;
using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace Project_64132989.Controllers
{
    public class Login64132989Controller : Controller
    {
        private readonly Model64132989DbContext _db = new Model64132989DbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find the user by email
            User userInDb = _db.Users.FirstOrDefault(u => u.email == model.Email);

            if (userInDb == null)
            {
                ViewBag.LoginResult = "Tài khoản không tồn tại";
                return View(model);
            }

            // Verify the password
            if (!BCrypt.Net.BCrypt.Verify(model.Password, userInDb.password_hash))
            {
                ViewBag.LoginResult = "Mật khẩu không đúng";
                return View(model);
            }

            /* 
             Role:
                1 - Student, 2 - Teacher
                3 - TrainingOfficer, 4 - Admin
             */

            string role = userInDb.user_type == 1 ? "Student" :
                          userInDb.user_type == 2 ? "Teacher" :
                          userInDb.user_type == 3 ? "TrainingOfficer" :
                          userInDb.user_type == 4 ? "Admin" : "Unknown";

            // Create FormsAuthenticationTicket
            var authTicket = new FormsAuthenticationTicket(
                1, userInDb.user_id, System.DateTime.Now,
                System.DateTime.Now.AddMinutes(30),
                model.RememberMe, role
            );

            // Encrypt and set cookie
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = Request.IsSecureConnection
            };

            if (model.RememberMe)
            {
                authCookie.Expires = authTicket.Expiration;
            }

            Response.Cookies.Add(authCookie);

            switch (role)
            {
                case "Student":
                    return RedirectToAction("Index", "Home64132989", new { area = "Student" });
                case "Teacher":
                    return RedirectToAction("Index", "Home64132989", new { area = "Teacher" });
                case "TrainingOfficer":
                    return RedirectToAction("Index", "Home64132989", new { area = "TrainingOfficer" });
                case "Admin":
                    return RedirectToAction("Index", "Home64132989", new { area = "Admin" });
                default:
                    return RedirectToAction("Index", "Home64132989");
            }
        }

        [Authorize]
        public ActionResult Direct()
        {
            // Lấy role từ AuthTicket
            var authTicket = ((FormsIdentity)User.Identity).Ticket;
            string role = authTicket.UserData;

            switch (role)
            {
                case "Student":
                    return RedirectToAction("Index", "Home64132989", new { area = "Student" });
                case "Teacher":
                    return RedirectToAction("Index", "Home64132989", new { area = "Teacher" });
                case "TrainingOfficer":
                    return RedirectToAction("Index", "Home64132989", new { area = "TrainingOfficer" });
                case "Admin":
                    return RedirectToAction("Index", "Home64132989", new { area = "Admin" });
                default:
                    return RedirectToAction("Index", "Home64132989");
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            // Đăng xuất bằng FormsAuthentication
            FormsAuthentication.SignOut();

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Index", "Login64132989");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
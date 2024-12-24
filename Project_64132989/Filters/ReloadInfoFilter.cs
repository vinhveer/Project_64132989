using Project_64132989.Models.Data;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project_64132989.Filters
{
    public class ReloadInfoFilter : ActionFilterAttribute
    {
        private readonly Model64132989DbContext _db;
        private readonly string[] _excludedControllers;

        public ReloadInfoFilter(params string[] excludedControllers)
        {
            _db = new Model64132989DbContext();
            _excludedControllers = excludedControllers.Select(c => c.ToLower()).ToArray();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var area = filterContext.RouteData.DataTokens["area"] as string;
                var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();

                // Kiểm tra xem controller có trong danh sách loại trừ không
                bool isExcludedController = _excludedControllers.Contains(controllerName) && string.IsNullOrEmpty(area);

                // Lấy thông tin identity
                FormsIdentity formsIdentity = filterContext.HttpContext.User.Identity as FormsIdentity;
                bool isAuthenticated = formsIdentity != null;

                // Nếu là controller được loại trừ và chưa đăng nhập
                if (isExcludedController && !isAuthenticated)
                {
                    // Cho phép tiếp tục mà không cần kiểm tra đăng nhập
                    base.OnActionExecuting(filterContext);
                    return;
                }

                // Nếu không phải controller loại trừ và chưa đăng nhập
                if (!isExcludedController && !isAuthenticated)
                {
                    throw new UnauthorizedAccessException("AuthTicket không hợp lệ hoặc chưa đăng nhập.");
                }

                // Nếu đã đăng nhập (có FormTicket), kiểm tra xem session đã có thông tin chưa
                if (isAuthenticated)
                {
                    var session = filterContext.HttpContext.Session;
                    // Kiểm tra xem session đã có thông tin chưa
                    if (session["UserId"] == null)
                    {
                        // Load thông tin vào session
                        LoadUserInfo(filterContext, formsIdentity);
                    }
                }

                base.OnActionExecuting(filterContext);
            }
            catch (UnauthorizedAccessException)
            {
                // Chuyển hướng đến trang đăng nhập chỉ khi không phải controller được loại trừ
                filterContext.Result = new RedirectResult("https://localhost:44348/Login64132989/Index");
            }
        }

        private void LoadUserInfo(ActionExecutingContext filterContext, FormsIdentity formsIdentity)
        {
            var authTicket = formsIdentity.Ticket;
            string userData = authTicket.Name;

            if (string.IsNullOrEmpty(userData))
            {
                throw new InvalidOperationException("Dữ liệu người dùng không hợp lệ.");
            }

            // Tìm người dùng trong database
            var userInDb = _db.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.user_id.ToString().Equals(userData, StringComparison.OrdinalIgnoreCase));

            if (userInDb == null)
            {
                throw new InvalidOperationException("Người dùng không tồn tại.");
            }

            // Tìm hồ sơ người dùng
            var userProfile = _db.Profiles
                .AsNoTracking()
                .FirstOrDefault(p => p.user_id.ToString().Equals(userData, StringComparison.OrdinalIgnoreCase));

            if (userProfile == null)
            {
                throw new InvalidOperationException("Hồ sơ người dùng không tồn tại.");
            }

            // Lấy thông tin học kỳ
            var semester = _db.Semesters
                .AsNoTracking()
                .FirstOrDefault(s => s.status == 1);

            if (semester == null)
            {
                throw new InvalidOperationException("Học kỳ hiện tại không tồn tại.");
            }

            // Lưu thông tin vào Session
            var session = filterContext.HttpContext.Session;
            session["UserEmail"] = userInDb.email;
            session["UserFullName"] = $"{userProfile.last_name} {userProfile.first_name}";
            session["UserId"] = userInDb.user_id;
            session["UserRole"] = authTicket.UserData;
            session["Avatar"] = userProfile.avatar_path;
            session["SemesterId"] = semester.semester_id;
            session["SemesterName"] = semester.semester_name;
        }
    }
}

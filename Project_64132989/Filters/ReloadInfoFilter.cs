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
        private readonly Model64132989DbContext _db; // Thay YourDbContext bằng lớp DbContext của bạn.
        private readonly string[] _excludedControllers; // Danh sách các controller cần bỏ qua.

        public ReloadInfoFilter(params string[] excludedControllers)
        {
            _db = new Model64132989DbContext(); // Khởi tạo DbContext
            _excludedControllers = excludedControllers.Select(c => c.ToLower()).ToArray(); // Đổi tên controller thành chữ thường để so sánh dễ dàng.
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var area = filterContext.RouteData.DataTokens["area"] as string;
                // Lấy tên controller hiện tại
                var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();

                // Nếu controller nằm trong danh sách bị loại trừ, bỏ qua filter
                if (_excludedControllers.Contains(controllerName) && string.IsNullOrEmpty(area))
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }

                // Lấy thông tin từ User.Identity và kiểm tra
                if (!(filterContext.HttpContext.User.Identity is FormsIdentity formsIdentity))
                {
                    throw new UnauthorizedAccessException("AuthTicket không hợp lệ hoặc chưa đăng nhập.");
                }

                var authTicket = formsIdentity.Ticket;
                string userData = authTicket.Name;

                if (string.IsNullOrEmpty(userData))
                {
                    throw new InvalidOperationException("Dữ liệu người dùng không hợp lệ.");
                }

                Debug.WriteLine(userData);

                // Tìm người dùng trong database
                var userInDb = _db.Users
                    .AsNoTracking() // Không cần tracking vì không thay đổi dữ liệu
                    .FirstOrDefault(u => u.user_id.ToString().Equals(userData, StringComparison.OrdinalIgnoreCase));

                if (userInDb == null)
                {
                    throw new InvalidOperationException("Người dùng không tồn tại.");
                }

                // Tìm hồ sơ người dùng trong database dựa trên user_id
                var userProfile = _db.Profiles
                    .AsNoTracking() // Không cần tracking
                    .FirstOrDefault(p => p.user_id.ToString().Equals(userData, StringComparison.OrdinalIgnoreCase)); // So sánh trực tiếp với user_id từ userInDb

                if (userProfile == null)
                {
                    throw new InvalidOperationException("Hồ sơ người dùng không tồn tại.");
                }


                // Lưu thông tin vào Session
                var session = filterContext.HttpContext.Session;
                session["UserEmail"] = userInDb.email;
                session["UserFullName"] = $"{userProfile.last_name} {userProfile.first_name}";
                session["UserId"] = userInDb.user_id;
                session["UserRole"] = authTicket.UserData; // Sử dụng UserData cho vai trò hoặc thay đổi nếu cần
                session["Avatar"] = userProfile.avatar_path;

                base.OnActionExecuting(filterContext);
            }
            catch (UnauthorizedAccessException)
            {
                // Chuyển hướng đến trang đăng nhập nếu không đăng nhập hoặc AuthTicket không hợp lệ
                filterContext.Result = new RedirectResult("https://localhost:44348/Login64132989/Index");
            }
        }
    }
}

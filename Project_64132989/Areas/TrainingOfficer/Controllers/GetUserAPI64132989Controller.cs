using Project_64132989.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132989.Areas.TrainingOfficer.Controllers
{
    [Authorize(Roles = "TrainingOfficer")]
    public class GetUserAPI64132989Controller : Controller
    {
        private readonly Model64132989DbContext _context = new Model64132989DbContext();

        public JsonResult GetStudentList(int offset = 0, int limit = 10, string search = "", string sort = "", string order = "", string classId = "")
        {
            try
            {
                // Khởi tạo query cơ bản
                var query = _context.Users
                    .Where(u => u.user_type == 1)
                    .Include(u => u.Profile)
                    .Include(u => u.Student)
                    .AsQueryable();

                // Nếu có classId, lọc theo lớp. Ngược lại lấy tất cả sinh viên
                if (!string.IsNullOrEmpty(classId))
                {
                    query = query.Where(u => u.Student.administrative_class_id == classId);
                }

                // Thêm điều kiện tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(u =>
                        u.user_id.ToLower().Contains(search) ||
                        (u.Profile.first_name + " " + u.Profile.last_name).ToLower().Contains(search) ||
                        u.email.ToLower().Contains(search));
                }

                // Sắp xếp mặc định nếu không có sort
                if (string.IsNullOrEmpty(sort))
                {
                    query = query.OrderBy(u => u.user_id);
                }
                else
                {
                    // Áp dụng sắp xếp theo trường được chọn
                    switch (sort.ToLower())
                    {
                        case "userid":
                            query = order == "asc" ? query.OrderBy(u => u.user_id) : query.OrderByDescending(u => u.user_id);
                            break;
                        case "fullname":
                            query = order == "asc"
                                ? query.OrderBy(u => u.Profile.last_name).ThenBy(u => u.Profile.first_name)
                                : query.OrderByDescending(u => u.Profile.last_name).ThenByDescending(u => u.Profile.first_name);
                            break;
                        case "email":
                            query = order == "asc" ? query.OrderBy(u => u.email) : query.OrderByDescending(u => u.email);
                            break;
                        default:
                            query = query.OrderBy(u => u.user_id);
                            break;
                    }
                }

                // Đếm tổng số bản ghi
                var total = query.Count();

                // Lấy dữ liệu đã được sắp xếp và phân trang
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
                        gender = u.Profile.gender == 1 ? "Nữ" : (u.Profile.gender == 0 ? "Nam" : "Khác"),
                        classId = u.Student.administrative_class_id
                    })
                    .ToList();

                return Json(new { total = total, rows = students }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTeacherList(int offset, int limit, string search, string sort, string order)
        {
            try
            {
                var query = _context.Users
                    .Where(u => u.user_type == 2)  // 2 là giáo viên
                    .Include(u => u.Profile);

                // Thêm điều kiện tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(u =>
                        u.user_id.ToLower().Contains(search) ||
                        (u.Profile.first_name + " " + u.Profile.last_name).ToLower().Contains(search) ||
                        u.email.ToLower().Contains(search)
                    );
                }

                // Thêm sắp xếp mặc định nếu không có sort
                if (string.IsNullOrEmpty(sort))
                {
                    sort = "userId";
                }

                // Áp dụng sắp xếp
                switch (sort.ToLower())
                {
                    case "userid":
                        query = order == "desc" ?
                            query.OrderByDescending(u => u.user_id) :
                            query.OrderBy(u => u.user_id);
                        break;
                    case "fullname":
                        query = order == "desc" ?
                            query.OrderByDescending(u => u.Profile.first_name + " " + u.Profile.last_name) :
                            query.OrderBy(u => u.Profile.first_name + " " + u.Profile.last_name);
                        break;
                    case "email":
                        query = order == "desc" ?
                            query.OrderByDescending(u => u.email) :
                            query.OrderBy(u => u.email);
                        break;
                    default:
                        query = query.OrderBy(u => u.user_id);  // Sắp xếp mặc định
                        break;
                }

                // Đếm tổng số bản ghi
                var total = query.Count();

                // Lấy dữ liệu theo offset và limit
                var teachers = query
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

                return Json(new { total = total, rows = teachers }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log lỗi
                System.Diagnostics.Debug.WriteLine($"Error in GetTeacherList: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
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
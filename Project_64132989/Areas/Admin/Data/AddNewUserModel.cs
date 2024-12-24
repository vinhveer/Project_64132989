using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_64132989.Areas.Admin.Data
{
    public class AddNewUserModel
    {
        [StringLength(10)]
        [Display(Name = "Mã số")]
        public string user_id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Tên")]
        public string first_name { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Họ đệm")]
        public string last_name { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Ngày sinh")]
        public DateTime? date_of_birth { get; set; }

        [Display(Name = "Giới tính")]
        public byte? gender { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string phone_number { get; set; }

        [Display(Name = "Địa chỉ")]
        public string address { get; set; }

        [StringLength(255)]
        [Display(Name = "Ảnh đại diện")]
        public string avatar_path { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Mật khẩu")]
        public string password { get; set; }

        [StringLength(255)]
        [Display(Name = "Phòng ban")]
        public string department_id { get; set; }

        [StringLength(255)]
        [Display(Name = "Phòng ban trực thuộc")]
        public string department_name { get; set; }

        [StringLength(255)]
        [Display(Name = "Chương trình học")]
        public string program_id { get; set; }

        [StringLength(255)]
        [Display(Name = "Chương trình học trực thuộc")]
        public string program_name { get; set; }
    }
}
namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class User
    {
        [Key]
        [StringLength(10)]
        [Required(ErrorMessage = "Mã người dùng không được để trống")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Mã người dùng chỉ được chứa chữ cái và số")]
        [DisplayName("Mã người dùng")]
        public string user_id { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [DisplayName("Email")]
        public string email { get; set; }

        [StringLength(255)]
        [DisplayName("Mật khẩu (đã mã hóa)")]
        public string password_hash { get; set; }

        [Required(ErrorMessage = "Loại tài khoản không được để trống")]
        [DisplayName("Loại tài khoản")]
        public byte user_type { get; set; }

        [DisplayName("Trạng thái tài khoản")]
        public byte? account_status { get; set; }

        [DisplayName("Lần đăng nhập cuối")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? last_login { get; set; }

        [DisplayName("Ngày tạo")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? created_at { get; set; } = DateTime.UtcNow;

        [StringLength(255)]
        [DisplayName("Token đặt lại mật khẩu")]
        public string ResetPasswordToken { get; set; }

        [DisplayName("Thời hạn token")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? ResetPasswordExpires { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual Student Student { get; set; }

        public virtual Teacher Teacher { get; set; }
    }
}
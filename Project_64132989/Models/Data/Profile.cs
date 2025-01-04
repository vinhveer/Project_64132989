namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class Profile
    {
        [Key]
        [StringLength(10)]
        [Required(ErrorMessage = "Mã người dùng không được để trống")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Mã người dùng chỉ được chứa chữ cái và số")]
        [DisplayName("Mã người dùng")]
        public string user_id { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên phải từ 2 đến 50 ký tự")]
        [DisplayName("Tên")]
        public string first_name { get; set; }

        [Required(ErrorMessage = "Họ không được để trống")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Họ phải từ 2 đến 50 ký tự")]
        [DisplayName("Họ đệm")]
        public string last_name { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Ngày sinh")]
        public DateTime? date_of_birth { get; set; }

        [Range(0, 2, ErrorMessage = "Giới tính không hợp lệ")]
        [DisplayName("Giới tính")]
        public byte? gender { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số điện thoại chỉ được chứa số")]
        [DisplayName("Số điện thoại")]
        public string phone_number { get; set; }

        [DisplayName("Địa chỉ")]
        public string address { get; set; }

        [StringLength(255)]
        [DisplayName("Ảnh đại diện")]
        public string avatar_path { get; set; }

        public virtual User User { get; set; }
    }
}
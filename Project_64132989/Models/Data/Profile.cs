namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Profile
    {
        [Key]
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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
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

        public virtual User User { get; set; }
    }
}

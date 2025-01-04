namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class TrainingProgram
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TrainingProgram()
        {
            Students = new HashSet<Student>();
        }

        [Key]
        [StringLength(10)]
        [Required(ErrorMessage = "Mã chương trình đào tạo không được để trống")]
        [DisplayName("Mã chương trình đào tạo")]
        public string program_id { get; set; }

        [Required(ErrorMessage = "Mã khoa không được để trống")]
        [StringLength(10)]
        [DisplayName("Mã khoa")]
        public string department_id { get; set; }

        [Required(ErrorMessage = "Tên chương trình đào tạo không được để trống")]
        [DisplayName("Tên chương trình đào tạo")]
        public string program_name { get; set; }

        [Required(ErrorMessage = "Tổng số tín chỉ không được để trống")]
        [Range(120, 180, ErrorMessage = "Tổng số tín chỉ phải từ 120 đến 180")]
        [DisplayName("Tổng số tín chỉ")]
        public int total_credits { get; set; }

        [Required(ErrorMessage = "Phiên bản không được để trống")]
        [StringLength(20)]
        [DisplayName("Phiên bản")]
        public string version { get; set; }

        [Required(ErrorMessage = "Năm bắt đầu không được để trống")]
        [DisplayName("Năm bắt đầu")]
        public int start_year { get; set; }

        [DisplayName("Năm kết thúc")]
        public int? end_year { get; set; }

        [Range(0, 255, ErrorMessage = "Trạng thái phải từ 0 đến 255")]
        [DisplayName("Trạng thái")]
        public byte? status { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student> Students { get; set; }
    }
}
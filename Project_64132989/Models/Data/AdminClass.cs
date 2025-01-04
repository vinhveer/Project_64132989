namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AdminClass
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AdminClass()
        {
            Students = new HashSet<Student>();
        }

        [Key]
        [StringLength(10)]
        [Display(Name = "Mã lớp")]
        [Required(ErrorMessage = "Vui lòng nhập mã lớp")]
        public string class_id { get; set; }

        [StringLength(100)]
        [Display(Name = "Tên lớp")]
        [Required(ErrorMessage = "Vui lòng nhập tên lớp")]
        public string class_name { get; set; }

        [Required(ErrorMessage = "Mã phòng ban chưa được chọn")]
        [StringLength(10)]
        [Display(Name = "Mã phòng ban")]
        public string department_id { get; set; }

        [StringLength(10)]
        [Display(Name = "Mã giáo viên chủ nhiệm")]
        public string advisor_teacher_id { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? created_date { get; set; }

        [Display(Name = "Trạng thái")]
        public byte? status { get; set; }

        public virtual Department Department { get; set; }

        public virtual Teacher Teacher { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student> Students { get; set; }
    }
}

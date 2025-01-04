namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class Semester
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Semester()
        {
            CourseOfferings = new HashSet<CourseOffering>();
            StudentLearningPlans = new HashSet<StudentLearningPlan>();
        }

        [Key]
        [DisplayName("Mã học kỳ")]
        public int semester_id { get; set; }

        [Required(ErrorMessage = "Tên học kỳ không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên học kỳ phải từ 3 đến 50 ký tự")]
        [RegularExpression(@"^[A-Za-z0-9\s\-]+$", ErrorMessage = "Tên học kỳ chỉ được chứa chữ cái, số, khoảng trắng và dấu gạch ngang")]
        [DisplayName("Tên học kỳ")]
        public string semester_name { get; set; }

        [DisplayName("Ngày bắt đầu đăng ký")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? registration_start_date { get; set; }

        [DisplayName("Ngày kết thúc đăng ký")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? registration_end_date { get; set; }

        [Range(0, 255, ErrorMessage = "Trạng thái phải từ 0 đến 255")]
        [DisplayName("Trạng thái")]
        public byte? status { get; set; }

        [DisplayName("Ngày bắt đầu đăng ký môn học")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? course_registration_start { get; set; }

        [DisplayName("Ngày kết thúc đăng ký môn học")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? course_registration_end { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseOffering> CourseOfferings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentLearningPlan> StudentLearningPlans { get; set; }
    }
}
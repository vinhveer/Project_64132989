namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class Student
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Student()
        {
            StudentCourseRegistrations = new HashSet<StudentCourseRegistration>();
            StudentLearningPlans = new HashSet<StudentLearningPlan>();
        }

        [Key]
        [StringLength(10)]
        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mã sinh viên phải gồm 10 chữ số")]
        [DisplayName("Mã sinh viên")]
        public string user_id { get; set; }

        [Required(ErrorMessage = "Mã chương trình đào tạo không được để trống")]
        [StringLength(10)]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Mã chương trình đào tạo chỉ được chứa chữ hoa và số")]
        [DisplayName("Mã chương trình đào tạo")]
        public string program_id { get; set; }

        [Required(ErrorMessage = "Năm nhập học không được để trống")]
        [DisplayName("Năm nhập học")]
        public int entry_year { get; set; }

        [DisplayName("Năm tốt nghiệp dự kiến")]
        public int? graduation_expected_year { get; set; }

        [DisplayName("Trạng thái học tập")]
        public byte? academic_status { get; set; }

        [StringLength(10)]
        [DisplayName("Mã lớp hành chính")]
        public string administrative_class_id { get; set; }

        public virtual AdminClass AdminClass { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentCourseRegistration> StudentCourseRegistrations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentLearningPlan> StudentLearningPlans { get; set; }

        public virtual TrainingProgram TrainingProgram { get; set; }

        public virtual User User { get; set; }
    }
}
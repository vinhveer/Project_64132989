namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    [Table("Courses")]
    public partial class Cours
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cours()
        {
            CourseOfferings = new HashSet<CourseOffering>();
            Courses1 = new HashSet<Cours>();
            StudentLearningPlans = new HashSet<StudentLearningPlan>();
            TeacherAssignments = new HashSet<TeacherAssignment>();
        }

        [Key]
        [StringLength(20)]
        [Required(ErrorMessage = "Mã môn học không được để trống")]
        [RegularExpression(@"^[A-Z]{2,4}\d{3,4}$", ErrorMessage = "Mã môn học phải gồm 2-4 chữ hoa theo sau là 3-4 số (Ví dụ: CS101, MATH2001)")]
        [DisplayName("Mã môn học")]
        public string course_id { get; set; }

        [Required(ErrorMessage = "Tên môn học không được để trống")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tên môn học phải từ 3 đến 200 ký tự")]
        [RegularExpression(@"^[A-Za-z0-9\s\-&(),.]+$", ErrorMessage = "Tên môn học chỉ được chứa chữ cái, số, khoảng trắng và dấu câu cơ bản")]
        [DisplayName("Tên môn học")]
        public string course_name { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Mô tả")]
        public string description { get; set; }

        [Required(ErrorMessage = "Số tín chỉ không được để trống")]
        [Range(1, 12, ErrorMessage = "Số tín chỉ phải từ 1 đến 12")]
        [DisplayName("Số tín chỉ")]
        public int credits { get; set; }

        [Required(ErrorMessage = "Mã khoa không được để trống")]
        [StringLength(10)]
        [RegularExpression(@"^[A-Z]{2,10}$", ErrorMessage = "Mã khoa phải gồm 2-10 chữ hoa")]
        [DisplayName("Mã khoa")]
        public string department_id { get; set; }

        [Required(ErrorMessage = "Loại môn học không được để trống")]
        [Range(0, 255, ErrorMessage = "Loại môn học phải từ 0 đến 255")]
        [DisplayName("Loại môn học")]
        public byte course_type { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^[A-Z]{2,4}\d{3,4}$", ErrorMessage = "Mã môn học tiên quyết phải gồm 2-4 chữ hoa theo sau là 3-4 số")]
        [DisplayName("Môn học tiên quyết")]
        public string prerequisite_course_id { get; set; }

        [Range(0, 255, ErrorMessage = "Trạng thái phải từ 0 đến 255")]
        [DisplayName("Trạng thái")]
        public byte? status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseOffering> CourseOfferings { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cours> Courses1 { get; set; }

        public virtual Cours Cours1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentLearningPlan> StudentLearningPlans { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; }
    }
}
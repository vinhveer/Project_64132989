namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class Teacher
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Teacher()
        {
            AdminClasses = new HashSet<AdminClass>();
            CourseOfferings = new HashSet<CourseOffering>();
            TeacherAssignments = new HashSet<TeacherAssignment>();
        }

        [Key]
        [StringLength(10)]
        [Required(ErrorMessage = "Mã giảng viên không được để trống")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Mã giảng viên chỉ được chứa chữ cái và số")]
        [DisplayName("Mã giảng viên")]
        public string user_id { get; set; }

        [Required(ErrorMessage = "Mã khoa không được để trống")]
        [StringLength(10)]
        [DisplayName("Mã khoa")]
        public string department_id { get; set; }

        [StringLength(50)]
        [DisplayName("Học hàm/Học vị")]
        public string academic_rank { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Lĩnh vực nghiên cứu")]
        public string research_areas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdminClass> AdminClasses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseOffering> CourseOfferings { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; }

        public virtual User User { get; set; }
    }
}
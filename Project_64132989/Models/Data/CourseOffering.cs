namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class CourseOffering
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CourseOffering()
        {
            Schedules = new HashSet<Schedule>();
            StudentCourseRegistrations = new HashSet<StudentCourseRegistration>();
        }

        [Key]
        [DisplayName("Mã lớp học phần")]
        public long offering_id { get; set; }

        [Required(ErrorMessage = "Mã môn học không được để trống")]
        [StringLength(20)]
        [DisplayName("Mã môn học")]
        public string course_id { get; set; }

        [Required(ErrorMessage = "Học kỳ không được để trống")]
        [DisplayName("Học kỳ")]
        public int semester_id { get; set; }

        [Required(ErrorMessage = "Mã giảng viên không được để trống")]
        [StringLength(10)]
        [DisplayName("Mã giảng viên")]
        public string teacher_user_id { get; set; }

        [DisplayName("Mã phòng")]
        public int? room_id { get; set; }

        [Required(ErrorMessage = "Sĩ số tối đa không được để trống")]
        [Range(1, 200, ErrorMessage = "Sĩ số tối đa phải từ 1 đến 200")]
        [DisplayName("Sĩ số tối đa")]
        public int max_capacity { get; set; }

        [Range(0, 255, ErrorMessage = "Trạng thái phải từ 0 đến 255")]
        [DisplayName("Trạng thái")]
        public byte? status { get; set; }

        public virtual Cours Cours { get; set; }

        public virtual Room Room { get; set; }

        public virtual Semester Semester { get; set; }

        public virtual Teacher Teacher { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Schedule> Schedules { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentCourseRegistration> StudentCourseRegistrations { get; set; }
    }
}
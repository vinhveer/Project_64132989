namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CourseOffering
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CourseOffering()
        {
            StudentCourseRegistrations = new HashSet<StudentCourseRegistration>();
        }

        [Key]
        public int offering_id { get; set; }

        public long course_id { get; set; }

        public int semester_id { get; set; }

        [Required]
        [StringLength(10)]
        public string teacher_user_id { get; set; }

        public int? room_id { get; set; }

        [StringLength(10)]
        public string section { get; set; }

        public int max_capacity { get; set; }

        public int? current_enrollment { get; set; }

        public byte? offering_status { get; set; }

        public virtual Cours Cours { get; set; }

        public virtual Room Room { get; set; }

        public virtual Semester Semester { get; set; }

        public virtual Teacher Teacher { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentCourseRegistration> StudentCourseRegistrations { get; set; }
    }
}

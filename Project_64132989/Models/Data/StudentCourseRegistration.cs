namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StudentCourseRegistration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StudentCourseRegistration()
        {
            Grades = new HashSet<Grade>();
        }

        [Key]
        public long registration_id { get; set; }

        [Required]
        [StringLength(10)]
        public string student_id { get; set; }

        public long offering_id { get; set; }

        public DateTime? registration_date { get; set; }

        public byte? status { get; set; }

        public virtual CourseOffering CourseOffering { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Grade> Grades { get; set; }

        public virtual Student Student { get; set; }
    }
}

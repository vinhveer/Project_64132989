namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Semester
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Semester()
        {
            CourseOfferings = new HashSet<CourseOffering>();
            StudentLearningPlans = new HashSet<StudentLearningPlan>();
        }

        [Key]
        public int semester_id { get; set; }

        [Required]
        [StringLength(50)]
        public string semester_name { get; set; }

        public DateTime? registration_start_date { get; set; }

        public DateTime? registration_end_date { get; set; }

        public byte? status { get; set; }

        public DateTime? course_registration_start { get; set; }

        public DateTime? course_registration_end { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseOffering> CourseOfferings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentLearningPlan> StudentLearningPlans { get; set; }
    }
}

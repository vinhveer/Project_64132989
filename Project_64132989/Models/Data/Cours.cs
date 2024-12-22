namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Courses")]
    public partial class Cours
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cours()
        {
            CourseOfferings = new HashSet<CourseOffering>();
            Courses1 = new HashSet<Cours>();
            TrainingProgramCourses = new HashSet<TrainingProgramCours>();
        }

        [Key]
        [StringLength(20)]
        public string course_id { get; set; }

        [Required]
        [StringLength(200)]
        public string course_name { get; set; }

        public string description { get; set; }

        public int credits { get; set; }

        [Required]
        [StringLength(10)]
        public string department_id { get; set; }

        public byte course_type { get; set; }

        [StringLength(20)]
        public string prerequisite_course_id { get; set; }

        public byte? status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseOffering> CourseOfferings { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cours> Courses1 { get; set; }

        public virtual Cours Cours1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingProgramCours> TrainingProgramCourses { get; set; }
    }
}

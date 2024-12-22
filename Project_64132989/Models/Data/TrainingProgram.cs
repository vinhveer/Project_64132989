namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TrainingProgram
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TrainingProgram()
        {
            Students = new HashSet<Student>();
            TrainingProgramCourses = new HashSet<TrainingProgramCours>();
        }

        [Key]
        [StringLength(10)]
        public string program_id { get; set; }

        [Required]
        [StringLength(10)]
        public string department_id { get; set; }

        [Required]
        [StringLength(200)]
        public string program_name { get; set; }

        public int total_credits { get; set; }

        [Required]
        [StringLength(20)]
        public string version { get; set; }

        public int start_year { get; set; }

        public int? end_year { get; set; }

        public byte? status { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student> Students { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingProgramCours> TrainingProgramCourses { get; set; }
    }
}

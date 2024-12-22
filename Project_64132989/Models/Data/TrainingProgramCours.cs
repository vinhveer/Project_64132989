namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingProgramCourses")]
    public partial class TrainingProgramCours
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string program_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string course_id { get; set; }

        public int semester_suggest { get; set; }

        public byte course_type { get; set; }

        public int? min_year { get; set; }

        public virtual Cours Cours { get; set; }

        public virtual TrainingProgram TrainingProgram { get; set; }
    }
}

namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StudentLearningPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long learning_plan_id { get; set; }

        [Required]
        [StringLength(20)]
        public string course_id { get; set; }

        public int semester_id { get; set; }

        [Required]
        [StringLength(10)]
        public string student_id { get; set; }

        public DateTime? planned_date { get; set; }

        public virtual Cours Cours { get; set; }

        public virtual Semester Semester { get; set; }

        public virtual Student Student { get; set; }
    }
}

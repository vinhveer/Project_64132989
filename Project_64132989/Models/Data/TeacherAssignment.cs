namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TeacherAssignment
    {
        [Key]
        public long assignment_id { get; set; }

        [Required]
        [StringLength(10)]
        public string teacher_id { get; set; }

        [Required]
        [StringLength(20)]
        public string course_id { get; set; }

        public virtual Cours Cours { get; set; }

        public virtual Teacher Teacher { get; set; }
    }
}

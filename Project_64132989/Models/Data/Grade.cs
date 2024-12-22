namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Grade
    {
        [Key]
        public long grade_id { get; set; }

        public long registration_id { get; set; }

        public byte assessment_type { get; set; }

        public decimal score { get; set; }

        public virtual StudentCourseRegistration StudentCourseRegistration { get; set; }
    }
}

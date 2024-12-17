namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StudentCourseRegistration
    {
        [Key]
        public long registration_id { get; set; }

        [Required]
        [StringLength(10)]
        public string student_user_id { get; set; }

        public int offering_id { get; set; }

        public DateTime? registration_date { get; set; }

        public byte? registration_status { get; set; }

        public byte registration_type { get; set; }

        public virtual CourseOffering CourseOffering { get; set; }

        public virtual Grade Grade { get; set; }

        public virtual Student Student { get; set; }
    }
}

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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long registration_id { get; set; }

        public decimal? midterm_score { get; set; }

        public decimal? final_score { get; set; }

        public decimal? total_score { get; set; }

        [StringLength(2)]
        public string grade_letter { get; set; }

        public decimal? grade_point { get; set; }

        public byte? status { get; set; }

        public decimal? partial_score { get; set; }

        public virtual StudentCourseRegistration StudentCourseRegistration { get; set; }
    }
}

namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class StudentLearningPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Mã kế hoạch học tập")]
        public long learning_plan_id { get; set; }

        [Required(ErrorMessage = "Mã môn học không được để trống")]
        [StringLength(20)]
        [DisplayName("Mã môn học")]
        public string course_id { get; set; }

        [Required(ErrorMessage = "Mã học kỳ không được để trống")]
        [DisplayName("Mã học kỳ")]
        public int semester_id { get; set; }

        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(10)]
        [DisplayName("Mã sinh viên")]
        public string student_id { get; set; }

        [DisplayName("Ngày lập kế hoạch")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? planned_date { get; set; } = DateTime.UtcNow;

        public virtual Cours Cours { get; set; }

        public virtual Semester Semester { get; set; }

        public virtual Student Student { get; set; }
    }
}
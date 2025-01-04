namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class TeacherAssignment
    {
        [Key]
        [DisplayName("Mã phân công")]
        public long assignment_id { get; set; }

        [Required(ErrorMessage = "Mã giảng viên không được để trống")]
        [StringLength(10)]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Mã giảng viên chỉ được chứa chữ cái và số")]
        [DisplayName("Mã giảng viên")]
        public string teacher_id { get; set; }

        [Required(ErrorMessage = "Mã môn học không được để trống")]
        [StringLength(20)]
        [DisplayName("Mã môn học")]
        public string course_id { get; set; }

        public virtual Cours Cours { get; set; }

        public virtual Teacher Teacher { get; set; }

        // Thêm các trường tự động
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [StringLength(10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string created_by { get; set; } = "trittntu";
    }
}
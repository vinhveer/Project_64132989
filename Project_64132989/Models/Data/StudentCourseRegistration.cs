namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class StudentCourseRegistration
    {
        [Key]
        [DisplayName("Mã đăng ký")]
        public long registration_id { get; set; }

        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(10)]
        [DisplayName("Mã sinh viên")]
        public string student_id { get; set; }

        [Required(ErrorMessage = "Mã lớp học phần không được để trống")]
        [DisplayName("Mã lớp học phần")]
        public long offering_id { get; set; }

        [DisplayName("Ngày đăng ký")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? registration_date { get; set; } = DateTime.UtcNow;

        [Range(0, 255, ErrorMessage = "Trạng thái phải từ 0 đến 255")]
        [DisplayName("Trạng thái")]
        public byte? status { get; set; }

        public virtual CourseOffering CourseOffering { get; set; }

        public virtual Student Student { get; set; }
    }
}
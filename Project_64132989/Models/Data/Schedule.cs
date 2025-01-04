namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class Schedule
    {
        [Key]
        [DisplayName("Mã thời khóa biểu")]
        public long schedule_id { get; set; }

        [Required(ErrorMessage = "Mã lớp học phần không được để trống")]
        [DisplayName("Mã lớp học phần")]
        public long offering_id { get; set; }

        [Required(ErrorMessage = "Thứ trong tuần không được để trống")]
        [Range(2, 8, ErrorMessage = "Thứ trong tuần phải từ 2 đến 8 (2: Thứ 2, ..., 8: Chủ nhật)")]
        [DisplayName("Thứ trong tuần")]
        public byte day_of_week { get; set; }

        [Required(ErrorMessage = "Ca học không được để trống")]
        [DisplayName("Ca học")]
        public byte slot_id { get; set; }

        [Required]
        [DisplayName("Ngày tạo")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(10)]
        [DisplayName("Người tạo")]
        public string created_by { get; set; } = "trittntu";

        [Required]
        [Range(0, 255, ErrorMessage = "Trạng thái phải từ 0 đến 255")]
        [DisplayName("Trạng thái")]
        public byte status { get; set; }

        public virtual CourseOffering CourseOffering { get; set; }

        public virtual TimeSlot TimeSlot { get; set; }
    }
}
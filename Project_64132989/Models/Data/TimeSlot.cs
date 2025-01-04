namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class TimeSlot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TimeSlot()
        {
            Schedules = new HashSet<Schedule>();
        }

        [Key]
        [Range(1, 10, ErrorMessage = "Ca học phải từ 1 đến 10")]
        [DisplayName("Mã ca học")]
        public byte slot_id { get; set; }

        [Required(ErrorMessage = "Thời gian bắt đầu không được để trống")]
        [DisplayName("Thời gian bắt đầu")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan start_time { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc không được để trống")]
        [DisplayName("Thời gian kết thúc")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan end_time { get; set; }

        [Required(ErrorMessage = "Buổi học không được để trống")]
        [StringLength(10)]
        [RegularExpression(@"^(MORNING|AFTERNOON|EVENING)$", ErrorMessage = "Buổi học phải là MORNING, AFTERNOON hoặc EVENING")]
        [DisplayName("Buổi học")]
        public string session { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Schedule> Schedules { get; set; }

        // Additional validation
        [NotMapped]
        public bool IsValid
        {
            get
            {
                return end_time > start_time && // End time must be after start time
                       start_time.Hours >= 7 && // Start time must be after 7:00
                       end_time.Hours <= 21;    // End time must be before 21:00
            }
        }

        // Custom validation for session based on time
        public bool IsValidSession()
        {
            switch (session)
            {
                case "MORNING":
                    return start_time.Hours >= 7 && end_time.Hours <= 12;
                case "AFTERNOON":
                    return start_time.Hours >= 13 && end_time.Hours <= 17;
                case "EVENING":
                    return start_time.Hours >= 18 && end_time.Hours <= 21;
                default:
                    return false;
            }
        }
    }
}
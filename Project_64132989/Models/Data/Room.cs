namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    public partial class Room
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Room()
        {
            CourseOfferings = new HashSet<CourseOffering>();
        }

        [Key]
        [DisplayName("Mã phòng")]
        public int room_id { get; set; }

        [Required(ErrorMessage = "Tên phòng không được để trống")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên phòng phải từ 2 đến 50 ký tự")]
        [RegularExpression(@"^[A-Za-z0-9\s\-\.]+$", ErrorMessage = "Tên phòng chỉ được chứa chữ cái, số, khoảng trắng, dấu gạch ngang và dấu chấm")]
        [DisplayName("Tên phòng")]
        public string room_name { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên tòa nhà phải từ 2 đến 50 ký tự")]
        [RegularExpression(@"^[A-Za-z0-9\s\-\.]+$", ErrorMessage = "Tên tòa nhà chỉ được chứa chữ cái, số, khoảng trắng, dấu gạch ngang và dấu chấm")]
        [DisplayName("Tòa nhà")]
        public string building { get; set; }

        [Range(1, 1000, ErrorMessage = "Sức chứa phải từ 1 đến 1000")]
        [DisplayName("Sức chứa")]
        public int? capacity { get; set; }

        [Range(0, 255, ErrorMessage = "Trạng thái phải từ 0 đến 255")]
        [DisplayName("Trạng thái")]
        public byte? status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseOffering> CourseOfferings { get; set; }
    }
}
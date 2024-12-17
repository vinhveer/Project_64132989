namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Room
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Room()
        {
            CourseOfferings = new HashSet<CourseOffering>();
        }

        [Key]
        public int room_id { get; set; }

        [Required]
        [StringLength(50)]
        public string room_name { get; set; }

        [StringLength(50)]
        public string building { get; set; }

        public int? capacity { get; set; }

        public byte? room_type { get; set; }

        public string equipment { get; set; }

        public byte? status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseOffering> CourseOfferings { get; set; }
    }
}

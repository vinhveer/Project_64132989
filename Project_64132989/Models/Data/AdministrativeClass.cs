namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AdministrativeClass
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AdministrativeClass()
        {
            Students = new HashSet<Student>();
        }

        [Key]
        [StringLength(10)]
        public string class_id { get; set; }

        [Required]
        [StringLength(50)]
        public string class_name { get; set; }

        [StringLength(100)]
        public string program { get; set; }

        public int? start_year { get; set; }

        [StringLength(10)]
        public string advisor_user_id { get; set; }

        public int total_students { get; set; }

        public virtual Teacher Teacher { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student> Students { get; set; }
    }
}

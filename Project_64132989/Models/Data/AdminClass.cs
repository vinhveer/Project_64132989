namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AdminClass
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AdminClass()
        {
            Students = new HashSet<Student>();
        }

        [Key]
        [StringLength(10)]
        public string class_id { get; set; }

        [Required]
        [StringLength(100)]
        public string class_name { get; set; }

        [Required]
        [StringLength(10)]
        public string department_id { get; set; }

        [StringLength(10)]
        public string advisor_teacher_id { get; set; }

        public DateTime? created_date { get; set; }

        public byte? status { get; set; }

        public virtual Department Department { get; set; }

        public virtual Teacher Teacher { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student> Students { get; set; }
    }
}

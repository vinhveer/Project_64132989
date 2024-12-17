namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Student
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Student()
        {
            StudentCourseRegistrations = new HashSet<StudentCourseRegistration>();
        }

        [Key]
        [StringLength(10)]
        public string user_id { get; set; }

        [Required]
        [StringLength(10)]
        public string administrative_class_id { get; set; }

        public int entry_year { get; set; }

        public int? graduation_expected_year { get; set; }

        public byte? academic_status { get; set; }

        public virtual AdministrativeClass AdministrativeClass { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentCourseRegistration> StudentCourseRegistrations { get; set; }

        public virtual User User { get; set; }
    }
}

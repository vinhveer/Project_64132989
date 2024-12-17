namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Teacher
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Teacher()
        {
            AdministrativeClasses = new HashSet<AdministrativeClass>();
            CourseOfferings = new HashSet<CourseOffering>();
        }

        [Key]
        [StringLength(10)]
        public string user_id { get; set; }

        [StringLength(100)]
        public string department { get; set; }

        [StringLength(50)]
        public string academic_rank { get; set; }

        public string research_areas { get; set; }

        public int? teaching_experience { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdministrativeClass> AdministrativeClasses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseOffering> CourseOfferings { get; set; }

        public virtual User User { get; set; }
    }
}

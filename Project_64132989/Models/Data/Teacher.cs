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
            AdminClasses = new HashSet<AdminClass>();
            CourseOfferings = new HashSet<CourseOffering>();
            TeacherAssignments = new HashSet<TeacherAssignment>();
        }

        [Key]
        [StringLength(10)]
        public string user_id { get; set; }

        [Required]
        [StringLength(10)]
        public string department_id { get; set; }

        [StringLength(50)]
        public string academic_rank { get; set; }

        public string research_areas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdminClass> AdminClasses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseOffering> CourseOfferings { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; }

        public virtual User User { get; set; }
    }
}

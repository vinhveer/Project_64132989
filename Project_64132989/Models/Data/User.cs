namespace Project_64132989.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [Key]
        [StringLength(10)]
        public string user_id { get; set; }

        [Required]
        [StringLength(100)]
        public string email { get; set; }

        [Required]
        [StringLength(255)]
        public string password_hash { get; set; }

        public byte user_type { get; set; }

        public byte? account_status { get; set; }

        public DateTime? last_login { get; set; }

        public DateTime? created_at { get; set; }

        [StringLength(255)]
        public string ResetPasswordToken { get; set; }

        public DateTime? ResetPasswordExpires { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual Student Student { get; set; }

        public virtual Teacher Teacher { get; set; }
    }
}

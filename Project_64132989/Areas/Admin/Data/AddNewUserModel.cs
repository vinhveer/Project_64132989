using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_64132989.Areas.Admin.Data
{
    public class AddNewUserModel
    {
        [StringLength(10)]
        public string user_id { get; set; }

        [Required]
        [StringLength(50)]
        public string first_name { get; set; }

        [Required]
        [StringLength(50)]
        public string last_name { get; set; }

        [Column(TypeName = "date")]
        public DateTime? date_of_birth { get; set; }

        public byte? gender { get; set; }

        [StringLength(20)]
        public string phone_number { get; set; }

        public string address { get; set; }

        [StringLength(255)]
        public string avatar_path { get; set; }

        [Required]
        [StringLength(100)]
        public string email { get; set; }

        [Required]
        [StringLength(255)]
        public string password { get; set; }
    }
}
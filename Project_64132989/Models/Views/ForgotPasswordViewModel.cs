using System.ComponentModel.DataAnnotations;

namespace Project_64132989.Models.Views
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email (Thư điện tử)")]
        public string Email { get; set; }
    }
}
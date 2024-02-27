using System.ComponentModel.DataAnnotations;

namespace CasaDespaDraft.ViewModels
{
    public class AdminResetPasswordViewModel
    {
        public int userId { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        public string? email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string? userPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(userPassword), ErrorMessage = "Password do not match")]
        [Required(ErrorMessage = "You must confirm your password")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Firstname")]
        [Required(ErrorMessage = "Firstname is required")]
        public string? firstName { get; set; }

        [Display(Name = "Lastname")]
        [Required(ErrorMessage = "Lastname is required")]
        public string? lastName { get; set; }


    }
}
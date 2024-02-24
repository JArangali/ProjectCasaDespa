using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CasaDespaDraft.ViewModels
{

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        /*[PasswordComplexity]*/
        [Display(Name = "New password")]
        public string? NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Password do not match")]
        [Required(ErrorMessage = "You must confirm your password")]
        public string? ConfirmNewPassword { get; set; }
        public string Email { get; set; }

    }
}
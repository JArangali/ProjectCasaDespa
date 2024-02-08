using System.ComponentModel.DataAnnotations;

namespace CasaDespaDraft.ViewModels
{
    public class SignInViewModel
    {

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        public string? UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace CasaDespaDraft.ViewModels
{
    public class AdminCodeViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Admin Code is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Answer")]
        public string adminCode { get; set; }

    }
}
using System.ComponentModel.DataAnnotations;

namespace CasaDespaDraft.ViewModels
{
    public class SecurityQuestionViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Answer is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Answer")]
        public string answer { get; set; }

        public string question { get; set; }

        public string fanswer { get; set; }

    }
}

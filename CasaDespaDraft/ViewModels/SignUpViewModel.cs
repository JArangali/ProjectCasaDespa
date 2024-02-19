using System.ComponentModel.DataAnnotations;

namespace CasaDespaDraft.ViewModels
{
    public class SignUpViewModel
    {
        public int userId { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        public string? email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string? userPassword { get; set; }

        [Display(Name = "Firstname")]
        [Required(ErrorMessage = "Firstname is required")]
        public string? firstName { get; set; }

        [Display(Name = "Lastname")]
        [Required(ErrorMessage = "Lastname is required")]
        public string? lastName { get; set; }
       

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address is required")]
        public string? address { get; set; }

        
        public string? sex { get; set; }

        [Display(Name = "Security Question")]
        [Required(ErrorMessage = "Security Question is required")]
        public string? question { get; set; }

        [Display(Name = "Answer")]
        [Required(ErrorMessage = "Security Question answer is required")]
        public string? answer { get; set; }

        [DataType(DataType.Upload)]
        [RegularExpression(@"^.*\.(jpg|jpeg|png)$", ErrorMessage = "Only JPG, JPEG, and PNG files are allowed.")]
        public byte[]? profilePicture { get; set; }


    }
}

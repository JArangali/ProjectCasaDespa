namespace CasaDespaDraft.ViewModels
{
    public class SecurityQuestion
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Answer is required.")]
        [Display(Name = "Answer")]
        public string answer { get; set; }

        public string question { get; set; }
    }
}

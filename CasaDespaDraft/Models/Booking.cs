using CasaDespaDraft.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaDespaDraft.Models
{
    public class CustomUrlAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Uri uri;
            if (Uri.TryCreate(value as string, UriKind.Absolute, out uri))
            {
                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeFtp)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Please include 'https://' or 'http://' or 'ftp://' at the beginning of your link.");
        }
    }

    public enum Package
    {
        Day_Tour, Night_Tour, Twenty_Two_Hours  
    }

    public enum ProfileStatus
    {
        Requests, Pending_Payment, Approved, Archive
    }

    public class Booking
    {
        [Key]
        public int bookingId { get; set; }
        [ForeignKey("userId")]
        public string? userId { get; set; }
        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Full Name is required.")]
        public string? fullName { get; set; }

        [Display(Name = "Contact Number")]
        [Required(ErrorMessage = "Contact Number is required.")]
        [RegularExpression("[0-9]{11}", ErrorMessage = "Please follow Phone Number format 09**-***-****!")]
        public string? contactNumber { get; set; }

        [CustomUrl]
        [Required(ErrorMessage = "Messenger Link is needed.")]
        public string? messengerLink { get; set; }

        [Required(ErrorMessage = "Please choose your preferred package.")]
        public Package package { get; set; }
        [Required(ErrorMessage = "Please enter the number of people attending.")]
        public int? pax { get; set; }

        [Required]
        public string? date { get; set; }

        [DataType(DataType.Upload)]
        [RegularExpression(@"^.*\.(jpg|jpeg|png)$", ErrorMessage = "Only JPG, JPEG, and PNG files are allowed.")]
        [FileExtensions(Extensions = "jpg,jpeg,png", ErrorMessage = "Only JPG, JPEG, and PNG files are allowed.")]
        public byte[]? image { get; set; }

        public string? Amount { get; set; }

        public byte[]? QRCode { get; set; }

        public string? Refnum { get; set; }

        public string? BStatus { get; set; }

        public ProfileStatus Status { get; set; } = ProfileStatus.Requests;

        [Required(ErrorMessage = "Please pick an accomodation.")]
        public string? accomodation { get; set; }

        public string? Remarks { get; set; }
    }
}

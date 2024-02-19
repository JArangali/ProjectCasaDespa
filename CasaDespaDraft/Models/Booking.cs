using CasaDespaDraft.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaDespaDraft.Models
{
    public enum Package
    {
        Day_Tour, Night_Tour, Twenty_Two_Hours
    }

    public enum BookingStatus
    {
        Done, Cancelled, Denied
    }

    public enum ProfileStatus
    {
        Requests, Pending_Payment, Archive
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

        [Url]
        [Required(ErrorMessage = "Messenger Link is needed.")]
        public string? messengerLink { get; set; }

        [Required(ErrorMessage = "Please choose your preferred package.")]
        public Package package { get; set; }
        [Required(ErrorMessage = "Please enter the number of people attending.")]
        public int? pax { get; set; }

        [Required]
        public string? date { get; set; }

        public ProfileStatus Status { get; set; } = ProfileStatus.Requests;

    }
}

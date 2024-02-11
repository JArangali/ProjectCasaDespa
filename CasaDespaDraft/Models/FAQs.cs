using CasaDespaDraft.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaDespaDraft.Models
{
    public class FAQs
    {
        [Key]
        public int faqsId { get; set; }
        
        [Required(ErrorMessage = "Please input the question.")]
        public string? FAQuestion { get; set; }

        [Required(ErrorMessage = "Please input the answer to the question.")]
        public string? FAQAnswer { get; set; }

    }

}
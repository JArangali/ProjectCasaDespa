using CasaDespaDraft.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaDespaDraft.Models
{
    public class TTInDates
    {
        [Key]
        public int ttInId { get; set; }
        public string? ttInDate { get; set; }

    }
}

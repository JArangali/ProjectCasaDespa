using CasaDespaDraft.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaDespaDraft.Models
{
    public class DayDates
    {
        [Key]
        public int dayId { get; set; }
        public string? dayDate { get; set; }
    }
}

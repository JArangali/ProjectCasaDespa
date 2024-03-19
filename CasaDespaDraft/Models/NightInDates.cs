using CasaDespaDraft.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaDespaDraft.Models
{
    public class NightInDates
    {
        [Key]
        public int nightInId { get; set; }
        public string? nightInDate { get; set; }

    }
}

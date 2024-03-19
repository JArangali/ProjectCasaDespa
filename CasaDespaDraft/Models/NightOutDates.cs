using CasaDespaDraft.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaDespaDraft.Models
{
    public class NightOutDates
    {
        [Key]
        public int nightOutId { get; set; }
        public string? nightOutDate { get; set; }

    }
}

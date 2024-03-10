using CasaDespaDraft.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaDespaDraft.Models
{
    public class TTOutDates
    {
        [Key]
        public int ttOutId { get; set; }
        public string? ttOutDate { get; set; }

    }
}

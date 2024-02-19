using CasaDespaDraft.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaDespaDraft.Models
{
    public class Homepage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? wd22hrs { get; set; }

        [Required]
        public string? wdMornhrs { get; set; }

        [Required]
        public string? wdNythrs { get; set; }

        [Required]
        public string? we22hrs { get; set; }

        [Required]
        public string? weMornhrs { get; set; }

        [Required]
        public string? weNythrs { get; set; }


        [Required]
        public string? af1 { get; set; }

        [Required]
        public string? af2 { get; set; }

        [Required]
        public string? af3 { get; set; }

        [Required]
        public string? af4 { get; set; }

        [Required]
        public string? af5 { get; set; }


        [Required]
        public string? c1 { get; set; }

        [Required]
        public string? c2 { get; set; }

        [Required]
        public string? sd1 { get; set; }

        [Required]
        public string? oc1 { get; set; }

        [Required]
        public string? cIN { get; set; }

        [Required]
        public string? cOUT { get; set; }

        [Required]
        public string? payment { get; set; }

        [Required]
        public string? mc1 { get; set; }

        [Required]
        public string? mc2 { get; set; }

        [Required]
        public string? clean { get; set; }

        [Required]
        public string? sc1 { get; set; }

        [Required]
        public string? noise { get; set; }

        [Required]
        public string? penalty { get; set; }

        [Required]
        public string? warning { get; set; }
    }
}

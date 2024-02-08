using CasaDespaDraft.Models;
using Microsoft.AspNetCore.Identity;

namespace CasaDespaDraft.Data
{
    public class User : IdentityUser
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Address { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
    }
}

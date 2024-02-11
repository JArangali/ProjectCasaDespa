﻿using CasaDespaDraft.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaDespaDraft.Models
{
    public enum Filter
    {
        Pool, Amenities, Entertainment
    }

    public class Gallery
    {
        [Key]
        public int imageId { get; set; }

        [DataType(DataType.Upload)]
        [RegularExpression(@"^.*\.(jpg|jpeg|png)$", ErrorMessage = "Only JPG, JPEG, and PNG files are allowed.")]
        public byte[]? image { get; set; }

        [Required]
        public String imageName{ get; set; }

        [Required]
        public Filter filter { get; set; } = Filter.Pool;

        public string? filterChoice { get; set; }
    }
}
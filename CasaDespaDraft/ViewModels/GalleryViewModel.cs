using CasaDespaDraft.Models;
using System.ComponentModel.DataAnnotations;

namespace CasaDespaDraft.ViewModels
{
    public class GalleryViewModel
    {
        public IEnumerable<Gallery> Galleries { get; set; }
        public Gallery Gallery { get; set; }

        [Key]
        public int imageId { get; set; }

        [DataType(DataType.Upload)]
        [RegularExpression(@"^.*\.(jpg|jpeg|png)$", ErrorMessage = "Only JPG, JPEG, and PNG files are allowed.")]
        public byte[]? image { get; set; }

        [Required]
        public String imageName { get; set; }

        [Required]
        public Filter filter { get; set; }
        public string filterChoice { get; set; }
    }
}

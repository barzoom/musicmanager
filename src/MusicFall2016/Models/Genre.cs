using System.ComponentModel.DataAnnotations;

namespace MusicFall2016.Models
{
    public class Genre
    {
        public int GenreID { get; set; }
        [Required(ErrorMessage = "Please enter a name for the Genre.")]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
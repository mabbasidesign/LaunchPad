using System.ComponentModel.DataAnnotations;

namespace LaunchPad.DTO
{
    // Data Transfer Object for Book
    public class BookDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Author must be between 1 and 200 characters.")]
        public string Author { get; set; } = string.Empty;

        [Range(1000, 2100, ErrorMessage = "Year must be between 1000 and 2100.")]
        public int Year { get; set; }
    }
}
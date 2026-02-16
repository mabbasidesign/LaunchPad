using System.ComponentModel.DataAnnotations;

namespace LaunchPad.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Author must be between 1 and 200 characters.")]
        public string Author { get; set; } = string.Empty;

        [StringLength(20, MinimumLength = 10, ErrorMessage = "ISBN must be between 10 and 20 characters.")]
        [RegularExpression(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[X0-9]$", ErrorMessage = "ISBN format is invalid.")]
        public string ISBN { get; set; } = string.Empty;

        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000.")]
        public decimal Price { get; set; }

        [Range(0, 100000, ErrorMessage = "Stock must be between 0 and 100000.")]
        public int Stock { get; set; }

        [Range(1000, 2100, ErrorMessage = "Year must be between 1000 and 2100.")]
        public int Year { get; set; }
    }
}
using MediatR;
using LaunchPad.Models;

namespace LaunchPad.Features.Books.Commands
{
    /// <summary>
    /// Command to create a new book.
    /// </summary>
    public class CreateBookCommand : IRequest<Book>
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int Year { get; set; }
    }
}

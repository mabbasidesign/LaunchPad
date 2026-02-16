using MediatR;
using LaunchPad.Models;

namespace LaunchPad.Features.Books.Commands
{
    /// <summary>
    /// Command to update an existing book.
    /// </summary>
    public class UpdateBookCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int Year { get; set; }
    }
}

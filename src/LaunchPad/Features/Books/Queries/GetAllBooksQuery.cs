using MediatR;
using LaunchPad.DTO;

namespace LaunchPad.Features.Books.Queries
{
    /// <summary>
    /// Query to retrieve all books.
    /// </summary>
    public class GetAllBooksQuery : IRequest<List<BookDto>>
    {
    }
}

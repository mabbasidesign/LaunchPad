using MediatR;
using LaunchPad.DTO;

namespace LaunchPad.Features.Books.Queries
{
    /// <summary>
    /// Query to retrieve a book by ID.
    /// </summary>
    public class GetBookByIdQuery : IRequest<BookDto?>
    {
        public int Id { get; set; }

        public GetBookByIdQuery(int id)
        {
            Id = id;
        }
    }
}

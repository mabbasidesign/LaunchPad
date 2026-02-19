using MediatR;
using LaunchPad.DTO;

namespace LaunchPad.Features.Books.Queries
{
    /// <summary>
    /// Query to retrieve all books with pagination support.
    /// </summary>
    public class GetAllBooksQuery : IRequest<PaginatedResultDto<BookDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public GetAllBooksQuery() { }

        public GetAllBooksQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1;
            PageSize = pageSize > 0 ? pageSize : 10;
        }
    }
}

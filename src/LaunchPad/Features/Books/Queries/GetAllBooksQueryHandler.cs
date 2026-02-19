using MediatR;
using LaunchPad.DTO;
using LaunchPad.Services;

namespace LaunchPad.Features.Books.Queries
{
    /// <summary>
    /// Handler for GetAllBooksQuery. Retrieves all books from the repository with pagination support.
    /// </summary>
    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, PaginatedResultDto<BookDto>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<GetAllBooksQueryHandler> _logger;

        public GetAllBooksQueryHandler(IBookRepository bookRepository, ILogger<GetAllBooksQueryHandler> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PaginatedResultDto<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var (books, totalCount) = await _bookRepository.GetAllPaginatedAsync(request.PageNumber, request.PageSize);
                
                var bookDtos = books.Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Year = b.Year
                }).ToList();

                stopwatch.Stop();
                _logger.LogInformation("[QUERY: GetAllBooks] Retrieved {BookCount} books (Page {PageNumber}, Size {PageSize}, Total {TotalCount}) in {Duration}ms", 
                    bookDtos.Count, request.PageNumber, request.PageSize, totalCount, stopwatch.ElapsedMilliseconds);

                return new PaginatedResultDto<BookDto>
                {
                    Items = bookDtos,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[QUERY: GetAllBooks] Error retrieving books after {Duration}ms", 
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}

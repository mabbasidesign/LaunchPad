using MediatR;
using LaunchPad.DTO;
using LaunchPad.Services;

namespace LaunchPad.Features.Books.Queries
{
    /// <summary>
    /// Handler for GetAllBooksQuery. Retrieves all books from the repository.
    /// </summary>
    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, List<BookDto>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<GetAllBooksQueryHandler> _logger;

        public GetAllBooksQueryHandler(IBookRepository bookRepository, ILogger<GetAllBooksQueryHandler> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var books = await _bookRepository.GetAll();
                var bookDtos = books.Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Year = b.Year
                }).ToList();

                stopwatch.Stop();
                _logger.LogInformation("[QUERY: GetAllBooks] Retrieved {BookCount} books in {Duration}ms", 
                    bookDtos.Count, stopwatch.ElapsedMilliseconds);

                return bookDtos;
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

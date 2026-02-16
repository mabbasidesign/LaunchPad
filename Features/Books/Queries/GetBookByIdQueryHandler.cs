using MediatR;
using LaunchPad.DTO;
using LaunchPad.Services;

namespace LaunchPad.Features.Books.Queries
{
    /// <summary>
    /// Handler for GetBookByIdQuery. Retrieves a specific book by ID.
    /// </summary>
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<GetBookByIdQueryHandler> _logger;

        public GetBookByIdQueryHandler(IBookRepository bookRepository, ILogger<GetBookByIdQueryHandler> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                if (request.Id <= 0)
                {
                    _logger.LogWarning("[QUERY: GetBookById] Invalid ID: {Id}", request.Id);
                    return null;
                }

                var book = await _bookRepository.GetById(request.Id);
                
                stopwatch.Stop();
                
                if (book == null)
                {
                    _logger.LogInformation("[QUERY: GetBookById] Book not found for ID: {Id} after {Duration}ms", 
                        request.Id, stopwatch.ElapsedMilliseconds);
                    return null;
                }

                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Year = book.Year
                };

                _logger.LogInformation("[QUERY: GetBookById] Retrieved book {BookId}: {Title} in {Duration}ms", 
                    book.Id, book.Title, stopwatch.ElapsedMilliseconds);

                return bookDto;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[QUERY: GetBookById] Error retrieving book ID {Id} after {Duration}ms", 
                    request.Id, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}

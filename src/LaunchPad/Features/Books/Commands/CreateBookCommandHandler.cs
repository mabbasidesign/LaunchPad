using MediatR;
using LaunchPad.Models;
using LaunchPad.Services;

namespace LaunchPad.Features.Books.Commands
{
    /// <summary>
    /// Handler for CreateBookCommand. Creates a new book and persists to the repository.
    /// </summary>
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Book>
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<CreateBookCommandHandler> _logger;

        public CreateBookCommandHandler(IBookRepository bookRepository, ILogger<CreateBookCommandHandler> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Book> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var book = new Book
                {
                    Title = request.Title,
                    Author = request.Author,
                    ISBN = request.ISBN,
                    Price = request.Price,
                    Stock = request.Stock,
                    Year = request.Year
                };

                await _bookRepository.Add(book);

                stopwatch.Stop();
                _logger.LogInformation("[COMMAND: CreateBook] Book created successfully. ID: {BookId}, Title: {Title}, Duration: {Duration}ms",
                    book.Id, book.Title, stopwatch.ElapsedMilliseconds);

                return book;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[COMMAND: CreateBook] Error creating book for title {Title} after {Duration}ms",
                    request.Title, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}

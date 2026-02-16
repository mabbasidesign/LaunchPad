using MediatR;
using LaunchPad.Services;

namespace LaunchPad.Features.Books.Commands
{
    /// <summary>
    /// Handler for UpdateBookCommand. Updates an existing book in the repository.
    /// </summary>
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, Unit>
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<UpdateBookCommandHandler> _logger;

        public UpdateBookCommandHandler(IBookRepository bookRepository, ILogger<UpdateBookCommandHandler> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                if (request.Id <= 0)
                {
                    _logger.LogWarning("[COMMAND: UpdateBook] Invalid Book ID: {BookId}", request.Id);
                    throw new ArgumentException("Book ID must be greater than 0", nameof(request.Id));
                }

                var existingBook = await _bookRepository.GetById(request.Id);
                if (existingBook == null)
                {
                    _logger.LogWarning("[COMMAND: UpdateBook] Book not found for ID: {BookId}", request.Id);
                    throw new KeyNotFoundException($"Book with ID {request.Id} not found");
                }

                existingBook.Title = request.Title;
                existingBook.Author = request.Author;
                existingBook.ISBN = request.ISBN;
                existingBook.Price = request.Price;
                existingBook.Stock = request.Stock;
                existingBook.Year = request.Year;

                await _bookRepository.Update(existingBook);

                stopwatch.Stop();
                _logger.LogInformation("[COMMAND: UpdateBook] Book updated successfully. ID: {BookId}, Title: {Title}, Duration: {Duration}ms",
                    existingBook.Id, existingBook.Title, stopwatch.ElapsedMilliseconds);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[COMMAND: UpdateBook] Error updating book ID {BookId} after {Duration}ms",
                    request.Id, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}

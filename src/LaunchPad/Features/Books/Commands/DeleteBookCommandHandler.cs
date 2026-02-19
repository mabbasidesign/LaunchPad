using MediatR;
using LaunchPad.Services;

namespace LaunchPad.Features.Books.Commands
{
    /// <summary>
    /// Handler for DeleteBookCommand. Deletes a book from the repository.
    /// </summary>
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Unit>
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<DeleteBookCommandHandler> _logger;

        public DeleteBookCommandHandler(IBookRepository bookRepository, ILogger<DeleteBookCommandHandler> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                if (request.Id <= 0)
                {
                    _logger.LogWarning("[COMMAND: DeleteBook] Invalid Book ID: {BookId}", request.Id);
                    throw new ArgumentException("Book ID must be greater than 0", nameof(request.Id));
                }

                // Check existence with efficient query (no full entity load)
                var exists = await _bookRepository.ExistsAsync(request.Id);
                if (!exists)
                {
                    _logger.LogWarning("[COMMAND: DeleteBook] Book not found for ID: {BookId}", request.Id);
                    throw new KeyNotFoundException($"Book with ID {request.Id} not found");
                }

                await _bookRepository.Delete(request.Id);

                stopwatch.Stop();
                _logger.LogInformation("[COMMAND: DeleteBook] Book deleted successfully. ID: {BookId}, Duration: {Duration}ms",
                    request.Id, stopwatch.ElapsedMilliseconds);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[COMMAND: DeleteBook] Error deleting book ID {BookId} after {Duration}ms",
                    request.Id, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}

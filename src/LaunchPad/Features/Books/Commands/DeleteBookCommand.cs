using MediatR;

namespace LaunchPad.Features.Books.Commands
{
    /// <summary>
    /// Command to delete a book by ID.
    /// </summary>
    public class DeleteBookCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public DeleteBookCommand(int id)
        {
            Id = id;
        }
    }
}

using LaunchPad.DTO;
using LaunchPad.Data;
using LaunchPad.Models;
using System.Linq;
using LaunchPad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using LaunchPad.Features.Books.Queries;
using LaunchPad.Features.Books.Commands;

namespace LaunchPad.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IMediator mediator, ILogger<BooksController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResultDto<BookDto>>> GetBooks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var username = User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation("[GET_BOOKS] Request started. User: {Username}, PageNumber: {PageNumber}, PageSize: {PageSize}", 
                username, pageNumber, pageSize);

            try
            {
                var query = new GetAllBooksQuery(pageNumber, pageSize);
                var result = await _mediator.Send(query);

                _logger.LogInformation("[GET_BOOKS] Retrieved {BookCount} books (Page {PageNumber}, Total {TotalCount}). User: {Username}", 
                    result.Items.Count, result.PageNumber, result.TotalCount, username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GET_BOOKS] Failed to retrieve books. User: {Username}. Exception: {ExceptionMessage}", 
                    username, ex.Message);
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var username = User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation("[GET_BOOK] Request started. BookId: {BookId}. User: {Username}", id, username);

            try
            {
                var query = new GetBookByIdQuery(id);
                var bookDto = await _mediator.Send(query);
                
                if (bookDto == null)
                {
                    _logger.LogWarning("[GET_BOOK] Book not found. BookId: {BookId}. User: {Username}", id, username);
                    return NotFound();
                }

                _logger.LogInformation("[GET_BOOK] Retrieved book {Title}. BookId: {BookId}. User: {Username}", 
                    bookDto.Title, id, username);
                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GET_BOOK] Failed to retrieve book {BookId}. User: {Username}. Exception: {ExceptionMessage}", 
                    id, username, ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook([FromBody] Book book)
        {
            var username = User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation("[CREATE_BOOK] Request started. Title: {Title}. Author: {Author}. User: {Username}", 
                book.Title, book.Author, username);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[CREATE_BOOK] Validation failed. Errors: {Errors}. User: {Username}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))),
                    username);
                return BadRequest(ModelState);
            }

            try
            {
                var command = new CreateBookCommand
                {
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    Price = book.Price,
                    Stock = book.Stock,
                    Year = book.Year
                };

                var createdBook = await _mediator.Send(command);

                _logger.LogInformation("[CREATE_BOOK] Book created successfully. BookId: {BookId}. Title: {Title}. User: {Username}", 
                    createdBook.Id, createdBook.Title, username);

                var bookDto = new BookDto
                {
                    Id = createdBook.Id,
                    Title = createdBook.Title,
                    Author = createdBook.Author,
                    Year = createdBook.Year
                };
                return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, bookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CREATE_BOOK] Failed to create book. Title: {Title}. User: {Username}. Exception: {ExceptionMessage}", 
                    book.Title, username, ex.Message);
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            var username = User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation("[UPDATE_BOOK] Request started. BookId: {BookId}. Title: {Title}. User: {Username}", 
                id, book.Title, username);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[UPDATE_BOOK] Validation failed for BookId {BookId}. Errors: {Errors}. User: {Username}", 
                    id,
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))),
                    username);
                return BadRequest(ModelState);
            }

            if (id != book.Id)
            {
                _logger.LogWarning("[UPDATE_BOOK] ID mismatch. RouteId: {RouteId}. BookId: {BookId}. User: {Username}", 
                    id, book.Id, username);
                return BadRequest("Book ID mismatch.");
            }

            try
            {
                var command = new UpdateBookCommand
                {
                    Id = id,
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    Price = book.Price,
                    Stock = book.Stock,
                    Year = book.Year
                };

                await _mediator.Send(command);

                _logger.LogInformation("[UPDATE_BOOK] Book updated successfully. BookId: {BookId}. Title: {Title}. User: {Username}", 
                    id, book.Title, username);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("[UPDATE_BOOK] Book not found. BookId: {BookId}. User: {Username}", id, username);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UPDATE_BOOK] Failed to update book {BookId}. User: {Username}. Exception: {ExceptionMessage}", 
                    id, username, ex.Message);
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var username = User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation("[DELETE_BOOK] Request started. BookId: {BookId}. User: {Username}", id, username);

            try
            {
                var command = new DeleteBookCommand(id);
                await _mediator.Send(command);

                _logger.LogInformation("[DELETE_BOOK] Book deleted successfully. BookId: {BookId}. User: {Username}", 
                    id, username);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("[DELETE_BOOK] Book not found. BookId: {BookId}. User: {Username}", id, username);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DELETE_BOOK] Failed to delete book {BookId}. User: {Username}. Exception: {ExceptionMessage}", 
                    id, username, ex.Message);
                throw;
            }
        }
    }
}

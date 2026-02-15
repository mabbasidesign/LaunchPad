using LaunchPad.DTO;
using LaunchPad.Data;
using LaunchPad.Models;
using System.Linq;
using LaunchPad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaunchPad.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookRepository bookRepository, ILogger<BooksController> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var username = User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation("[GET_BOOKS] Request started. User: {Username}", username);

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
                _logger.LogInformation("[GET_BOOKS] Retrieved {BookCount} books. Duration: {Duration}ms. User: {Username}", 
                    bookDtos.Count, stopwatch.ElapsedMilliseconds, username);
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[GET_BOOKS] Failed to retrieve books. Duration: {Duration}ms. User: {Username}. Exception: {ExceptionMessage}", 
                    stopwatch.ElapsedMilliseconds, username, ex.Message);
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var username = User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation("[GET_BOOK] Request started. BookId: {BookId}. User: {Username}", id, username);

            try
            {
                var book = await _bookRepository.GetById(id);
                if (book == null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning("[GET_BOOK] Book not found. BookId: {BookId}. Duration: {Duration}ms. User: {Username}", 
                        id, stopwatch.ElapsedMilliseconds, username);
                    return NotFound();
                }

                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Year = book.Year
                };

                stopwatch.Stop();
                _logger.LogInformation("[GET_BOOK] Retrieved book {Title}. BookId: {BookId}. Duration: {Duration}ms. User: {Username}", 
                    book.Title, id, stopwatch.ElapsedMilliseconds, username);
                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[GET_BOOK] Failed to retrieve book {BookId}. Duration: {Duration}ms. User: {Username}. Exception: {ExceptionMessage}", 
                    id, stopwatch.ElapsedMilliseconds, username, ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var username = User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation("[CREATE_BOOK] Request started. Title: {Title}. Author: {Author}. User: {Username}", 
                book.Title, book.Author, username);

            if (!ModelState.IsValid)
            {
                stopwatch.Stop();
                _logger.LogWarning("[CREATE_BOOK] Validation failed. Duration: {Duration}ms. Errors: {Errors}. User: {Username}", 
                    stopwatch.ElapsedMilliseconds,
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))),
                    username);
                return BadRequest(ModelState);
            }

            try
            {
                var newBook = new Book
                {
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    Price = book.Price,
                    Stock = book.Stock,
                    Year = book.Year
                };
                await _bookRepository.Add(newBook);

                stopwatch.Stop();
                _logger.LogInformation("[CREATE_BOOK] Book created successfully. BookId: {BookId}. Title: {Title}. Duration: {Duration}ms. User: {Username}", 
                    newBook.Id, newBook.Title, stopwatch.ElapsedMilliseconds, username);

                var bookDto = new BookDto
                {
                    Id = newBook.Id,
                    Title = newBook.Title,
                    Author = newBook.Author,
                    Year = newBook.Year
                };
                return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, bookDto);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[CREATE_BOOK] Failed to create book. Title: {Title}. Duration: {Duration}ms. User: {Username}. Exception: {ExceptionMessage}", 
                    book.Title, stopwatch.ElapsedMilliseconds, username, ex.Message);
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var username = User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation("[UPDATE_BOOK] Request started. BookId: {BookId}. Title: {Title}. User: {Username}", 
                id, book.Title, username);

            if (!ModelState.IsValid)
            {
                stopwatch.Stop();
                _logger.LogWarning("[UPDATE_BOOK] Validation failed for BookId {BookId}. Duration: {Duration}ms. Errors: {Errors}. User: {Username}", 
                    id, stopwatch.ElapsedMilliseconds,
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))),
                    username);
                return BadRequest(ModelState);
            }

            if (id != book.Id)
            {
                stopwatch.Stop();
                _logger.LogWarning("[UPDATE_BOOK] ID mismatch. RouteId: {RouteId}. BookId: {BookId}. Duration: {Duration}ms. User: {Username}", 
                    id, book.Id, stopwatch.ElapsedMilliseconds, username);
                return BadRequest("Book ID mismatch.");
            }

            try
            {
                await _bookRepository.Update(book);
                stopwatch.Stop();
                _logger.LogInformation("[UPDATE_BOOK] Book updated successfully. BookId: {BookId}. Title: {Title}. Duration: {Duration}ms. User: {Username}", 
                    id, book.Title, stopwatch.ElapsedMilliseconds, username);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[UPDATE_BOOK] Failed to update book {BookId}. Duration: {Duration}ms. User: {Username}. Exception: {ExceptionMessage}", 
                    id, stopwatch.ElapsedMilliseconds, username, ex.Message);
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var username = User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation("[DELETE_BOOK] Request started. BookId: {BookId}. User: {Username}", id, username);

            try
            {
                var book = await _bookRepository.GetById(id);
                if (book == null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning("[DELETE_BOOK] Book not found. BookId: {BookId}. Duration: {Duration}ms. User: {Username}", 
                        id, stopwatch.ElapsedMilliseconds, username);
                    return NotFound();
                }

                await _bookRepository.Delete(id);
                stopwatch.Stop();
                _logger.LogInformation("[DELETE_BOOK] Book deleted successfully. BookId: {BookId}. Title: {Title}. Duration: {Duration}ms. User: {Username}", 
                    id, book.Title, stopwatch.ElapsedMilliseconds, username);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[DELETE_BOOK] Failed to delete book {BookId}. Duration: {Duration}ms. User: {Username}. Exception: {ExceptionMessage}", 
                    id, stopwatch.ElapsedMilliseconds, username, ex.Message);
                throw;
            }
        }
    }
}

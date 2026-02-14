using LaunchPad.DTO;
using LaunchPad.Data;
using LaunchPad.Models;
using System.Linq;
using LaunchPad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaunchPad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            _logger.LogInformation("Getting all books");
            var books = await _bookRepository.GetAll();
            var bookDtos = books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Year = b.Year
            });
            return Ok(bookDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            _logger.LogInformation("Getting book with id {BookId}", id);
            var book = await _bookRepository.GetById(id);
            if (book == null)
            {
                _logger.LogWarning("Book with id {BookId} not found", id);
                return NotFound();
            }
            var bookDto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Year = book.Year
            };
            return Ok(bookDto);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            _logger.LogInformation("Creating a new book");
            var newBook = new Book
            {
                Title = book.Title,
                Author = book.Author,
                Year = book.Year
            };
            await _bookRepository.Add(newBook);
            _logger.LogInformation("Book created with id {BookId}", newBook.Id);
            var bookDto = new BookDto
            {
                Id = newBook.Id,
                Title = newBook.Title,
                Author = newBook.Author,
                Year = newBook.Year
            };
            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, bookDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                _logger.LogWarning("Book id mismatch: route id {RouteId}, book id {BookId}", id, book.Id);
                return BadRequest();
            }
            _logger.LogInformation("Updating book with id {BookId}", id);
            await _bookRepository.Update(book);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            _logger.LogInformation("Deleting book with id {BookId}", id);
            var book = await _bookRepository.GetById(id);
            if (book == null)
            {
                _logger.LogWarning("Book with id {BookId} not found for deletion", id);
                return NotFound();
            }
            await _bookRepository.Delete(id);
            _logger.LogInformation("Book with id {BookId} deleted", id);
            return NoContent();
        }
    }
}

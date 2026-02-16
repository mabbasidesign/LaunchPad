using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaunchPad.Models;
using LaunchPad.Data;
using Microsoft.EntityFrameworkCore;

namespace LaunchPad.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.Extensions.Caching.Distributed.IDistributedCache _cache;

        public BookRepository(AppDbContext context, Microsoft.Extensions.Caching.Distributed.IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public async Task<IEnumerable<Book>> GetAll()
        {
            // return await _context.Books.AsNoTracking().ToListAsync();

             var cacheKey = "books_all";
            var cachedBooks = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedBooks))
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<Book>>(cachedBooks) ?? new List<Book>();
            }

            var books = await _context.Books.AsNoTracking().ToListAsync();
            if (books != null && books.Count > 0)
            {
                var serialized = System.Text.Json.JsonSerializer.Serialize(books);
                await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(5)
                });
            }
            return books;
        }
        
        public async Task<Book?> GetById(int id)
        {
            var cacheKey = $"book_{id}";
            var cachedBook = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedBook))
            {
                return System.Text.Json.JsonSerializer.Deserialize<Book>(cachedBook);
            }

            var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            if (book != null)
            {
                var serialized = System.Text.Json.JsonSerializer.Serialize(book);
                await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(5)
                });
                return book;
            }
            return null;
        }

        public async Task Add(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}

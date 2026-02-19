using System.Collections.Generic;
using System.Threading.Tasks;
using LaunchPad.Models;

namespace LaunchPad.Services
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAll();
        Task<(List<Book> books, int totalCount)> GetAllPaginatedAsync(int pageNumber, int pageSize);
        Task<Book?> GetById(int id);
        Task<bool> ExistsAsync(int id);
        Task Add(Book book);
        Task Update(Book book);
        Task Delete(int id);
    }
}

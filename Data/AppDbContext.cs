using Microsoft.EntityFrameworkCore;
using LaunchPad.Models;

namespace LaunchPad.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "The Pragmatic Programmer", Author = "Andrew Hunt, David Thomas", ISBN = "978-0201616224", Price = 42.99M, Stock = 10 },
                new Book { Id = 2, Title = "Clean Code", Author = "Robert C. Martin", ISBN = "978-0132350884", Price = 37.99M, Stock = 8 },
                new Book { Id = 3, Title = "Design Patterns", Author = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides", ISBN = "978-0201633610", Price = 54.99M, Stock = 5 }
            );
        }
    }
}
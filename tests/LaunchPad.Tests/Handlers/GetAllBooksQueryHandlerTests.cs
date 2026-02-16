using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using LaunchPad.Services;
using LaunchPad.Models;
using LaunchPad.DTO;
using LaunchPad.Features.Books.Queries;
using Microsoft.Extensions.Logging;

namespace LaunchPad.Tests.Handlers
{
    public class GetAllBooksQueryHandlerTests
    {
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<ILogger<GetAllBooksQueryHandler>> _mockLogger;

        public GetAllBooksQueryHandlerTests()
        {
            _mockBookRepository = new Mock<IBookRepository>();
            _mockLogger = new Mock<ILogger<GetAllBooksQueryHandler>>();
        }

        [Fact]
        public async Task Handle_ReturnsAllBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = "Author 1", Year = 2024 },
                new Book { Id = 2, Title = "Book 2", Author = "Author 2", Year = 2025 }
            };

            _mockBookRepository.Setup(x => x.GetAll())
                .ReturnsAsync(books);

            var handler = new GetAllBooksQueryHandler(_mockBookRepository.Object, _mockLogger.Object);
            var query = new GetAllBooksQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Book 1", result[0].Title);
            Assert.Equal("Book 2", result[1].Title);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyListWhenNoBooks()
        {
            // Arrange
            _mockBookRepository.Setup(x => x.GetAll())
                .ReturnsAsync(new List<Book>());

            var handler = new GetAllBooksQueryHandler(_mockBookRepository.Object, _mockLogger.Object);
            var query = new GetAllBooksQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_CallsRepositoryGetAll()
        {
            // Arrange
            _mockBookRepository.Setup(x => x.GetAll())
                .ReturnsAsync(new List<Book>());

            var handler = new GetAllBooksQueryHandler(_mockBookRepository.Object, _mockLogger.Object);
            var query = new GetAllBooksQuery();

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            _mockBookRepository.Verify(x => x.GetAll(), Times.Once);
        }
    }
}

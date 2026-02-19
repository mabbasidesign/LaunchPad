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

            _mockBookRepository.Setup(x => x.GetAllPaginatedAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((books, 2));

            var handler = new GetAllBooksQueryHandler(_mockBookRepository.Object, _mockLogger.Object);
            var query = new GetAllBooksQuery(1, 10);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal("Book 1", result.Items[0].Title);
            Assert.Equal("Book 2", result.Items[1].Title);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyListWhenNoBooks()
        {
            // Arrange
            _mockBookRepository.Setup(x => x.GetAllPaginatedAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((new List<Book>(), 0));

            var handler = new GetAllBooksQueryHandler(_mockBookRepository.Object, _mockLogger.Object);
            var query = new GetAllBooksQuery(1, 10);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task Handle_CallsRepositoryGetAllPaginated()
        {
            // Arrange
            _mockBookRepository.Setup(x => x.GetAllPaginatedAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((new List<Book>(), 0));

            var handler = new GetAllBooksQueryHandler(_mockBookRepository.Object, _mockLogger.Object);
            var query = new GetAllBooksQuery(2, 5);

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            _mockBookRepository.Verify(x => x.GetAllPaginatedAsync(2, 5), Times.Once);
        }
    }
}

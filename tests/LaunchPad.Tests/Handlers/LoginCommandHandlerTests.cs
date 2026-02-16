using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using LaunchPad.Services;
using LaunchPad.Models;
using LaunchPad.Features.Auth.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LaunchPad.Tests.Handlers
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<LoginCommandHandler>> _mockLogger;

        public LoginCommandHandlerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<LoginCommandHandler>>();
        }

        [Fact]
        public async Task Handle_WithValidCredentials_ReturnsTokenResponse()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", PasswordHash = "hash" };
            _mockAuthService.Setup(x => x.ValidateUserAsync("testuser", "password123"))
                .ReturnsAsync(user);

            // Setup configuration for JWT
            var configSection = new Mock<IConfigurationSection>();
            configSection.Setup(x => x["SecretKey"]).Returns("yourSuperSecretKey1234567890!@#$%^");
            configSection.Setup(x => x["Issuer"]).Returns("LaunchPadAPI");
            configSection.Setup(x => x["Audience"]).Returns("LaunchPadUsers");
            
            _mockConfiguration.Setup(x => x.GetSection("JwtSettings"))
                .Returns(configSection.Object);

            var handler = new LoginCommandHandler(_mockAuthService.Object, _mockConfiguration.Object, _mockLogger.Object);
            var command = new LoginCommand("testuser", "password123");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal("Login successful", result.Message);
            _mockAuthService.Verify(x => x.ValidateUserAsync("testuser", "password123"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidCredentials_ReturnsEmptyToken()
        {
            // Arrange
            _mockAuthService.Setup(x => x.ValidateUserAsync("testuser", "wrongpassword"))
                .ReturnsAsync((User)null);

            var handler = new LoginCommandHandler(_mockAuthService.Object, _mockConfiguration.Object, _mockLogger.Object);
            var command = new LoginCommand("testuser", "wrongpassword");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Token);
            Assert.Equal("Invalid username or password", result.Message);
        }

        [Fact]
        public async Task Handle_CallsAuthServiceOnce()
        {
            // Arrange
            _mockAuthService.Setup(x => x.ValidateUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((User)null);

            var handler = new LoginCommandHandler(_mockAuthService.Object, _mockConfiguration.Object, _mockLogger.Object);
            var command = new LoginCommand("testuser", "password123");

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert - Verify the service was called exactly once
            _mockAuthService.Verify(
                x => x.ValidateUserAsync("testuser", "password123"),
                Times.Once,
                "AuthService.ValidateUserAsync should be called exactly once");
        }
    }
}

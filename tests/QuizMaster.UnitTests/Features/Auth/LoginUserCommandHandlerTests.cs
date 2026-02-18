using AutoMapper;
using FluentAssertions;
using Moq;
using QuizMaster.Application.Features.Auth.Commands.LoginUser;
using QuizMaster.Core.Entities;
using QuizMaster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace QuizMaster.UnitTests.Features.Auth
{
    public class LoginUserCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly LoginUserCommandHandler _handler;

        public LoginUserCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTokenService = new Mock<ITokenService>();
            _mockMapper = new Mock<IMapper>();
            _handler = new LoginUserCommandHandler(
                _mockUnitOfWork.Object,
                _mockTokenService.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsSuccessWithToken()
        {
            // Arrange
            var command = new LoginUserCommand
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                CreatedAt = DateTime.UtcNow
            };

            var users = new List<User> { user };

            _mockUnitOfWork.Setup(x => x.Users.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ReturnsAsync(users);

            _mockTokenService.Setup(x => x.GenerateToken(user))
                .Returns("fake-jwt-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Token.Should().Be("fake-jwt-token");
        }

        [Fact]
        public async Task Handle_InvalidEmail_ReturnsFailure()
        {
            // Arrange
            var command = new LoginUserCommand
            {
                Email = "nonexistent@example.com",
                Password = "Password123"
            };

            _mockUnitOfWork.Setup(x => x.Users.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid credentials");
        }

        [Fact]
        public async Task Handle_InvalidPassword_ReturnsFailure()
        {
            // Arrange
            var command = new LoginUserCommand
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123"),
                CreatedAt = DateTime.UtcNow
            };

            _mockUnitOfWork.Setup(x => x.Users.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ReturnsAsync(new List<User> { user });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid credentials");
        }
    }
}
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using QuizMaster.Application.Features.Auth.Commands.RegisterUser;
using QuizMaster.Application.Features.Auth.Commands.LoginUser;

namespace QuizMaster.IntegrationTests.Controllers
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ValidData_ReturnsSuccess()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Username = $"testuser{Guid.NewGuid()}",
                Email = $"test{Guid.NewGuid()}@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange - First register
            var username = $"testuser{Guid.NewGuid()}";
            var email = $"test{Guid.NewGuid()}@example.com";
            var password = "Password123";

            await _client.PostAsJsonAsync("/api/auth/register", new RegisterUserCommand
            {
                Username = username,
                Email = email,
                Password = password,
                ConfirmPassword = password
            });

            // Act - Then login
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginUserCommand
            {
                Email = email,
                Password = password
            });

            // Assert
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await loginResponse.Content.ReadAsStringAsync();
            content.Should().Contain("token");
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var command = new LoginUserCommand
            {
                Email = "invalid@example.com",
                Password = "WrongPassword"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
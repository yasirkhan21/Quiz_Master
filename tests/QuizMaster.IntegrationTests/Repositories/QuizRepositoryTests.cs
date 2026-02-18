using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Core.Entities;
using QuizMaster.Infrastructure.Data;
using QuizMaster.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QuizMaster.IntegrationTests.Repositories
{
    public class QuizRepositoryTests : IDisposable
    {
        private readonly QuizMasterDbContext _context;
        private readonly UnitOfWork _unitOfWork;

        public QuizRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<QuizMasterDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new QuizMasterDbContext(options);
            _unitOfWork = new UnitOfWork(_context);
        }

        [Fact]
        public async Task AddUser_ShouldSaveToDatabase()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                CreatedAt = DateTime.UtcNow
            };

            // Act
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Assert
            var savedUser = await _unitOfWork.Users.GetByIdAsync(user.UserId);
            savedUser.Should().NotBeNull();
            savedUser!.Username.Should().Be("testuser");
            savedUser.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task FindUserByEmail_ShouldReturnCorrectUser()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = "findme",
                Email = "findme@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Act
            var users = await _unitOfWork.Users.FindAsync(u => u.Email == "findme@example.com");
            var foundUser = users.FirstOrDefault();

            // Assert
            foundUser.Should().NotBeNull();
            foundUser!.Username.Should().Be("findme");
        }

        [Fact]
        public async Task CreateQuizWithQuestions_ShouldSaveRelatedEntities()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = "quizuser",
                Email = "quiz@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);

            var quiz = new Quiz
            {
                QuizId = Guid.NewGuid(),
                UserId = user.UserId,
                Category = "Science",
                Difficulty = "easy",
                TotalScore = 0,
                Accuracy = 0,
                AverageTimePerQuestion = 0,
                Timestamp = DateTime.UtcNow,
                IsCompleted = false
            };

            await _unitOfWork.Quizzes.AddAsync(quiz);

            var question = new Question
            {
                QuestionId = Guid.NewGuid(),
                QuizId = quiz.QuizId,
                QuestionText = "What is H2O?",
                CorrectAnswer = "Water",
                Category = "Science",
                Difficulty = "easy",
                Type = "multiple",
                IsCorrect = false,
                TimeSpent = 0
            };

            await _unitOfWork.Questions.AddAsync(question);
            await _unitOfWork.SaveChangesAsync();

            // Act
            var savedQuiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.QuizId == quiz.QuizId);

            // Assert
            savedQuiz.Should().NotBeNull();
            savedQuiz!.Questions.Should().HaveCount(1);
            savedQuiz.Questions.First().QuestionText.Should().Be("What is H2O?");
        }

        [Fact]
        public async Task DeleteQuiz_ShouldCascadeDeleteQuestions()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = "deleteuser",
                Email = "delete@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);

            var quiz = new Quiz
            {
                QuizId = Guid.NewGuid(),
                UserId = user.UserId,
                Category = "Math",
                Difficulty = "medium",
                Timestamp = DateTime.UtcNow,
                IsCompleted = false
            };

            await _unitOfWork.Quizzes.AddAsync(quiz);

            var question = new Question
            {
                QuestionId = Guid.NewGuid(),
                QuizId = quiz.QuizId,
                QuestionText = "2 + 2 = ?",
                CorrectAnswer = "4",
                Category = "Math",
                Difficulty = "easy",
                Type = "multiple"
            };

            await _unitOfWork.Questions.AddAsync(question);
            await _unitOfWork.SaveChangesAsync();

            // Act
            await _unitOfWork.Quizzes.DeleteAsync(quiz);
            await _unitOfWork.SaveChangesAsync();

            // Assert
            var deletedQuiz = await _unitOfWork.Quizzes.GetByIdAsync(quiz.QuizId);
            var questions = await _unitOfWork.Questions.FindAsync(q => q.QuizId == quiz.QuizId);

            deletedQuiz.Should().BeNull();
            questions.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateUserWeaknessProfile_ShouldPersist()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = "weakuser",
                Email = "weak@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);

            var profile = new UserWeaknessProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.UserId,
                Topic = "Math",
                WeightScore = 1.5m,
                LastUpdated = DateTime.UtcNow
            };

            await _unitOfWork.WeaknessProfiles.AddAsync(profile);
            await _unitOfWork.SaveChangesAsync();

            // Act
            profile.WeightScore = 2.5m;
            profile.LastUpdated = DateTime.UtcNow;
            await _unitOfWork.WeaknessProfiles.UpdateAsync(profile);
            await _unitOfWork.SaveChangesAsync();

            // Assert
            var updated = await _unitOfWork.WeaknessProfiles.GetByIdAsync(profile.Id);
            updated.Should().NotBeNull();
            updated!.WeightScore.Should().Be(2.5m);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
using FluentAssertions;
using QuizMaster.Application.Algorithms;
using QuizMaster.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace QuizMaster.UnitTests.Algorithms
{
    public class RecommendationEngineTests
    {
        private readonly RecommendationEngine _engine;

        public RecommendationEngineTests()
        {
            _engine = new RecommendationEngine();
        }

        [Fact]
        public void BuildWeaknessVector_ShouldNormalizeWeights()
        {
            // Arrange
            var profiles = new List<UserWeaknessProfile>
            {
                new() { Topic = "Math", WeightScore = 3.0m, LastUpdated = DateTime.UtcNow },
                new() { Topic = "Science", WeightScore = 1.0m, LastUpdated = DateTime.UtcNow }
            };

            // Act
            var vector = _engine.BuildWeaknessVector(profiles);

            // Assert
            vector.Should().ContainKey("Math");
            vector.Should().ContainKey("Science");
            vector["Math"].Should().BeGreaterThan(vector["Science"]);
        }

        [Fact]
        public void CalculateCosineSimilarity_IdenticalVectors_ReturnsOne()
        {
            // Arrange
            var vector = new Dictionary<string, double>
            {
                { "Math", 1.0 },
                { "Science", 1.0 }
            };

            // Act
            var similarity = _engine.CalculateCosineSimilarity(vector, vector);

            // Assert
            similarity.Should().BeApproximately(1.0, 0.001);
        }

        [Fact]
        public void CalculateCosineSimilarity_OrthogonalVectors_ReturnsZero()
        {
            // Arrange
            var vectorA = new Dictionary<string, double> { { "Math", 1.0 } };
            var vectorB = new Dictionary<string, double> { { "Science", 1.0 } };

            // Act
            var similarity = _engine.CalculateCosineSimilarity(vectorA, vectorB);

            // Assert
            similarity.Should().Be(0.0);
        }

        [Fact]
        public void RankQuestionsByWeakness_ShouldPrioritizeWeakTopics()
        {
            // Arrange
            var weaknessVector = new Dictionary<string, double>
            {
                { "Math", 0.8 },
                { "Science", 0.2 }
            };

            var questions = new List<Question>
            {
                new() { QuestionId = Guid.NewGuid(), Category = "Math", TagsJson = "[\"Math\"]" },
                new() { QuestionId = Guid.NewGuid(), Category = "Science", TagsJson = "[\"Science\"]" },
                new() { QuestionId = Guid.NewGuid(), Category = "History", TagsJson = "[\"History\"]" }
            };

            // Act
            var ranked = _engine.RankQuestionsByWeakness(
                questions,
                weaknessVector,
                new List<Guid>(),
                10
            );

            // Assert
            ranked.First().Question.Category.Should().Be("Math");
        }
    }
}
using FluentAssertions;
using QuizMaster.Application.Algorithms;
using QuizMaster.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace QuizMaster.UnitTests.Algorithms
{
    public class ScoringEngineTests
    {
        private readonly ScoringEngine _engine;

        public ScoringEngineTests()
        {
            _engine = new ScoringEngine();
        }

        [Fact]
        public void CalculateScore_CorrectAnswer_ReturnsBasePoints()
        {
            // Arrange
            var context = new ScoringContext
            {
                IsCorrect = true,
                BasePoints = 10,
                Difficulty = "easy",
                TimeSpent = 10,
                CorrectStreak = 0
            };

            var rules = new List<ScoringRule>();

            // Act
            var result = _engine.CalculateScore(context, rules);

            // Assert
            result.FinalPoints.Should().Be(10);
        }

        [Fact]
        public void CalculateScore_IncorrectAnswer_ReturnsZeroPoints()
        {
            // Arrange
            var context = new ScoringContext
            {
                IsCorrect = false,
                BasePoints = 10
            };

            // Act
            var result = _engine.CalculateScore(context, new List<ScoringRule>());

            // Assert
            result.FinalPoints.Should().Be(0);
        }

        [Fact]
        public void CalculateScore_SpeedBonus_AddsExtraPoints()
        {
            // Arrange
            var context = new ScoringContext
            {
                IsCorrect = true,
                BasePoints = 10,
                TimeSpent = 4,
                Difficulty = "easy"
            };

            var speedRule = new ScoringRule
            {
                RuleId = Guid.NewGuid(),
                RuleName = "Speed Bonus",
                IsActive = true,
                Priority = 1,
                ConditionsJson = JsonSerializer.Serialize(new List<RuleCondition>
                {
                    new() { Type = "time_under", Value = 5 },
                    new() { Type = "is_correct", Value = 1 }
                }),
                ActionsJson = JsonSerializer.Serialize(new List<RuleAction>
                {
                    new() { Type = "add_points", Value = 5 }
                })
            };

            // Act
            var result = _engine.CalculateScore(context, new[] { speedRule });

            // Assert
            result.FinalPoints.Should().Be(15); // 10 base + 5 speed bonus
            result.AppliedRules.Should().Contain("Speed Bonus");
        }

        [Fact]
        public void CalculateScore_WeakTopic_AppliesMultiplier()
        {
            // Arrange
            var context = new ScoringContext
            {
                IsCorrect = true,
                BasePoints = 10,
                IsWeakTopic = true,
                Category = "Math"
            };

            // Act
            var result = _engine.CalculateScore(context, new List<ScoringRule>());

            // Assert
            result.FinalPoints.Should().Be(15); // 10 * 1.5
            result.FeedbackMessages.Should().Contain(m => m.Contains("Math"));
        }
    }
}
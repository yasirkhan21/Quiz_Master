using QuizMaster.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace QuizMaster.Application.Algorithms
{
    // Rule Models for JSON deserialization
    public class RuleCondition
    {
        public string Type { get; set; }       // "correct_streak", "time_under", "difficulty"
        public string Operator { get; set; }   // "AND", "OR"
        public int? Value { get; set; }
        public string StringValue { get; set; }
        public List<RuleCondition> SubConditions { get; set; }
    }

    public class RuleAction
    {
        public string Type { get; set; }       // "add_points", "multiply_points", "subtract_points"
        public int Value { get; set; }
        public string Message { get; set; }
    }

    public class ScoringContext
    {
        public int CorrectStreak { get; set; }
        public decimal TimeSpent { get; set; }
        public string Difficulty { get; set; }
        public bool IsCorrect { get; set; }
        public int BasePoints { get; set; }
        public bool IsWeakTopic { get; set; }
        public string Category { get; set; }
        public int TotalQuestionsAnswered { get; set; }
    }

    public class ScoringResult
    {
        public int FinalPoints { get; set; }
        public List<string> AppliedRules { get; set; } = new();
        public List<string> FeedbackMessages { get; set; } = new();
        public string MotivationalMessage { get; set; }
    }

    public class ScoringEngine
    {
        // Evaluate all active rules and compute final score
        public ScoringResult CalculateScore(
            ScoringContext context,
            IEnumerable<ScoringRule> rules)
        {
            var result = new ScoringResult
            {
                FinalPoints = context.BasePoints
            };

            if (!context.IsCorrect)
            {
                result.FinalPoints = 0;
                result.MotivationalMessage = GetMotivationalMessage(context);
                return result;
            }

            // Sort rules by priority
            var orderedRules = rules
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.Priority)
                .ToList();

            foreach (var rule in orderedRules)
            {
                try
                {
                    var conditions = JsonSerializer
                        .Deserialize<List<RuleCondition>>(rule.ConditionsJson);
                    var actions = JsonSerializer
                        .Deserialize<List<RuleAction>>(rule.ActionsJson);

                    if (conditions == null || actions == null) continue;

                    if (EvaluateConditions(conditions, context))
                    {
                        ApplyActions(actions, result, context);
                        result.AppliedRules.Add(rule.RuleName);
                    }
                }
                catch
                {
                    // Skip malformed rules
                    continue;
                }
            }

            // Apply adaptive scoring for weak topics
            if (context.IsWeakTopic && context.IsCorrect)
            {
                result.FinalPoints = (int)(result.FinalPoints * 1.5);
                result.FeedbackMessages.Add(
                    $"🌟 Bonus points! You're improving in {context.Category}!");
            }

            result.FinalPoints = Math.Max(0, result.FinalPoints);
            result.MotivationalMessage = GetMotivationalMessage(context);

            return result;
        }

        // Evaluate conditions using AND/OR logic
        private bool EvaluateConditions(
            List<RuleCondition> conditions,
            ScoringContext context)
        {
            if (!conditions.Any()) return false;

            // Default top-level is AND
            foreach (var condition in conditions)
            {
                if (!EvaluateSingleCondition(condition, context))
                    return false;
            }

            return true;
        }

        private bool EvaluateSingleCondition(
            RuleCondition condition,
            ScoringContext context)
        {
            // Handle compound conditions
            if (condition.SubConditions != null && condition.SubConditions.Any())
            {
                return condition.Operator?.ToUpper() == "OR"
                    ? condition.SubConditions.Any(c => EvaluateSingleCondition(c, context))
                    : condition.SubConditions.All(c => EvaluateSingleCondition(c, context));
            }

            // Evaluate atomic conditions
            return condition.Type?.ToLower() switch
            {
                "correct_streak" => context.CorrectStreak >= (condition.Value ?? 0),
                "time_under" => context.TimeSpent < (condition.Value ?? 999),
                "time_over" => context.TimeSpent > (condition.Value ?? 0),
                "difficulty" => string.Equals(
                    context.Difficulty,
                    condition.StringValue,
                    StringComparison.OrdinalIgnoreCase),
                "is_correct" => context.IsCorrect == (condition.Value == 1),
                "is_weak_topic" => context.IsWeakTopic == (condition.Value == 1),
                "questions_answered" => context.TotalQuestionsAnswered >= (condition.Value ?? 0),
                _ => false
            };
        }

        private void ApplyActions(
            List<RuleAction> actions,
            ScoringResult result,
            ScoringContext context)
        {
            foreach (var action in actions)
            {
                switch (action.Type?.ToLower())
                {
                    case "add_points":
                        result.FinalPoints += action.Value;
                        break;
                    case "multiply_points":
                        result.FinalPoints = (int)(result.FinalPoints * action.Value);
                        break;
                    case "subtract_points":
                        result.FinalPoints = Math.Max(0, result.FinalPoints - action.Value);
                        break;
                    case "set_points":
                        result.FinalPoints = action.Value;
                        break;
                    case "add_message":
                        if (!string.IsNullOrEmpty(action.Message))
                            result.FeedbackMessages.Add(action.Message);
                        break;
                }
            }
        }

        private string GetMotivationalMessage(ScoringContext context)
        {
            if (!context.IsCorrect)
            {
                return context.IsWeakTopic
                    ? $"💪 Keep practicing {context.Category} — you'll get it!"
                    : "Don't give up! Review and try again.";
            }

            return context.CorrectStreak switch
            {
                >= 10 => "🔥 Unstoppable! You're on fire!",
                >= 5 => "⚡ Amazing streak! Keep going!",
                >= 3 => "🎯 Great streak! You're in the zone!",
                _ => context.Difficulty?.ToLower() switch
                {
                    "hard" => "🏆 Impressive! That was a tough question!",
                    "medium" => "✅ Well done! Keep it up!",
                    _ => "👍 Correct! Nice work!"
                }
            };
        }

        // Build default rules for seeding
        public static List<ScoringRule> GetDefaultRules()
        {
            return new List<ScoringRule>
            {
                new ScoringRule
                {
                    RuleId = Guid.NewGuid(),
                    RuleName = "Speed Bonus",
                    Description = "Award bonus points for answering within 5 seconds",
                    ConditionsJson = JsonSerializer.Serialize(new List<RuleCondition>
                    {
                        new RuleCondition { Type = "time_under", Value = 5 },
                        new RuleCondition { Type = "is_correct", Value = 1 }
                    }),
                    ActionsJson = JsonSerializer.Serialize(new List<RuleAction>
                    {
                        new RuleAction { Type = "add_points", Value = 5 },
                        new RuleAction { Type = "add_message", Message = "⚡ Speed bonus!" }
                    }),
                    IsActive = true,
                    Scope = "SinglePlayer",
                    Priority = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new ScoringRule
                {
                    RuleId = Guid.NewGuid(),
                    RuleName = "Streak Bonus x3",
                    Description = "Double points for 3 correct answers in a row",
                    ConditionsJson = JsonSerializer.Serialize(new List<RuleCondition>
                    {
                        new RuleCondition { Type = "correct_streak", Value = 3 },
                        new RuleCondition { Type = "is_correct", Value = 1 }
                    }),
                    ActionsJson = JsonSerializer.Serialize(new List<RuleAction>
                    {
                        new RuleAction { Type = "multiply_points", Value = 2 },
                        new RuleAction { Type = "add_message", Message = "🔥 3x Streak! Double points!" }
                    }),
                    IsActive = true,
                    Scope = "SinglePlayer",
                    Priority = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new ScoringRule
                {
                    RuleId = Guid.NewGuid(),
                    RuleName = "Hard Question Bonus",
                    Description = "Extra points for correct hard questions",
                    ConditionsJson = JsonSerializer.Serialize(new List<RuleCondition>
                    {
                        new RuleCondition { Type = "difficulty", StringValue = "hard" },
                        new RuleCondition { Type = "is_correct", Value = 1 }
                    }),
                    ActionsJson = JsonSerializer.Serialize(new List<RuleAction>
                    {
                        new RuleAction { Type = "add_points", Value = 10 },
                        new RuleAction { Type = "add_message", Message = "💎 Hard question bonus!" }
                    }),
                    IsActive = true,
                    Scope = "SinglePlayer",
                    Priority = 3,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }
    }
}
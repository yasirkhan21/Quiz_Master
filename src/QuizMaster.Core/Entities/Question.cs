using System;
using System.Collections.Generic;
using System.Text.Json;

namespace QuizMaster.Core.Entities
{
    public class Question
    {
        public Guid QuestionId { get; set; }
        public Guid QuizId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? UserAnswer { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
        public decimal TimeSpent { get; set; } = 0;
        public string Category { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? IncorrectAnswersJson { get; set; }
        public string? TagsJson { get; set; } // Store tags as JSON string

        // Navigation property
        public virtual Quiz Quiz { get; set; } = null!;

        // Computed - not mapped to DB
        public List<string> Tags
        {
            get => string.IsNullOrEmpty(TagsJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(TagsJson) ?? new List<string>();
            set => TagsJson = JsonSerializer.Serialize(value);
        }
    }
}
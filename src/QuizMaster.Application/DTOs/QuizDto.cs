using System;
using System.Collections.Generic;

namespace QuizMaster.Application.DTOs
{
    public class QuizDto
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public int TotalScore { get; set; }
        public decimal Accuracy { get; set; }
        public decimal AverageTimePerQuestion { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsCompleted { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }

    public class CreateQuizDto
    {
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public int NumberOfQuestions { get; set; } = 10;
    }

    public class QuizSummaryDto
    {
        public Guid QuizId { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public int TotalScore { get; set; }
        public decimal Accuracy { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
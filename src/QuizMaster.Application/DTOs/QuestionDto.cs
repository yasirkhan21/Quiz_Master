using System;
using System.Collections.Generic;

namespace QuizMaster.Application.DTOs
{
    public class QuestionDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public string Type { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
        public string UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public decimal TimeSpent { get; set; }
    }

    public class SubmitAnswerDto
    {
        public Guid QuizId { get; set; }
        public Guid QuestionId { get; set; }
        public string UserAnswer { get; set; }
        public decimal TimeSpent { get; set; }
    }

    public class AnswerResultDto
    {
        public bool IsCorrect { get; set; }
        public string CorrectAnswer { get; set; }
        public int PointsEarned { get; set; }
        public string Feedback { get; set; }
        public string FeedbackEmoji { get; set; }
    }
}
using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using System;
using System.Collections.Generic;

namespace QuizMaster.Application.Features.Quizzes.Queries.GetRecommendedQuestions
{
    public class GetRecommendedQuestionsQuery : IRequest<ApiResponse<List<QuestionDto>>>
    {
        public Guid UserId { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public int Count { get; set; } = 10;
    }
}
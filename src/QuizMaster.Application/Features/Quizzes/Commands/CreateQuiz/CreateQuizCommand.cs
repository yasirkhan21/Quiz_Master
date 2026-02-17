using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using System;

namespace QuizMaster.Application.Features.Quizzes.Commands.CreateQuiz
{
    public class CreateQuizCommand : IRequest<ApiResponse<QuizDto>>
    {
        public Guid UserId { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public int NumberOfQuestions { get; set; } = 10;
    }
}
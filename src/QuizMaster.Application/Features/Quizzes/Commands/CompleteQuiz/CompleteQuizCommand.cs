using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using System;

namespace QuizMaster.Application.Features.Quizzes.Commands.CompleteQuiz
{
    public class CompleteQuizCommand : IRequest<ApiResponse<QuizSummaryDto>>
    {
        public Guid UserId { get; set; }
        public Guid QuizId { get; set; }
    }
}
using MediatR;
using QuizMaster.Application.Common;
using System;

namespace QuizMaster.Application.Features.Quizzes.Commands.DeleteQuiz
{
    public class DeleteQuizCommand : IRequest<ApiResponse<bool>>
    {
        public Guid UserId { get; set; }
        public Guid QuizId { get; set; }
    }
}
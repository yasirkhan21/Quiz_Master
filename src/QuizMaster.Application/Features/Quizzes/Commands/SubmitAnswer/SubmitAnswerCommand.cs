using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using System;

namespace QuizMaster.Application.Features.Quizzes.Commands.SubmitAnswer
{
    public class SubmitAnswerCommand : IRequest<ApiResponse<AnswerResultDto>>
    {
        public Guid UserId { get; set; }
        public Guid QuizId { get; set; }
        public Guid QuestionId { get; set; }
        public string UserAnswer { get; set; }
        public decimal TimeSpent { get; set; }
    }
}
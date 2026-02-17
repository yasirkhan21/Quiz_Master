using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using System;

namespace QuizMaster.Application.Features.Quizzes.Queries.GetQuizById
{
    public class GetQuizByIdQuery : IRequest<ApiResponse<QuizDto>>
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
    }
}
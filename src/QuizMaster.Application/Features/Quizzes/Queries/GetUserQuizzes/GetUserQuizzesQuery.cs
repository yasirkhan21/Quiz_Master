using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using System;
using System.Collections.Generic;

namespace QuizMaster.Application.Features.Quizzes.Queries.GetUserQuizzes
{
    public class GetUserQuizzesQuery : IRequest<ApiResponse<List<QuizSummaryDto>>>
    {
        public Guid UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
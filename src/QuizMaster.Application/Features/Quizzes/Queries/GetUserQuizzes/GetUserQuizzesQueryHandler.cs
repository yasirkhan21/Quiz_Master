using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using QuizMaster.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster.Application.Features.Quizzes.Queries.GetUserQuizzes
{
    public class GetUserQuizzesQueryHandler
        : IRequestHandler<GetUserQuizzesQuery, ApiResponse<List<QuizSummaryDto>>>
    {
        private readonly IQuizMasterDbContext _context;
        private readonly IMapper _mapper;

        public GetUserQuizzesQueryHandler(IQuizMasterDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<QuizSummaryDto>>> Handle(
            GetUserQuizzesQuery request,
            CancellationToken cancellationToken)
        {
            var quizzes = await _context.Quizzes
                .Include(q => q.Questions)
                .Where(q => q.UserId == request.UserId)
                .OrderByDescending(q => q.Timestamp)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var quizDtos = _mapper.Map<List<QuizSummaryDto>>(quizzes);
            return ApiResponse<List<QuizSummaryDto>>.SuccessResult(
                quizDtos,
                "Quizzes retrieved successfully");
        }
    }
}
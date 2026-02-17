using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using QuizMaster.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster.Application.Features.Quizzes.Queries.GetQuizById
{
    public class GetQuizByIdQueryHandler
        : IRequestHandler<GetQuizByIdQuery, ApiResponse<QuizDto>>
    {
        private readonly IQuizMasterDbContext _context;
        private readonly IMapper _mapper;

        public GetQuizByIdQueryHandler(IQuizMasterDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<QuizDto>> Handle(
            GetQuizByIdQuery request,
            CancellationToken cancellationToken)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q =>
                    q.QuizId == request.QuizId &&
                    q.UserId == request.UserId,
                    cancellationToken);

            if (quiz == null)
                return ApiResponse<QuizDto>.FailureResult("Quiz not found");

            var quizDto = _mapper.Map<QuizDto>(quiz);
            return ApiResponse<QuizDto>.SuccessResult(quizDto);
        }
    }
}
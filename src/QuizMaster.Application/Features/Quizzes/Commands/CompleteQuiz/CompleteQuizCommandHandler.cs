using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using QuizMaster.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster.Application.Features.Quizzes.Commands.CompleteQuiz
{
    public class CompleteQuizCommandHandler
        : IRequestHandler<CompleteQuizCommand, ApiResponse<QuizSummaryDto>>
    {
        private readonly IQuizMasterDbContext _context;
        private readonly IMapper _mapper;

        public CompleteQuizCommandHandler(IQuizMasterDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<QuizSummaryDto>> Handle(
            CompleteQuizCommand request,
            CancellationToken cancellationToken)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q =>
                    q.QuizId == request.QuizId &&
                    q.UserId == request.UserId,
                    cancellationToken);

            if (quiz == null)
                return ApiResponse<QuizSummaryDto>.FailureResult("Quiz not found");

            if (quiz.IsCompleted)
                return ApiResponse<QuizSummaryDto>.FailureResult("Quiz already completed");

            var questions = quiz.Questions.ToList();
            int totalQuestions = questions.Count;
            int correctAnswers = questions.Count(q => q.IsCorrect);

            quiz.IsCompleted = true;
            quiz.Accuracy = totalQuestions > 0
                ? Math.Round((decimal)correctAnswers / totalQuestions * 100, 2)
                : 0;

            quiz.TotalScore = questions
                .Where(q => q.IsCorrect)
                .Sum(q => CalculatePoints(q.Difficulty, q.TimeSpent));

            quiz.AverageTimePerQuestion = totalQuestions > 0
                ? Math.Round(questions.Average(q => q.TimeSpent), 2)
                : 0;

            await _context.SaveChangesAsync(cancellationToken);

            var summaryDto = _mapper.Map<QuizSummaryDto>(quiz);
            return ApiResponse<QuizSummaryDto>.SuccessResult(summaryDto, "Quiz completed successfully");
        }

        private int CalculatePoints(string difficulty, decimal timeSpent)
        {
            int basePoints = difficulty?.ToLower() switch
            {
                "easy" => 10,
                "medium" => 20,
                "hard" => 30,
                _ => 15
            };

            return timeSpent < 5 ? basePoints + 5 :
                   timeSpent < 10 ? basePoints + 3 : basePoints;
        }
    }
}
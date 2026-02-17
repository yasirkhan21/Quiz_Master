using MediatR;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Application.Common;
using QuizMaster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster.Application.Features.Stats.Queries
{
    public class DashboardStatsDto
    {
        public int TotalQuizzesTaken { get; set; }
        public decimal AverageAccuracy { get; set; }
        public int TotalScore { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public int TotalQuestionsAnswered { get; set; }
        public string BestCategory { get; set; }
        public string WeakestCategory { get; set; }
        public decimal AverageTimePerQuestion { get; set; }
        public List<QuizHistoryDto> RecentQuizzes { get; set; } = new();
    }

    public class QuizHistoryDto
    {
        public Guid QuizId { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public decimal Accuracy { get; set; }
        public int TotalScore { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class GetDashboardStatsQuery : IRequest<ApiResponse<DashboardStatsDto>>
    {
        public Guid UserId { get; set; }
    }

    public class GetDashboardStatsQueryHandler
        : IRequestHandler<GetDashboardStatsQuery, ApiResponse<DashboardStatsDto>>
    {
        private readonly IQuizMasterDbContext _context;

        public GetDashboardStatsQueryHandler(IQuizMasterDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<DashboardStatsDto>> Handle(
            GetDashboardStatsQuery request,
            CancellationToken cancellationToken)
        {
            var quizzes = await _context.Quizzes
                .Include(q => q.Questions)
                .Where(q => q.UserId == request.UserId && q.IsCompleted)
                .OrderByDescending(q => q.Timestamp)
                .ToListAsync(cancellationToken);

            if (!quizzes.Any())
            {
                return ApiResponse<DashboardStatsDto>.SuccessResult(
                    new DashboardStatsDto(),
                    "No quiz data yet");
            }

            var allQuestions = quizzes.SelectMany(q => q.Questions).ToList();

            var categoryStats = quizzes
                .GroupBy(q => q.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Accuracy = g.Average(q => (double)q.Accuracy)
                })
                .ToList();

            var stats = new DashboardStatsDto
            {
                TotalQuizzesTaken = quizzes.Count,
                AverageAccuracy = Math.Round(quizzes.Average(q => q.Accuracy), 2),
                TotalScore = quizzes.Sum(q => q.TotalScore),
                TotalCorrectAnswers = allQuestions.Count(q => q.IsCorrect),
                TotalQuestionsAnswered = allQuestions.Count,
                AverageTimePerQuestion = allQuestions.Any()
                    ? Math.Round(allQuestions.Average(q => q.TimeSpent), 2)
                    : 0,
                BestCategory = categoryStats
                    .OrderByDescending(c => c.Accuracy)
                    .FirstOrDefault()?.Category ?? "N/A",
                WeakestCategory = categoryStats
                    .OrderBy(c => c.Accuracy)
                    .FirstOrDefault()?.Category ?? "N/A",
                RecentQuizzes = quizzes.Take(5).Select(q => new QuizHistoryDto
                {
                    QuizId = q.QuizId,
                    Category = q.Category,
                    Difficulty = q.Difficulty,
                    Accuracy = q.Accuracy,
                    TotalScore = q.TotalScore,
                    Timestamp = q.Timestamp
                }).ToList()
            };

            return ApiResponse<DashboardStatsDto>.SuccessResult(stats);
        }
    }
}
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Application.Algorithms;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using QuizMaster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster.Application.Features.Quizzes.Queries.GetRecommendedQuestions
{
    public class GetRecommendedQuestionsQueryHandler
        : IRequestHandler<GetRecommendedQuestionsQuery, ApiResponse<List<QuestionDto>>>
    {
        private readonly IQuizMasterDbContext _context;
        private readonly IOpenTriviaService _triviaService;
        private readonly RecommendationEngine _recommendationEngine;
        private readonly IMapper _mapper;

        public GetRecommendedQuestionsQueryHandler(
            IQuizMasterDbContext context,
            IOpenTriviaService triviaService,
            IMapper mapper)
        {
            _context = context;
            _triviaService = triviaService;
            _mapper = mapper;
            _recommendationEngine = new RecommendationEngine();
        }

        public async Task<ApiResponse<List<QuestionDto>>> Handle(
            GetRecommendedQuestionsQuery request,
            CancellationToken cancellationToken)
        {
            // Get user weakness profiles
            var weaknessProfiles = await _context.UserWeaknessProfiles
                .Where(w => w.UserId == request.UserId)
                .ToListAsync(cancellationToken);

            // Get recently answered question IDs
            var recentlyAnsweredIds = await _context.Questions
                .Include(q => q.Quiz)
                .Where(q => q.Quiz.UserId == request.UserId && q.UserAnswer != null)
                .OrderByDescending(q => q.Quiz.Timestamp)
                .Take(50)
                .Select(q => q.QuestionId)
                .ToListAsync(cancellationToken);

            // Build available questions query
            var availableQuestionsQuery = _context.Questions
                .Include(q => q.Quiz)
                .Where(q => q.Quiz.UserId == request.UserId);

            if (!string.IsNullOrEmpty(request.Category))
                availableQuestionsQuery = availableQuestionsQuery
                    .Where(q => q.Category == request.Category);

            if (!string.IsNullOrEmpty(request.Difficulty))
                availableQuestionsQuery = availableQuestionsQuery
                    .Where(q => q.Difficulty == request.Difficulty);

            var availableQuestions = await availableQuestionsQuery
                .ToListAsync(cancellationToken);

            List<QuestionDto> recommendedDtos;

            if (weaknessProfiles.Any() && availableQuestions.Any())
            {
                var weaknessVector = _recommendationEngine
                    .BuildWeaknessVector(weaknessProfiles);

                var rankedQuestions = _recommendationEngine.RankQuestionsByWeakness(
                    availableQuestions,
                    weaknessVector,
                    recentlyAnsweredIds,
                    request.Count);

                recommendedDtos = rankedQuestions
                    .Select(rq => _mapper.Map<QuestionDto>(rq.Question))
                    .ToList();
            }
            else
            {
                // Fallback: fetch from API
                var freshQuestions = await _triviaService.GetQuestionsAsync(
                    request.Count,
                    request.Category,
                    request.Difficulty);

                var rng = new Random();
                recommendedDtos = freshQuestions.Select(q => new QuestionDto
                {
                    QuestionText = q.Question,
                    Category = q.Category,
                    Difficulty = q.Difficulty,
                    Type = q.Type,
                    CorrectAnswer = q.CorrectAnswer,
                    Options = new List<string>(q.IncorrectAnswers) { q.CorrectAnswer }
                        .OrderBy(x => rng.Next())
                        .ToList()
                }).ToList();
            }

            return ApiResponse<List<QuestionDto>>.SuccessResult(
                recommendedDtos,
                "Recommended questions retrieved successfully");
        }
    }
}
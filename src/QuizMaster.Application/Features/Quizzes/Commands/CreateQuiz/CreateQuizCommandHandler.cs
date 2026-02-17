using AutoMapper;
using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using QuizMaster.Core.Entities;
using QuizMaster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster.Application.Features.Quizzes.Commands.CreateQuiz
{
    public class CreateQuizCommandHandler
        : IRequestHandler<CreateQuizCommand, ApiResponse<QuizDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenTriviaService _triviaService;
        private readonly IMapper _mapper;

        public CreateQuizCommandHandler(
            IUnitOfWork unitOfWork,
            IOpenTriviaService triviaService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _triviaService = triviaService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<QuizDto>> Handle(
            CreateQuizCommand request,
            CancellationToken cancellationToken)
        {
            // Fetch questions from Open Trivia DB
            var triviaQuestions = await _triviaService.GetQuestionsAsync(
                request.NumberOfQuestions,
                request.Category,
                request.Difficulty
            );

            if (triviaQuestions == null || !triviaQuestions.Any())
            {
                return ApiResponse<QuizDto>.FailureResult(
                    "Failed to fetch questions",
                    new List<string> { "Unable to retrieve questions from the trivia database" });
            }

            // Create quiz entity
            var quiz = new Quiz
            {
                QuizId = Guid.NewGuid(),
                UserId = request.UserId,
                Category = request.Category ?? "General Knowledge",
                Difficulty = request.Difficulty ?? "medium",
                Timestamp = DateTime.UtcNow,
                IsCompleted = false,
                TotalScore = 0,
                Accuracy = 0,
                AverageTimePerQuestion = 0
            };

            await _unitOfWork.Quizzes.AddAsync(quiz);

            // Create question entities
            var questions = new List<Question>();
            foreach (var tq in triviaQuestions)
            {
                var tags = ExtractTags(tq.Category);

                var question = new Question
                {
                    QuestionId = Guid.NewGuid(),
                    QuizId = quiz.QuizId,
                    QuestionText = tq.Question,
                    CorrectAnswer = tq.CorrectAnswer,
                    IncorrectAnswersJson = JsonSerializer.Serialize(tq.IncorrectAnswers),
                    Category = tq.Category,
                    Difficulty = tq.Difficulty,
                    Type = tq.Type,
                    IsCorrect = false,
                    TimeSpent = 0,
                    UserAnswer = null,       // Null until user answers
                    TagsJson = JsonSerializer.Serialize(tags)
                };

                questions.Add(question);
                await _unitOfWork.Questions.AddAsync(question);
            }

            await _unitOfWork.SaveChangesAsync();

            quiz.Questions = questions;
            var quizDto = _mapper.Map<QuizDto>(quiz);

            return ApiResponse<QuizDto>.SuccessResult(quizDto, "Quiz created successfully");
        }

        private List<string> ExtractTags(string category)
        {
            // Map category to relevant topic tags
            var tagMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Science & Nature",    new List<string> { "Science", "Nature", "Biology" } },
                { "Computers",           new List<string> { "Technology", "Programming", "IT" } },
                { "Mathematics",         new List<string> { "Math", "Numbers", "Logic" } },
                { "History",             new List<string> { "History", "Events", "Timeline" } },
                { "Geography",           new List<string> { "Geography", "Countries", "Maps" } },
                { "Sports",              new List<string> { "Sports", "Athletics", "Games" } },
                { "Music",               new List<string> { "Music", "Songs", "Artists" } },
                { "Film",                new List<string> { "Movies", "Cinema", "Film" } },
                { "General Knowledge",   new List<string> { "General", "Trivia", "Mixed" } },
                { "Animals",             new List<string> { "Animals", "Wildlife", "Nature" } },
                { "Video Games",         new List<string> { "Gaming", "VideoGames", "Technology" } },
            };

            return tagMap.TryGetValue(category, out var tags)
                ? tags
                : new List<string> { category };
        }
    }
}
using AutoMapper;
using QuizMaster.Application.DTOs;
using QuizMaster.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace QuizMaster.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<RegisterUserDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Quizzes, opt => opt.Ignore())
                .ForMember(dest => dest.WeaknessProfiles, opt => opt.Ignore());

            // Quiz mappings
            CreateMap<Quiz, QuizDto>();
            CreateMap<Quiz, QuizSummaryDto>()
                .ForMember(dest => dest.TotalQuestions,
                    opt => opt.MapFrom(src => src.Questions.Count))
                .ForMember(dest => dest.CorrectAnswers,
                    opt => opt.MapFrom(src => src.Questions.Count(q => q.IsCorrect)));

            CreateMap<CreateQuizDto, Quiz>()
                .ForMember(dest => dest.QuizId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.IsCompleted, opt => opt.Ignore())
                .ForMember(dest => dest.TotalScore, opt => opt.Ignore())
                .ForMember(dest => dest.Accuracy, opt => opt.Ignore())
                .ForMember(dest => dest.AverageTimePerQuestion, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore());

            // Question mappings
            CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.Options,
                    opt => opt.MapFrom(src => GetOptions(src.IncorrectAnswersJson, src.CorrectAnswer)));

            CreateMap<QuestionDto, Question>()
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.QuizId, opt => opt.Ignore())
                .ForMember(dest => dest.Quiz, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.IncorrectAnswersJson,
                    opt => opt.MapFrom(src => SerializeIncorrect(src.Options, src.CorrectAnswer)));
        }

        // Static methods avoid optional arguments issue in expression trees
        private static List<string> GetOptions(string incorrectAnswersJson, string correctAnswer)
        {
            if (string.IsNullOrEmpty(incorrectAnswersJson))
                return new List<string> { correctAnswer };

            var options = new List<string> { correctAnswer };

            var incorrectAnswers = JsonSerializer.Deserialize<List<string>>(incorrectAnswersJson);
            if (incorrectAnswers != null)
                options.AddRange(incorrectAnswers);

            // Shuffle using Fisher-Yates
            var random = new Random();
            for (int i = options.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (options[i], options[j]) = (options[j], options[i]);
            }

            return options;
        }

        private static string SerializeIncorrect(List<string> options, string correctAnswer)
        {
            if (options == null)
                return JsonSerializer.Serialize(new List<string>());

            var incorrect = options
                .Where(o => o != correctAnswer)
                .ToList();

            return JsonSerializer.Serialize(incorrect);
        }
    }
}
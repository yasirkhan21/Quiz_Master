using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using QuizMaster.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster.Application.Features.Quizzes.Commands.SubmitAnswer
{
    public class SubmitAnswerCommandHandler 
        : IRequestHandler<SubmitAnswerCommand, ApiResponse<AnswerResultDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmitAnswerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<AnswerResultDto>> Handle(
            SubmitAnswerCommand request, 
            CancellationToken cancellationToken)
        {
            // Get the question
            var question = await _unitOfWork.Questions.GetByIdAsync(request.QuestionId);
            if (question == null)
            {
                return ApiResponse<AnswerResultDto>.FailureResult(
                    "Question not found",
                    new List<string> { "The specified question does not exist" });
            }

            // Verify quiz ownership
            var quiz = await _unitOfWork.Quizzes.GetByIdAsync(request.QuizId);
            if (quiz == null || quiz.UserId != request.UserId)
            {
                return ApiResponse<AnswerResultDto>.FailureResult(
                    "Unauthorized",
                    new List<string> { "You don't have permission to submit answers for this quiz" });
            }

            // Check if answer is correct
            bool isCorrect = question.CorrectAnswer.Trim().Equals(
                request.UserAnswer?.Trim(), 
                StringComparison.OrdinalIgnoreCase);

            // Update question
            question.UserAnswer = request.UserAnswer;
            question.IsCorrect = isCorrect;
            question.TimeSpent = request.TimeSpent;

            await _unitOfWork.Questions.UpdateAsync(question);

            // Update user weakness profile if incorrect
            if (!isCorrect && question.Tags != null && question.Tags.Any())
            {
                await UpdateWeaknessProfile(request.UserId, question.Tags);
            }

            await _unitOfWork.SaveChangesAsync();

            // Calculate points (basic scoring)
            int pointsEarned = isCorrect ? CalculatePoints(question.Difficulty, request.TimeSpent) : 0;

            // Generate feedback
            var result = new AnswerResultDto
            {
                IsCorrect = isCorrect,
                CorrectAnswer = question.CorrectAnswer,
                PointsEarned = pointsEarned,
                Feedback = GenerateFeedback(isCorrect, question.Difficulty),
                FeedbackEmoji = GetFeedbackEmoji(isCorrect)
            };

            return ApiResponse<AnswerResultDto>.SuccessResult(result, "Answer submitted successfully");
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

            // Time bonus (faster answers get more points)
            if (timeSpent < 5)
                return basePoints + 5;
            else if (timeSpent < 10)
                return basePoints + 3;
            else
                return basePoints;
        }

        private string GenerateFeedback(bool isCorrect, string difficulty)
        {
            if (isCorrect)
            {
                return difficulty?.ToLower() switch
                {
                    "hard" => "Excellent! That was a tough one!",
                    "medium" => "Great job! Keep it up!",
                    _ => "Correct! Well done!"
                };
            }
            else
            {
                return "Not quite right. Keep learning!";
            }
        }

        private string GetFeedbackEmoji(bool isCorrect)
        {
            return isCorrect ? "🎉" : "💪";
        }

        private async Task UpdateWeaknessProfile(Guid userId, List<string> tags)
        {
            foreach (var tag in tags)
            {
                var profiles = await _unitOfWork.WeaknessProfiles.FindAsync(
                    w => w.UserId == userId && w.Topic == tag);
                
                var profile = profiles.FirstOrDefault();

                if (profile == null)
                {
                    profile = new Core.Entities.UserWeaknessProfile
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Topic = tag,
                        WeightScore = 1.0m,
                        LastUpdated = DateTime.UtcNow
                    };
                    await _unitOfWork.WeaknessProfiles.AddAsync(profile);
                }
                else
                {
                    // Increase weight and apply decay
                    profile.WeightScore = profile.WeightScore * 0.9m + 1.0m;
                    profile.LastUpdated = DateTime.UtcNow;
                    await _unitOfWork.WeaknessProfiles.UpdateAsync(profile);
                }
            }
        }
    }
}
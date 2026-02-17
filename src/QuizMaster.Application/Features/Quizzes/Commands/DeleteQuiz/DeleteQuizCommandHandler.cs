using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster.Application.Features.Quizzes.Commands.DeleteQuiz
{
    public class DeleteQuizCommandHandler
        : IRequestHandler<DeleteQuizCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteQuizCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(
            DeleteQuizCommand request,
            CancellationToken cancellationToken)
        {
            var quiz = await _unitOfWork.Quizzes.GetByIdAsync(request.QuizId);

            if (quiz == null)
                return ApiResponse<bool>.FailureResult("Quiz not found");

            if (quiz.UserId != request.UserId)
                return ApiResponse<bool>.FailureResult("Unauthorized to delete this quiz");

            await _unitOfWork.Quizzes.DeleteAsync(quiz);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Quiz deleted successfully");
        }
    }
}
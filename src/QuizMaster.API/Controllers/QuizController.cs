using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizMaster.Application.DTOs;
using QuizMaster.Application.Features.Quizzes.Commands.CompleteQuiz;
using QuizMaster.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizMaster.Application.Features.Quizzes.Commands.DeleteQuiz;
using QuizMaster.Application.Features.Quizzes.Commands.SubmitAnswer;
using QuizMaster.Application.Features.Quizzes.Queries.GetQuizById;
using QuizMaster.Application.Features.Quizzes.Queries.GetRecommendedQuestions;
using QuizMaster.Application.Features.Quizzes.Queries.GetUserQuizzes;
using System;
using System.Threading.Tasks;
using QuizMaster.Application.Common;
using QuizMaster.Core.Interfaces;

namespace QuizMaster.API.Controllers
{
    [Authorize]
    public class QuizController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IOpenTriviaService _triviaService;

        public QuizController(IMediator mediator, IOpenTriviaService triviaService)
        {
            _mediator = mediator;
            _triviaService = triviaService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizDto dto)
        {
            var command = new CreateQuizCommand
            {
                UserId = GetCurrentUserId(),
                Category = dto.Category,
                Difficulty = dto.Difficulty,
                NumberOfQuestions = dto.NumberOfQuestions
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("submit-answer")]
        public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerDto dto)
        {
            var command = new SubmitAnswerCommand
            {
                UserId = GetCurrentUserId(),
                QuizId = dto.QuizId,
                QuestionId = dto.QuestionId,
                UserAnswer = dto.UserAnswer,
                TimeSpent = dto.TimeSpent
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("{quizId}/complete")]
        public async Task<IActionResult> CompleteQuiz(Guid quizId)
        {
            var command = new CompleteQuizCommand
            {
                UserId = GetCurrentUserId(),
                QuizId = quizId
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("my-quizzes")]
        public async Task<IActionResult> GetMyQuizzes(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetUserQuizzesQuery
            {
                UserId = GetCurrentUserId(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuizById(Guid quizId)
        {
            var query = new GetQuizByIdQuery
            {
                QuizId = quizId,
                UserId = GetCurrentUserId()
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommendedQuestions(
            [FromQuery] string? category = null,
            [FromQuery] string? difficulty = null,
            [FromQuery] int count = 10)
        {
            var query = new GetRecommendedQuestionsQuery
            {
                UserId = GetCurrentUserId(),
                Category = category,
                Difficulty = difficulty,
                Count = count
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuiz(Guid quizId)
        {
            var command = new DeleteQuizCommand
            {
                UserId = GetCurrentUserId(),
                QuizId = quizId
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        
       [HttpGet("categories")]
       [AllowAnonymous]
       public async Task<IActionResult> GetCategories()
       {
         var categories = await _triviaService.GetCategoriesAsync();
         return Ok(ApiResponse<List<string>>.SuccessResult(categories, "Categories retrieved successfully"));
       }
    }
}
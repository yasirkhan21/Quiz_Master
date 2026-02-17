using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizMaster.Application.Features.Stats.Queries;
using System.Threading.Tasks;

namespace QuizMaster.API.Controllers
{
    [Authorize]
    public class StatsController : BaseController
    {
        private readonly IMediator _mediator;

        public StatsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var query = new GetDashboardStatsQuery
            {
                UserId = GetCurrentUserId()
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
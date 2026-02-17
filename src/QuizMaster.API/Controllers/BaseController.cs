using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace QuizMaster.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not authenticated");

            return Guid.Parse(userIdClaim);
        }

        protected string GetCurrentUsername()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }
    }
}
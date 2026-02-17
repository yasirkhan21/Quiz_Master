using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;

namespace QuizMaster.Application.Features.Auth.Commands.LoginUser
{
    public class LoginUserCommand : IRequest<ApiResponse<AuthResponseDto>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
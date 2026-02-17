using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;

namespace QuizMaster.Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommand : IRequest<ApiResponse<AuthResponseDto>>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
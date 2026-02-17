using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using System;

namespace QuizMaster.Application.Features.Auth.Commands.LoginUser
{
    public class GetCurrentUserQuery : IRequest<ApiResponse<UserDto>>
    {
        public Guid UserId { get; set; }
    }
}
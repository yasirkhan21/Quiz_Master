using AutoMapper;
using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using QuizMaster.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster.Application.Features.Auth.Commands.LoginUser
{
    public class LoginUserCommandHandler 
        : IRequestHandler<LoginUserCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public LoginUserCommandHandler(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(
            LoginUserCommand request, 
            CancellationToken cancellationToken)
        {
            // Find user by email
            var users = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
            var user = users.FirstOrDefault();

            if (user == null)
            {
                return ApiResponse<AuthResponseDto>.FailureResult(
                    "Invalid credentials",
                    new List<string> { "Email or password is incorrect" });
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponseDto>.FailureResult(
                    "Invalid credentials",
                    new List<string> { "Email or password is incorrect" });
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate token
            var token = _tokenService.GenerateToken(user);
            var userDto = _mapper.Map<UserDto>(user);

            var authResponse = new AuthResponseDto
            {
                Token = token,
                User = userDto,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };

            return ApiResponse<AuthResponseDto>.SuccessResult(
                authResponse, 
                "Login successful");
        }
    }
}
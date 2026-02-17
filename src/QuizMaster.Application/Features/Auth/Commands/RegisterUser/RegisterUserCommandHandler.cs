using AutoMapper;
using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using QuizMaster.Core.Entities;
using QuizMaster.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using BCrypt.Net;

namespace QuizMaster.Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandHandler 
        : IRequestHandler<RegisterUserCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(
            RegisterUserCommand request, 
            CancellationToken cancellationToken)
        {
            // Validate passwords match
            if (request.Password != request.ConfirmPassword)
            {
                return ApiResponse<AuthResponseDto>.FailureResult(
                    "Passwords do not match",
                    new List<string> { "Password and Confirm Password must match" });
            }

            // Check if email already exists
            var existingUser = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
            if (existingUser.Any())
            {
                return ApiResponse<AuthResponseDto>.FailureResult(
                    "Email already registered",
                    new List<string> { "A user with this email already exists" });
            }

            // Check if username already exists
            var existingUsername = await _unitOfWork.Users.FindAsync(u => u.Username == request.Username);
            if (existingUsername.Any())
            {
                return ApiResponse<AuthResponseDto>.FailureResult(
                    "Username already taken",
                    new List<string> { "This username is already in use" });
            }

            // Create new user
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
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
                "User registered successfully");
        }
    }
}
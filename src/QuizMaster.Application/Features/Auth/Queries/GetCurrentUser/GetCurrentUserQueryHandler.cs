using AutoMapper;
using MediatR;
using QuizMaster.Application.Common;
using QuizMaster.Application.DTOs;
using QuizMaster.Application.Features.Auth.Commands.LoginUser;
using QuizMaster.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster.Application.Features.Auth.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler
        : IRequestHandler<GetCurrentUserQuery, ApiResponse<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<UserDto>> Handle(
            GetCurrentUserQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);

            if (user == null)
                return ApiResponse<UserDto>.FailureResult("User not found");

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponse<UserDto>.SuccessResult(userDto);
        }
    }
}
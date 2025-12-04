using MediatR;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Exceptions;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IAccountApiService _accountApiService;

        public LoginCommandHandler(IAccountApiService accountApiService)
        {
            _accountApiService = accountApiService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ApiException("Usuario y contraseña son requeridos.", 400);
            }
            var result = await _accountApiService.LoginAsync(request.UserName, request.Password);

            return result;
        }
    }
}

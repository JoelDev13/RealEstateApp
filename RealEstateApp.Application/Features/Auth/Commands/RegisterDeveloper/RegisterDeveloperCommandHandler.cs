using MediatR;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Exceptions;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Features.Auth.Commands.RegisterDeveloper
{
    public class RegisterDeveloperCommandHandler : IRequestHandler<RegisterDeveloperCommand, string>
    {
        private readonly IAccountApiService _accountApiService;

        public RegisterDeveloperCommandHandler(IAccountApiService accountApiService)
        {
            _accountApiService = accountApiService;
        }

        public async Task<string> Handle(RegisterDeveloperCommand request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ConfirmPassword)
            {
                throw new ApiException("Las contraseñas no coinciden.", 400);
            }

            var dto = new RegisterDto
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                UserType = "Desarrollador"
            };

            var userId = await _accountApiService.RegisterDeveloperAsync(dto);

            return userId;
        }
    }
}

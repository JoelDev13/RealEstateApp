using MediatR;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Exceptions;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Features.Auth.Commands.RegisterAdmin
{
    public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, string>
    {
        private readonly IAccountApiService _accountApiService;

        public RegisterAdminCommandHandler(IAccountApiService accountApiService)
        {
            _accountApiService = accountApiService;
        }

        public async Task<string> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentAdminId))
            {
                throw new ApiException("Debe existir un administrador autenticado para realizar esta acción.", 401);
            }

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
                UserType = "Administrator"
            };

            var userId = await _accountApiService.RegisterAdminAsync(dto, request.CurrentAdminId);

            return userId;
        }
    }
}

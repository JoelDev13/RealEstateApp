using MediatR;
using RealEstateApp.Application.Dtos.Auth;

namespace RealEstateApp.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

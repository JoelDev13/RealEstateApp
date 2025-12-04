using MediatR;

namespace RealEstateApp.Application.Features.Auth.Commands.RegisterAdmin
{
    public class RegisterAdminCommand : IRequest<string>
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string CurrentAdminId { get; set; } = string.Empty;
    }
}

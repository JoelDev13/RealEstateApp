using FluentValidation;

namespace RealEstateApp.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es requerido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.");
        }
    }
}

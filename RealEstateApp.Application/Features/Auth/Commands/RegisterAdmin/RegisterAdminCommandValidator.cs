using FluentValidation;

namespace RealEstateApp.Application.Features.Auth.Commands.RegisterAdmin
{
    public class RegisterAdminCommandValidator : AbstractValidator<RegisterAdminCommand>
    {
        public RegisterAdminCommandValidator()
        {
            RuleFor(x => x.CurrentAdminId)
                .NotEmpty().WithMessage("Debe haber un administrador autenticado.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es requerido.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es requerido.")
                .EmailAddress().WithMessage("El correo no tiene un formato válido.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es requerido.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es requerido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Debe confirmar la contraseña.")
                .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden.");
        }
    }
}

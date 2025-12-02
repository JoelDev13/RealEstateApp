using FluentValidation;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.UpdatePropertyType
{
    public class UpdatePropertyTypeCommandValidator : AbstractValidator<UpdatePropertyTypeCommand>
    {
        public UpdatePropertyTypeCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El Id debe ser mayor que cero.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}

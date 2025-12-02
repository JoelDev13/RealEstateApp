using FluentValidation;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.CreatePropertyType
{
    public class CreatePropertyTypeCommandValidator : AbstractValidator<CreatePropertyTypeCommand>
    {
        public CreatePropertyTypeCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}

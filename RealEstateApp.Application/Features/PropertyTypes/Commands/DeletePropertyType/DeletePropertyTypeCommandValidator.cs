using FluentValidation;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.DeletePropertyType
{
    public class DeletePropertyTypeCommandValidator : AbstractValidator<DeletePropertyTypeCommand>
    {
        public DeletePropertyTypeCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El Id debe ser mayor que cero.");
        }
    }
}

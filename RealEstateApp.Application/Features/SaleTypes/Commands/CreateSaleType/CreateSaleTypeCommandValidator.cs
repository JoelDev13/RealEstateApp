using FluentValidation;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.CreateSaleType
{
    public class CreateSaleTypeCommandValidator : AbstractValidator<CreateSaleTypeCommand>
    {
        public CreateSaleTypeCommandValidator()
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

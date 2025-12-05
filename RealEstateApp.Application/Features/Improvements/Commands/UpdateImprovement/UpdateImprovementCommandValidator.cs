using FluentValidation;

namespace RealEstateApp.Application.Features.Improvements.Commands.UpdateImprovement
{
    public class UpdateImprovementCommandValidator : AbstractValidator<UpdateImprovementCommand>
    {
        public UpdateImprovementCommandValidator()
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

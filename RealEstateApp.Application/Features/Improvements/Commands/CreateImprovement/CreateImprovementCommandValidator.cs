using FluentValidation;

namespace RealEstateApp.Application.Features.Improvements.Commands.CreateImprovement
{
    public class CreateImprovementCommandValidator : AbstractValidator<CreateImprovementCommand>
    {
        public CreateImprovementCommandValidator()
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

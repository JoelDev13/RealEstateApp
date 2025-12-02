using FluentValidation.Results;

namespace RealEstateApp.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> Errors { get; } = new();

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : base("One or more validation errors occurred.")
        {
            Errors = failures
                .GroupBy(f => f.PropertyName)
                .Select(g => $"{g.Key}: {string.Join(", ", g.Select(f => f.ErrorMessage))}")
                .ToList();
        }
    }
}

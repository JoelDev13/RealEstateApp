using FluentValidation.TestHelper;
using RealEstateApp.Application.Features.Improvements.Commands.CreateImprovement;

namespace RealEstateApp.Unit.Tests.Improvements.Commands;

public class CreateImprovementCommandValidatorTests
{
    private readonly CreateImprovementCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var cmd = new CreateImprovementCommand { Name = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Too_Long()
    {
        var cmd = new CreateImprovementCommand { Name = new string('a', 101) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Too_Long()
    {
        var cmd = new CreateImprovementCommand { Name = "Piscina", Description = new string('b', 501) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var cmd = new CreateImprovementCommand
        {
            Name = "Piscina",
            Description = "Incluye jacuzzi"
        };

        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

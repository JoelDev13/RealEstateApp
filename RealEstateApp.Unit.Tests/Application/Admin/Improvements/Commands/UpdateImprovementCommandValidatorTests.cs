using FluentValidation.TestHelper;
using RealEstateApp.Application.Features.Improvements.Commands.UpdateImprovement;

namespace RealEstateApp.Unit.Tests.Improvements.Commands;

public class UpdateImprovementCommandValidatorTests
{
    private readonly UpdateImprovementCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Id_Invalid()
    {
        var cmd = new UpdateImprovementCommand { Id = 0 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Empty()
    {
        var cmd = new UpdateImprovementCommand { Id = 1, Name = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Too_Long()
    {
        var cmd = new UpdateImprovementCommand { Id = 1, Name = new string('a', 101) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Too_Long()
    {
        var cmd = new UpdateImprovementCommand { Id = 1, Name = "Jacuzzi", Description = new string('b', 501) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var cmd = new UpdateImprovementCommand
        {
            Id = 1,
            Name = "Piscina Deluxe",
            Description = "Válido"
        };

        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

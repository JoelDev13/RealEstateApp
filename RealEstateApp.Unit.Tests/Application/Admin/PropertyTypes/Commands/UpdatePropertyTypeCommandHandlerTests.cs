using FluentValidation.TestHelper;
using RealEstateApp.Application.Features.PropertyTypes.Commands.UpdatePropertyType;

namespace RealEstateApp.Unit.Tests.PropertyTypes.Commands;

public class UpdatePropertyTypeCommandValidatorTests
{
    private readonly UpdatePropertyTypeCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Id_Is_Invalid()
    {
        var command = new UpdatePropertyTypeCommand { Id = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new UpdatePropertyTypeCommand
        {
            Id = 1,
            Name = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Too_Long()
    {
        var command = new UpdatePropertyTypeCommand
        {
            Id = 1,
            Name = new string('a', 101)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Too_Long()
    {
        var command = new UpdatePropertyTypeCommand
        {
            Id = 1,
            Name = "Casa",
            Description = new string('b', 501)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var command = new UpdatePropertyTypeCommand
        {
            Id = 1,
            Name = "Apartamento",
            Description = "Algo válido",
            IsActive = true
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}

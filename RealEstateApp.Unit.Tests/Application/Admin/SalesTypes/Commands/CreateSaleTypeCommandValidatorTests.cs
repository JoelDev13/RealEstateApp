using FluentValidation.TestHelper;
using RealEstateApp.Application.Features.SaleTypes.Commands.CreateSaleType;

namespace RealEstateApp.Unit.Tests.SaleTypes.Commands;

public class CreateSaleTypeCommandValidatorTests
{
    private readonly CreateSaleTypeCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var cmd = new CreateSaleTypeCommand { Name = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Too_Long()
    {
        var cmd = new CreateSaleTypeCommand { Name = new string('a', 101) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Too_Long()
    {
        var cmd = new CreateSaleTypeCommand { Name = "Test", Description = new string('b', 501) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var cmd = new CreateSaleTypeCommand
        {
            Name = "Directa",
            Description = "Válido"
        };

        var result = _validator.TestValidate(cmd);

        result.ShouldNotHaveAnyValidationErrors();
    }
}

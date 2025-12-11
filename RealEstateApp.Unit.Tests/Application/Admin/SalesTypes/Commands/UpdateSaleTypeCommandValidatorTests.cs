using FluentValidation.TestHelper;
using RealEstateApp.Application.Features.SaleTypes.Commands.UpdateSaleType;

namespace RealEstateApp.Unit.Tests.SaleTypes.Commands;

public class UpdateSaleTypeCommandValidatorTests
{
    private readonly UpdateSaleTypeCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Id_Invalid()
    {
        var cmd = new UpdateSaleTypeCommand { Id = 0 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Empty()
    {
        var cmd = new UpdateSaleTypeCommand { Id = 1, Name = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Too_Long()
    {
        var cmd = new UpdateSaleTypeCommand { Id = 1, Name = new string('a', 101) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Too_Long()
    {
        var cmd = new UpdateSaleTypeCommand { Id = 1, Name = "Test", Description = new string('b', 501) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var cmd = new UpdateSaleTypeCommand
        {
            Id = 1,
            Name = "Financiado",
            Description = "Válido"
        };

        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

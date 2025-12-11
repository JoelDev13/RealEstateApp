using FluentValidation.TestHelper;
using RealEstateApp.Application.Features.PropertyTypes.Commands.CreatePropertyType;

namespace RealEstateApp.Unit.Tests.Application.Admin.PropertyTypes.Commands
{
    public class CreatePropertyTypeCommandValidatorTests
    {
        private readonly CreatePropertyTypeCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var command = new CreatePropertyTypeCommand
            {
                Name = string.Empty,
                Description = "Alguna descripción"
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Null()
        {
            var command = new CreatePropertyTypeCommand
            {
                Name = null!,
                Description = "Alguna descripción"
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Long()
        {
            var command = new CreatePropertyTypeCommand
            {
                Name = new string('a', 101)
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Too_Long()
        {
            var command = new CreatePropertyTypeCommand
            {
                Name = "Casa",
                Description = new string('b', 501)
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var command = new CreatePropertyTypeCommand
            {
                Name = "Casa",
                Description = "Descripción válida",
                IsActive = true
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

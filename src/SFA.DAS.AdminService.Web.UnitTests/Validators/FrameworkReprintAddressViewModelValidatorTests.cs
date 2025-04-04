using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.Search;

namespace SFA.DAS.AdminService.Web.Tests.Validators
{
     [TestFixture]
    public class FrameworkReprintAddressViewModelValidatorTests
    {
        private readonly FrameworkReprintAddressViewModelValidator _validator;
        private readonly string invalidCharacters = @"@#$^=+\\/<%>%";

        public FrameworkReprintAddressViewModelValidatorTests()
        {
            _validator = new FrameworkReprintAddressViewModelValidator();
        }

        [Test]
        public void ValidModel_ShouldNotHaveErrors()
        {
            // Arrange
            var model = new FrameworkReprintAddressViewModel 
            {
                AddressLine1 = "123 Main St",
                TownOrCity = "Anytown",
                Postcode = "AA1 1AA",
                AddressLine2 = "Apt 1",
                County = "AnyCounty"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void EmptyAddressLine1_ShouldHaveError()
        {
            // Arrange
            var model = new FrameworkReprintAddressViewModel 
            {
                AddressLine1 = string.Empty,
                TownOrCity = "Anytown",
                Postcode = "AA1 1AA",
                AddressLine2 = "Apt 1",
                County = "AnyCounty"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AddressLine1)
                .WithErrorMessage("Enter address line 1, typically the building and street");
        }

        [Test]
        public void InvalidCharactersInAddressLine1_ShouldHaveError()
        {
            // Arrange
            var model = new FrameworkReprintAddressViewModel { AddressLine1 = "Anytown@" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AddressLine1)
                .WithErrorMessage($"Address line 1 must not include any special characters: {invalidCharacters}");
        }

        [Test]
        public void InvalidCharactersInAddressLine2_ShouldHaveError()
        {
            // Arrange
            var model = new FrameworkReprintAddressViewModel { AddressLine2 = "Anytown$" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AddressLine2)
                .WithErrorMessage($"Address line must not include any special characters: {invalidCharacters}");
        }

        [Test]
        public void EmptyTownOrCity_ShouldHaveError()
        {
            // Arrange
            var model = new FrameworkReprintAddressViewModel 
            {
                AddressLine1 = "123 Main St",
                TownOrCity = "",
                Postcode = "AA1 1AA",
                AddressLine2 = "Apt 1",
                County = "AnyCounty"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TownOrCity)
                .WithErrorMessage("Enter town or city");
        }

        [Test]
        public void InvalidCharactersInTownOrCity_ShouldHaveError()
        {
            // Arrange
            var model = new FrameworkReprintAddressViewModel { TownOrCity = "Anytown^" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TownOrCity)
                .WithErrorMessage($"Town or city must not include any special characters: {invalidCharacters}");
        }

        [Test]
        public void InvalidCharactersInCounty_ShouldHaveError()
        {
            // Arrange
            var model = new FrameworkReprintAddressViewModel { County = "Anytown<^>" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.County)
                .WithErrorMessage($"County must not include any special characters: {invalidCharacters}");
        }

        [Test]
        public void EmptyPostcode_ShouldHaveError()
        {
            // Arrange
            var model = new FrameworkReprintAddressViewModel 
            {
                AddressLine1 = "123 Main St",
                TownOrCity = "Anytown",
                Postcode = "",
                AddressLine2 = "Apt 1",
                County = "AnyCounty"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Postcode)
                .WithErrorMessage("Enter postcode");
        }

        [Test]
        public void InvalidCharactersInPostcode_ShouldHaveError()
        {
            // Arrange
            var model = new FrameworkReprintAddressViewModel { Postcode = "Anytown<^>" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Postcode)
                .WithErrorMessage($"Postcode must not include any special characters: {invalidCharacters}");
        }

        [Test]
        public void InvalidPostcodeFormat_ShouldHaveError()
        {
            // Arrange
            var model = new FrameworkReprintAddressViewModel { Postcode = "InvalidPostcode" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Postcode)
                .WithErrorMessage("Enter a valid postcode");
        }

    }



}




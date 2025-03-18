using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Validators;
using System;

namespace SFA.DAS.AdminService.Web.UnitTests.Validators
{
    [TestFixture]
    public class SearchInputViewModelValidatorTests
    {
        private SearchInputViewModelValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new SearchInputViewModelValidator();
        }

        [Test]
        public void StandardsSearch_SearchStringNullOrEmpty_HasError()
        {
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Standards, SearchString = null };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.SearchString)
                .WithErrorMessage("Enter name or number");
        }

        [Test]
        public void StandardsSearch_SearchStringTooShort_HasError()
        {
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Standards, SearchString = "a" };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.SearchString)
                .WithErrorMessage("Name or number must be at least 2 characters.");
        }

        [Test]
        public void StandardsSearch_SearchStringValid_NoError()
        {
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Standards, SearchString = "test" };
            var result = _validator.TestValidate(vm);
            result.ShouldNotHaveValidationErrorFor(x => x.SearchString);
        }

        [Test]
        public void FrameworksSearch_FirstNameNullOrEmpty_HasError()
        {
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, FirstName = null };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                .WithErrorMessage("Enter first name");
        }

        [Test]
        public void FrameworksSearch_FirstNameTooShort_HasError()
        {
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, FirstName = "a" };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                .WithErrorMessage("First name must be at least 2 characters.");
        }


        [Test]
        public void FrameworksSearch_LastNameNullOrEmpty_HasError()
        {
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, LastName = null };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.LastName)
                .WithErrorMessage("Enter last name");
        }

        [Test]
        public void FrameworksSearch_LastNameTooShort_HasError()
        {
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, LastName = "a" };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.LastName)
                .WithErrorMessage("Last name must be at least 2 characters.");
        }

        [Test]
        public void FrameworksSearch_BlankDate_HasError()
        {
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, Day = "", Month = "", Year = "" };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("Enter a date of birth");


            vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, Day = null, Month = null, Year = null };
            result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("Enter a date of birth");
        }

        [Test]
        public void FrameworksSearch_InvalidDate_HasError()
        {
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, Day = "32", Month = "1", Year = "2023" }; 
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("The date must be a real date");

            vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, Day = "1", Month = "13", Year = "2023" }; 
            result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("The date must be a real date");


            vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, Day = "1", Month = "1", Year = "2023a" }; 
            result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("The date must be a real date");

            vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, Day = "0", Month = "0", Year = "0" };
            result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("The date must be a real date");

            vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, Day = "1", Month = "1", Year = "1001" };
            result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("Check the year of your date of birth");


        }

        [Test]
        public void FrameworksSearch_FutureDate_HasError()
        {
            var futureDate = DateTime.Now.AddDays(1);
            var vm = new SearchInputViewModel
            {
                SearchType = SearchTypes.Frameworks,
                Day = futureDate.Day.ToString(),
                Month = futureDate.Month.ToString(),
                Year = futureDate.Year.ToString()
            };

            var result = _validator.TestValidate(vm);

            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("The date of birth must be in the past");
        }

        [Test]
        public void FrameworksSearch_ValidInput_NoError()
        {
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks, FirstName = "Test", LastName = "User", Day = "1", Month = "1", Year = "2000" };
            var result = _validator.TestValidate(vm);
            result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
            result.ShouldNotHaveValidationErrorFor(x => x.LastName);
            result.ShouldNotHaveValidationErrorFor(x => x.Date);
        }
    }
}
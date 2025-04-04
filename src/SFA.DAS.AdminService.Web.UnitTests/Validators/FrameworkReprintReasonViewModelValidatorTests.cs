using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.UnitTests.Validators
{
    [TestFixture]
    public class FrameworkReprintReasonViewModelValidatorTests
    {
        private FrameworkReprintReasonViewModelValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new FrameworkReprintReasonViewModelValidator();
        }

        [Test]
        public void FrameworkReprintReasonViewModel_ValidModel_NoErrors()
        {
            var vm = new FrameworkReprintReasonViewModel
            {
                TicketNumber = "ABC12345",
                SelectedReprintReasons = new List<string> { "Other", "A", "B" },
                OtherReason = "Reason text"
            };
            var result = _validator.TestValidate(vm);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void FrameworkReprintReasonViewModel_TicketNumberNullOrEmpty_HasError(string ticketNumber)
        {
            var vm = new FrameworkReprintReasonViewModel
            {
                TicketNumber = ticketNumber,
                SelectedReprintReasons = new List<string> { "Other", "A", "B" },
                OtherReason = "Reason text"
            };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.TicketNumber)
                .WithErrorMessage("Enter the ticket reference number");
        }

        [Test]
        public void FrameworkReprintReasonViewModel_TicketNumberExceedsMaxCharacters_HasError()
        {
            var vm = new FrameworkReprintReasonViewModel
            {
                TicketNumber = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789",
                SelectedReprintReasons = new List<string> { "Other", "A", "B" },
                OtherReason = "Reason text"
            };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.TicketNumber)
                .WithErrorMessage("Ticket reference number must be 20 characters or fewer");
        }

        [Test]
        [TestCase("ABC123!")]
        [TestCase("ABC123£")]
        [TestCase("ABC123$")]
        [TestCase("ABC123%")]
        [TestCase("ABC123^")]
        [TestCase("ABC123&")]
        [TestCase("ABC123*")]
        public void FrameworkReprintReasonViewModel_TicketNumberContainsNonAlphanumericCharacters_HasError(string ticketNumber)
        {
            var vm = new FrameworkReprintReasonViewModel
            {
                TicketNumber = ticketNumber,
                SelectedReprintReasons = new List<string> { "Other", "A", "B" },
                OtherReason = "Reason text"
            };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.TicketNumber)
                .WithErrorMessage("Enter only alphanumeric characters");
        }

        [Test]
        public void FrameworkReprintReasonViewModel_SelectedReprintReasonsEmpty_HasError()
        {
            var vm = new FrameworkReprintReasonViewModel
            {
                TicketNumber = "ABC1234",
                SelectedReprintReasons = new List<string>(),
                OtherReason = "Reason text"
            };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.SelectedReprintReasons)
                .WithErrorMessage("Select reasons for requesting a reprint");
        }

        [Test]
        public void FrameworkReprintReasonViewModel_SelectedReprintReasonsNull_HasError()
        {
            var vm = new FrameworkReprintReasonViewModel
            {
                TicketNumber = "ABC1234",
                SelectedReprintReasons = null,
                OtherReason = "Reason text"
            };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.SelectedReprintReasons)
                .WithErrorMessage("Select reasons for requesting a reprint");
        }

        [Test]
        public void FrameworkReprintReasonViewModel_Other_OtherReasonEmpty_HasError()
        {
            var vm = new FrameworkReprintReasonViewModel
            {
                TicketNumber = "ABC1234",
                SelectedReprintReasons = new List<string> { "Other"},
                OtherReason = ""
            };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.OtherReason)
                .WithErrorMessage("Give details");
        }

        [Test]
        public void FrameworkReprintReasonViewModel_Other_OtherReasonNull_HasError()
        {
            var vm = new FrameworkReprintReasonViewModel
            {
                TicketNumber = "ABC1234",
                SelectedReprintReasons = new List<string> { "Other"},
                OtherReason = null
            };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.OtherReason)
                .WithErrorMessage("Give details");
        }

        [Test]
        public void FrameworkReprintReasonViewModel_Other_OtherReasonExceedsMaxCharacters_HasError()
        {
            var vm = new FrameworkReprintReasonViewModel
            {
                TicketNumber = "ABC1234",
                SelectedReprintReasons = new List<string> { "Other"},
                OtherReason = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."
            };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.OtherReason)
                .WithErrorMessage("Details must be 200 characters or fewer");
        }
    }
}
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Models.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using System;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenPostingSetSecondaryEpaoEffectiveToDate : MergeControllerTestBase
    {
        [Test]
        public void And_ModelIsValid_Then_CallUpdateWithNewEffectiveToDate()
        {
            var dateTime = DateTime.Now;

            var expectedDay = dateTime.Day;
            var expectedMonth = dateTime.Month;
            var expectedYear = dateTime.Year;

            var viewModel = SetUpViewModel(dateTime);

            var result = MergeController.SetSecondaryEpaoEffectiveToDate(viewModel);

            _mockMergeSessionService.Verify(ms => ms.UpdateMergeRequest(It.Is<MergeRequest>(
                r => r.SecondaryEpaoEffectiveTo.Value == dateTime.Date)), Times.Once());
        }

        [Test]
        public void And_ModelIsValid_Then_ReturnRedirectToMergeOverview()
        {
            var dateTime = _autoFixture.Build<DateTime>().Create();

            var viewModel = SetUpViewModel(dateTime);

            var result = MergeController.SetSecondaryEpaoEffectiveToDate(viewModel) as RedirectToActionResult;

            result.ActionName.Should().Be("MergeOverview");
        }

        [Test]
        public void And_ModelIsInvalid_Then_ReturnView()
        {
            MergeController.ModelState.AddModelError("Error", "Error message");

            var viewModel = _autoFixture.Build<SetSecondaryEpaoEffectiveToDateViewModel>().Create();

            var result = MergeController.SetSecondaryEpaoEffectiveToDate(viewModel) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }

        private static SetSecondaryEpaoEffectiveToDateViewModel SetUpViewModel(DateTime? date)
        {
            return new SetSecondaryEpaoEffectiveToDateViewModel
            {
                Day = date?.Day.ToString(),
                Month = date?.Month.ToString(),
                Year = date?.Year.ToString()
            };
        }
    }
}

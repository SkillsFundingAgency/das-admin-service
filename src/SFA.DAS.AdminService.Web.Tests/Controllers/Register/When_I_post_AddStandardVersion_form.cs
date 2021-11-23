using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class When_I_post_AddStandardVersion_form : RegisterAddStandardBase
    {
        private RegisterAddStandardVersionViewModel _viewModel;

        [SetUp]
        public void Arrange()
        {
            _viewModel = new RegisterAddStandardVersionViewModel();

            Sut = new RegisterController(ControllerSession.Object, ApiClient.Object, ApplyApiClient.Object, ContactsApiClient.Object, StandardServiceClient.Object, Env.Object);

            SetUpAddStandardViewModelWithStandard();
        }

        [Test]
        public void And_TheFormIsValid_Then_RedirectToAddStandardVersion()
        {
            var date = DateTime.UtcNow;

            _viewModel = Fixture.Build<RegisterAddStandardVersionViewModel>()
                .With(x => x.EffectiveFromDay, date.Day.ToString())
                .With(x => x.EffectiveFromMonth, date.Month.ToString())
                .With(x => x.EffectiveFromYear, "2020")
                .Create();

            var redirectResult = Sut.AddOrganisationStandardVersion(_viewModel) as RedirectToActionResult;

            redirectResult.ActionName.Should().Be("AddOrganisationStandard");
            redirectResult.ControllerName.Should().Be("Register");
            redirectResult.RouteValues["organisationId"].Should().Be(_viewModel.OrganisationId);
            redirectResult.RouteValues["standardId"].Should().Be(_viewModel.IfateReferenceNumber);
        }

        [Test]
        public void And_FormIsInvalid_Then_ReturnView()
        {
            Sut.ModelState.AddModelError("Error", "Error");

            var result = Sut.AddOrganisationStandardVersion(_viewModel) as ViewResult;

            result.Should().BeOfType<ViewResult>();
            result.ViewData.ModelState.IsValid.Should().BeFalse();
        }
    }
}

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class When_I_post_AddStandard_form : RegisterAddStandardBase
    {
        [SetUp]
        public void Arrange()
        {
            Sut = new RegisterController(ControllerSession.Object, ApiClient.Object, ApplyApiClient.Object, ContactsApiClient.Object, StandardServiceClient.Object, Env.Object);

            SetUpAddStandardViewModelWithStandard();
        }

        [Test]
        public async Task And_FormIsValid_Then_CreateEpaOrganisationStandardWithVersionsThatAreAdded()
        {
            AddOrganisationStandardViewModel.Versions[0].EffectiveFrom = DateTime.UtcNow;

            await Sut.AddOrganisationStandard(AddOrganisationStandardViewModel);

            ApiClient.Verify(c => c.CreateEpaOrganisationStandard(It.Is<CreateEpaOrganisationStandardRequest>(
                req => req.StandardVersions.Where(x => x.Version == "1.0").Count() == 1
                    && req.StandardVersions.Where(x => x.Version == "2.0").Count() == 0
                )), Times.Once());
        }

        [Test, MoqAutoData]
        public async Task And_FormIsValid_Then_ReturnRedirectToViewStandard(string orgStandardId)
        {
            SetupCreateEpaOrganisationStandard(orgStandardId);

            var redirectResult = await Sut.AddOrganisationStandard(AddOrganisationStandardViewModel) as RedirectToActionResult;

            redirectResult.ActionName.Should().Be("ViewStandard");
            redirectResult.ControllerName.Should().Be("Register");
            redirectResult.RouteValues["organisationStandardId"].Should().Be(orgStandardId);
        }

        [Test]
        public async Task And_FormIsValid_Then_RemoveAddStandardViewModel()
        {
            await Sut.AddOrganisationStandard(AddOrganisationStandardViewModel);

            ControllerSession.Verify(s => s.Remove("AddOrganisationStandardViewModel"));
        }

        [Test]
        public async Task And_FormIsInvalid_Then_ReturnView()
        {
            Sut.ModelState.AddModelError("Error", "Error");

            var result = await Sut.AddOrganisationStandard(AddOrganisationStandardViewModel) as ViewResult;

            result.Should().BeOfType<ViewResult>();
            result.ViewData.ModelState.IsValid.Should().BeFalse();
        }
    }
}

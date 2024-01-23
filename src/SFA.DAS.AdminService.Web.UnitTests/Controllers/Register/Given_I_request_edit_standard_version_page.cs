using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class Given_I_request_edit_standard_version_page : RegisterBase
    {
        private OrganisationStandard _orgStandardResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _orgStandardResponse = fixture.Create<OrganisationStandard>();
            _orgStandardResponse.Versions.First().Version = "1.0";

            RegisterApiClient.Setup(c => c.GetOrganisationStandard(It.IsAny<int>()))
                     .ReturnsAsync(_orgStandardResponse);

            Sut = new RegisterController(ControllerSession.Object, RegisterApiClient.Object, ApplicationApiClient.Object, OrganisationsApiClient.Object, ContactsApiClient.Object, Env.Object);
        }

        [Test]
        public async Task Then_correct_standard_version_is_mapped()
        {
            var result = await Sut.EditStandardVersion(1, "1.0") as ViewResult;

            var model = result.Model as RegisterEditOrganisationStandardVersionViewModel;

            model.Version.Should().Be("1.0");
        }

        [Test]
        public async Task And_submit_valid_updated_dates_Then_return_redirect_to_view_organisation_standard()
        {
            OrganisationsApiClient.Setup(client => client.UpdateEpaOrganisationStandardVersion(It.IsAny<UpdateOrganisationStandardVersionRequest>()))
                .ReturnsAsync("OK");
            var model = new RegisterEditOrganisationStandardVersionViewModel();

            var result = await Sut.EditStandardVersion(1, "1.0", model) as RedirectToActionResult;

            result.ControllerName.Should().Be("Register");
            result.ActionName.Should().Be("ViewStandard");
        }
    }
}

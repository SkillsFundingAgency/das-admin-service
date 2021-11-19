using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class When_I_request_CancelAddStandard : RegisterAddStandardBase
    {
        [SetUp]
        public void Arrange()
        {
            Sut = new RegisterController(ControllerSession.Object, ApiClient.Object, ApplyApiClient.Object, ContactsApiClient.Object, Env.Object);
        }

        [Test, MoqAutoData]
        public void Then_ViewModelIsRemovedFromSession(string organisationId)
        {
            Sut.CancelAddStandard(organisationId);

            ControllerSession.Verify(s => s.Remove("AddOrganisationStandardViewModel"), Times.Once());
        }

        [Test, MoqAutoData]
        public void Then_RedirectToStandardSearchPage(string organisationId)
        {
            var redirectResult = Sut.CancelAddStandard(organisationId) as RedirectToActionResult;

            redirectResult.ActionName.Should().Be("SearchStandards");
            redirectResult.ControllerName.Should().Be("Register");
            redirectResult.RouteValues["organisationId"].Should().Be(organisationId);
        }
    }
}

using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class When_I_request_AddStandardVersion_page : RegisterAddStandardBase
    {
        [SetUp]
        public void Arrange()
        {
            Sut = new RegisterController(ControllerSession.Object, ApiClient.Object, ApplyApiClient.Object, ContactsApiClient.Object, Env.Object);
        }

        [Test, MoqAutoData]
        public void And_AddStandardViewModelIsEmpty_Then_RedirectToAddStandardPage(string ifateReferenceNumber, string version)
        {
            ControllerSession.Setup(s => s.AddOrganisationStandardViewModel).Returns((RegisterAddOrganisationStandardViewModel)null);
            
            var redirectResult = Sut.AddOrganisationStandardVersion(OrganisationOneOrganisationId, ifateReferenceNumber, version) as RedirectToActionResult;

            redirectResult.ActionName.Should().Be("AddOrganisationStandard");
            redirectResult.ControllerName.Should().Be("Register");
            redirectResult.RouteValues["organisationId"].Should().Be(OrganisationOneOrganisationId);
            redirectResult.RouteValues["ifateReferenceNumber"].Should().Be(ifateReferenceNumber);
        }

        [Test]
        public void And_ViewModelIsPopulated_Then_ViewModelIsMapped()
        {
            SetUpAddStandardViewModelWithStandard();
           
            var viewResult = Sut.AddOrganisationStandardVersion(OrganisationOneOrganisationId, StandardVersion1.IFateReferenceNumber, StandardVersion1.Version) as ViewResult;

            var model = viewResult.Model as RegisterAddStandardVersionViewModel;

            model.OrganisationId.Should().Be(OrganisationOneOrganisationId);
            model.Title.Should().Be(StandardVersion1.Title);
        }
    }
}

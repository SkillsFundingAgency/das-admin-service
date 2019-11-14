using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class Given_I_request_view_organisation_page : RegisterBase
    {
        private RegisterViewAndEditOrganisationViewModel _viewModelResponse;

        [SetUp]
        public async Task Arrange()
        {
            Sut = new RegisterController(ControllerSession.Object, ApiClient.Object, ApplyApiClient.Object, ContactsApiClient.Object, StandardServiceClient.Object, Env.Object);

            var actionResult = await Sut.ViewOrganisation(OrganisationOneOrganisationId);
            var result = actionResult as ViewResult;

            _viewModelResponse = result.Model as RegisterViewAndEditOrganisationViewModel;
        }

        [Test]
        public void Should_show_users_seperate_from_contacts()
        {
            _viewModelResponse.Id.Should().Be(OrganisationOneId);

            _viewModelResponse.Users.Should().HaveCount(3);
            _viewModelResponse.Users.Should().Contain(p => p.Contact.Id == UserOneId);
            _viewModelResponse.Users.Should().Contain(p => p.Contact.Id == UserTwoId);
            _viewModelResponse.Users.Should().Contain(p => p.Contact.Id == UserThreeId);

            _viewModelResponse.Contacts.Should().HaveCount(2);
            _viewModelResponse.Contacts.Should().Contain(p => p.Id == ContactOneId);
            _viewModelResponse.Contacts.Should().Contain(p => p.Id == ContactTwoId);
        }
    }
}

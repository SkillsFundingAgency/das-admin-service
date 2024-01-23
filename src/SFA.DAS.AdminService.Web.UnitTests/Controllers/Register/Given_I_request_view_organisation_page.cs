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
            Sut = new RegisterController(ControllerSession.Object, RegisterApiClient.Object, ApplicationApiClient.Object, OrganisationsApiClient.Object, ContactsApiClient.Object, Env.Object);

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

        [Test]
        public void Should_have_first_page_of_organisation_standards()
        {
            _viewModelResponse.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.PaginationViewModel.PaginatedList.PageIndex.Should().Be(1);
        }

        [Test]
        public void Should_display_10_items_per_page_of_organisation_standards()
        {
            _viewModelResponse.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.PaginationViewModel.PaginatedList.Items.Count.Should().Be(10);
            _viewModelResponse.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.PaginationViewModel.ItemsPerPage.Should().Be(10);
        }

        [Test]
        public void Should_display_organisation_standards_sorted_by_standard_name_ascending()
        {
            for (int standard = 0; standard < 10; standard++)
            {
                _viewModelResponse.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.PaginationViewModel.PaginatedList.Items[standard].Title.Should().StartWith("A");
            }
        }
    }
}
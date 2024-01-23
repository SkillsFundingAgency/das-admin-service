using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class Given_I_request_view_organisation_page_after_change_per_page_organsation_standards : RegisterBase
    {
        private RegisterViewAndEditOrganisationViewModel _viewModelResponse;

        public async Task Setup(int approvedStandardsPerPage)
        {
            Sut = new RegisterController(ControllerSession.Object, RegisterApiClient.Object, ApplicationApiClient.Object, OrganisationsApiClient.Object, ContactsApiClient.Object, Env.Object);
            
            Sut.ChangeStandardsPerPageViewOrganisationApprovedStandards(OrganisationOneOrganisationId, approvedStandardsPerPage);
            
            var viewActionResult = await Sut.ViewOrganisation(OrganisationOneOrganisationId);
            var result = viewActionResult as ViewResult;

            _viewModelResponse = result.Model as RegisterViewAndEditOrganisationViewModel;
        }

        [TestCase(2, 1)]
        [TestCase(5, 1)]
        [TestCase(9, 1)]
        public async Task Should_have_first_page_of_organisation_standards(int approvedStandardsPerPage, int pageNumber)
        {
            await Setup(approvedStandardsPerPage);
            _viewModelResponse.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.PaginationViewModel.PaginatedList.PageIndex.Should().Be(pageNumber);
        }

        [TestCase(2, 2)]
        [TestCase(5, 5)]
        [TestCase(9, 9)]
        public async Task Should_display_items_per_page_of_organisation_standards(int approvedStandardsPerPage, int itemsPerPage)
        {
            await Setup(approvedStandardsPerPage);
            _viewModelResponse.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.PaginationViewModel.PaginatedList.Items.Count.Should().Be(itemsPerPage);
            _viewModelResponse.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.PaginationViewModel.ItemsPerPage.Should().Be(itemsPerPage);
        }

        [TestCase(2)]
        [TestCase(5)]
        [TestCase(9)]
        public async Task Should_display_organisation_standards_sorted_by_standard_name_ascending(int approvedStandardsPerPage)
        {
            await Setup(approvedStandardsPerPage);
            for (int standard = 0; standard < approvedStandardsPerPage; standard++)
            {
                _viewModelResponse.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.PaginationViewModel.PaginatedList.Items[standard].Title.Should().StartWith("A");
            }
        }
    }
}
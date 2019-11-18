using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Extensions.TagHelpers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class Given_I_request_view_organiation_page_after_paging_organsation_standards : RegisterBase
    {
        private RegisterViewAndEditOrganisationViewModel _viewModelResponse;

        [SetUp]
        public async Task Arrange()
        {
            Sut = new RegisterController(ControllerSession.Object, ApiClient.Object, ApplyApiClient.Object, ContactsApiClient.Object, StandardServiceClient.Object, Env.Object);
            
            Sut.ChangePageViewOrganisationApprovedStandards(OrganisationOneOrganisationId, 2);
            
            var viewActionResult = await Sut.ViewOrganisation(OrganisationOneOrganisationId);
            var result = viewActionResult as ViewResult;

            _viewModelResponse = result.Model as RegisterViewAndEditOrganisationViewModel;
        }

        [Test]
        public void Should_have_second_page_of_organisation_standards()
        {
            _viewModelResponse.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.PaginationViewModel.PaginatedList.PageIndex.Should().Be(2);
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
                _viewModelResponse.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.PaginationViewModel.PaginatedList.Items[standard].StandardCollation.Title.Should().StartWith("B");
            }
        }
    }
}
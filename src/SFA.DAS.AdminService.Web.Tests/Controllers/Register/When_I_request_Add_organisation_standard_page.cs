using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class When_I_request_Add_organisation_standard_page : RegisterAddStandardBase
    {
        private IEnumerable<StandardVersion> _newStandardVersionsResponse;

        [SetUp]
        public void Arrange()
        {
            Sut = new RegisterController(ControllerSession.Object, ApiClient.Object, ApplyApiClient.Object, ContactsApiClient.Object, Env.Object);

            SetUpAddStandardViewModelWithStandard();
        }

        [Test]
        public async Task And_CurrentStandardIsStoredInSession_Then_MapViewModel()
        {
            var result = await Sut.AddOrganisationStandard(OrganisationOneOrganisationId, AddOrganisationStandardViewModel.IFateReferenceNumber) as ViewResult;

            var viewModel = result.Model as RegisterAddOrganisationStandardViewModel;

            viewModel.Should().BeEquivalentTo(AddOrganisationStandardViewModel);
        }

        [Test, MoqAutoData]
        public async Task And_DifferentStandardIsStoredInSession_Then_GetNewStandardData(string newReference)
        {
            SetUpAddStandardViewModelWithStandard();
            SetupGetNewStandardVersionsResponse(newReference);

            var result = await Sut.AddOrganisationStandard(OrganisationOneOrganisationId, newReference) as ViewResult;

            var viewModel = result.Model as RegisterAddOrganisationStandardViewModel;

            ApiClient.Verify(c => c.GetStandardVersions(newReference), Times.Once());
            viewModel.IFateReferenceNumber.Should().Be(_newStandardVersionsResponse.First().IFateReferenceNumber);
        }

        [Test, MoqAutoData]
        public async Task And_NoInProgressAddIsInSession_Then_GetNewStandardData(string newReference)
        {
            SetUpEmptyAddStandardViewModel();
            SetupGetNewStandardVersionsResponse(newReference);

            var result = await Sut.AddOrganisationStandard(OrganisationOneOrganisationId, newReference) as ViewResult;

            var viewModel = result.Model as RegisterAddOrganisationStandardViewModel;

            ApiClient.Verify(c => c.GetStandardVersions(newReference), Times.Once());
            viewModel.IFateReferenceNumber.Should().Be(_newStandardVersionsResponse.First().IFateReferenceNumber);
        }

        public void SetupGetNewStandardVersionsResponse(string ifateReferenceNumber)
        {
            var newStandardVersion1_0 = Fixture.Build<StandardVersion>()
                .With(v => v.IFateReferenceNumber, ifateReferenceNumber)
                .With(v => v.Version, "1.0")
                .With(v => v.VersionMajor, 1)
                .With(v => v.VersionMinor, 0).Create();

            var newStandardVersion1_1 = Fixture.Build<StandardVersion>()
                .With(v => v.IFateReferenceNumber, ifateReferenceNumber)
                .With(v => v.Version, "1.1")
                .With(v => v.VersionMajor, 1)
                .With(v => v.VersionMinor, 1).Create();

            _newStandardVersionsResponse = new List<StandardVersion>
            {
                newStandardVersion1_0,
                newStandardVersion1_1
            };

            ApiClient.Setup(c => c.GetStandardVersions(ifateReferenceNumber)).ReturnsAsync(_newStandardVersionsResponse);
        }
    }
}

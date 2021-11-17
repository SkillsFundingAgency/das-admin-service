using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class When_I_request_Add_organisation_standard_page : RegisterBase
    {
        private Fixture _fixture;

        private RegisterAddOrganisationStandardViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            Sut = new RegisterController(ControllerSession.Object, ApiClient.Object, ApplyApiClient.Object, ContactsApiClient.Object, Env.Object);
        }

        [Test]
        public async Task And_current_standard_is_stored_in_session_Then_map_view_model()
        {
            SetupControllerSession();

            var result = await Sut.AddOrganisationStandard(_viewModelResponse.OrganisationId, _viewModelResponse.StandardId.ToString()) as ViewResult;

            var viewModel = result.Model as RegisterAddOrganisationStandardViewModel;

            viewModel.Should().BeEquivalentTo(_viewModelResponse);
        }

        [Test]
        public async Task And_different_standard_is_stored_in_session_Then_reset_session()
        {
            SetupControllerSession();

            var result = await Sut.AddOrganisationStandard(_viewModelResponse.OrganisationId, _viewModelResponse.StandardId.ToString()) as ViewResult;

            var viewModel = result.Model as RegisterAddOrganisationStandardViewModel;

            //viewModel.Should().BeEquivalentTo(_viewModelResponse);
        }

        private void SetupControllerSession()
        {
            _viewModelResponse = _fixture.Create<RegisterAddOrganisationStandardViewModel>();

            ControllerSession.Setup(session => session.AddOrganisationStandardViewModel).Returns(_viewModelResponse);
        }
    }
}

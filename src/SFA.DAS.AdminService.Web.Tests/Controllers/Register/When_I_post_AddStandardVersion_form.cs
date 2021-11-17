using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class When_I_post_AddStandardVersion_form : RegisterBase
    {
        [SetUp]
        public void Arrange()
        {
            Sut = new RegisterController(ControllerSession.Object, ApiClient.Object, ApplyApiClient.Object, ContactsApiClient.Object, Env.Object);
        }
        [Test]
        public async Task And_the_form_is_valid_Then_update_AddStandardVersionViewModel()
        {

        }

        [Test]
        public async Task And_the_form_is_valid_Then_redirect_to_AddStandardVersion()
        {

        }

        [Test]
        public void And_the_form_is_invalid_Then_return_view()
        {
            var viewModel = new RegisterAddStandardVersionViewModel();

            var result = Sut.AddOrganisationStandardVersion(viewModel) as ViewResult;

            result.ViewData.ModelState.IsValid.Should().BeFalse();
        }
    }
}

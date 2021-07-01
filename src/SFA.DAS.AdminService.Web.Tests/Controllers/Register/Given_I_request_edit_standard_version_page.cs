using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class Given_I_request_edit_standard_version_page : RegisterBase
    {

        [SetUp]
        public async Task Arrange()
        {
            
        }

        [Test]
        public void Then_edit_standard_version_view_returned()
        {

        }

        [Test]
        public void And_submit_valid_updated_dates_Then_return_redirect_to_view_organisation_standard()
        {
            ApiClient.Setup(client => client.UpdateEpaOrganisationStandardVersion(It.IsAny<UpdateEpaOrganisationStandardVersionRequest>()))
                .ReturnsAsync("OK");


        }
    }
}

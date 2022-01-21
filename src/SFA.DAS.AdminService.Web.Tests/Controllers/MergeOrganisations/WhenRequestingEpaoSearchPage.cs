using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Merge;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenRequestingEpaoSearchPage : MergeControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [TestCase("primary")]
        [TestCase("secondary")]
        public void Then_SearchTypeIsMapped(string type)
        {
            var result = MergeController.SearchEpao(type) as ViewResult;

            var model = result.Model as SearchOrganisationViewModel;

            model.OrganisationType.Should().Be(type);
        }

        [TestCase("primary", "assessor")]
        [TestCase("secondary", "assessor")]
        [TestCase("primary", null)]
        [TestCase("secondary", null)]
        public void Then_SearchStringIsMapped(string type, string searchString)
        {
            var result = MergeController.SearchEpao(type, searchString) as ViewResult;

            var model = result.Model as SearchOrganisationViewModel;

            model.SearchString.Should().Be(searchString);
        }

    }
}

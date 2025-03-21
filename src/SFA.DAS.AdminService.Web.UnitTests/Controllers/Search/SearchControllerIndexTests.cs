using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Search;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerIndexTests : SearchControllerTestsBase
    {
        [Test]
        public void Index_ReturnsViewResult_WithNewViewModel_WhenVmIsNull()
        {
            // Arrange
            SearchInputViewModel vm = null;

            // Act
            var result = _controller.Index(vm);

            // Assert
            result.Should().NotBeNull();
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<SearchInputViewModel>().Subject.Should().NotBeNull();
        }

        [Test]
        public void Index_ReturnsViewResult_WithProvidedViewModel_WhenVmIsNotNull()
        {
            // Arrange
            var vm = new SearchInputViewModel { FirstName = "John", LastName = "Doe" };

            // Act
            var result = _controller.Index(vm);

            // Assert
            result.Should().NotBeNull();
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(vm); 
        }
    }
}

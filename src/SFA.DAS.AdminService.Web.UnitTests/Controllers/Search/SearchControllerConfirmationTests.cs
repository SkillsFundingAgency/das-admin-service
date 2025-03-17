using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Common.Extensions;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerConfirmationTests : SearchControllerTestsBase 
    {
        [Test]
        [MoqAutoData]
        public void Confirm_SessionAndSelectedResultValid_ReturnsViewWithCorrectModel()
        {
            //Arrange
            var printRunDate = DateTime.Now.AddDays(3).ToSfaShortDateString();
         
            //Act
            var result = _controller.ConfirmFrameworkReprint(printRunDate);

            //Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var resultModel = viewResult.Model.Should().BeOfType<FrameworkLearnerReprintSubmittedViewModel>().Subject;
            resultModel.PrintRunDate.Should().Be(printRunDate); 
        }
    }
}
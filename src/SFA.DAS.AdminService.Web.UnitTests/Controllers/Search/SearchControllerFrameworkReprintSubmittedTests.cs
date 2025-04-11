using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerFrameworkReprintSubmittedTests : SearchControllerTestsBase 
    {
        [Test]
        [MoqAutoData]
        public async Task Confirm_SessionAndSelectedResultValid_ReturnsViewWithCorrectModel()
        {
            //Arrange
            var printRunDate = DateTime.Now.AddDays(3);
            _scheduleApiClientMock.Setup(s => s.GetNextScheduledRun((int)ScheduleType.PrintRun))
                .ReturnsAsync(new ScheduleRun { RunTime = printRunDate });
         
            //Act
            var result = await _controller.FrameworkReprintSubmitted();

            //Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var resultModel = viewResult.Model.Should().BeOfType<FrameworkReprintSubmittedViewModel>().Subject;
            resultModel.PrintRunDate.Should().Be(printRunDate.ToSfaShortDateString()); 
        }
    }
}
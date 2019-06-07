using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions.Primitives;
using SFA.DAS.AdminService.Application.Interfaces;
using SFA.DAS.AdminService.Application.Interfaces.Validation;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services;

namespace SFA.DAS.AdminService.Web.Tests.Services
{
    [TestFixture]
    public class AnswerServiceGetAnswersTests
    {
        private AnswerService _answerService;
        private Mock<IApplyApiClient> _mockApplyApiClient;
        private Mock<IApiClient> _mockAssessorApiClient;

        private Guid _applicationId;

        [SetUp]
        public void Setup()
        {
            _applicationId = Guid.NewGuid();
            _mockApplyApiClient = new Mock<IApplyApiClient>();
            _mockAssessorApiClient = new Mock<IApiClient>();
            _answerService = new AnswerService(
                _mockApplyApiClient.Object,
                _mockAssessorApiClient.Object
            );
        }


        [TestCase("tag-one","answer1")]
        [TestCase("tag-two", null)]
        [TestCase(null, null)]
        public void WhenGettingAnswersForQuestionTag( string questionTag, string answer)
        {
            _mockApplyApiClient.Setup(x => x.GetAnswer(_applicationId, questionTag))
                .Returns(Task.FromResult(new GetAnswersResponse {Answer = answer}));

            var expectedResult = _answerService.GetAnswer(_applicationId, questionTag).Result;

            Assert.AreEqual(expectedResult, answer);
        }

        [TestCase("tag-one", "answer1",false)]
        [TestCase("tag-one", null, true)]
        public void WhenGettingAnswersForQuestionTagWithWrongApplicationId(string questionTag, string answer, bool useInvalidApplicationId)
        {
            var currentApplicationId = Guid.NewGuid();
            var newApplicationId = Guid.NewGuid();

            _mockApplyApiClient.Setup(x => x.GetAnswer(It.IsAny<Guid>(), questionTag))
                .Returns(Task.FromResult(new GetAnswersResponse { Answer = null }));

            _mockApplyApiClient.Setup(x => x.GetAnswer(currentApplicationId, questionTag))
                .Returns(Task.FromResult(new GetAnswersResponse { Answer = answer }));

            if (useInvalidApplicationId)
                currentApplicationId = newApplicationId;

             var expectedResultFromValidApplicationId = _answerService.GetAnswer(currentApplicationId, questionTag).Result;

            Assert.AreEqual(expectedResultFromValidApplicationId, answer);
        }
    }
}

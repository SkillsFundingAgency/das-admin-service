using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AdminService.Application.Interfaces;
using SFA.DAS.AdminService.Application.Interfaces.Validation;
using SFA.DAS.AdminService.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AdminService.Web.Tests.Services
{
    [TestFixture]
    public class AnswerInjectionServiceApplyOrganisationStandardIntoRegisterTests
    {
        private AnswerInjectionService _answerInjectionService;
        private IValidationService _validationService;
        private Mock<IApiClient> _mockApiClient;
        private Mock<IApplicationApiClient> _mockApplyClient;
        private IAssessorValidationService _assessorValidationService;
        private Mock<ILogger<AnswerService>> _mockLogger;
        private Mock<ISpecialCharacterCleanserService> _mockSpecialCharacterCleanserService;

        [SetUp]
        public void Setup()
        {
            _mockApiClient = new Mock<IApiClient>();

            _mockApiClient.Setup(r => r.SearchOrganisations(It.IsAny<string>()))
                .ReturnsAsync(new List<AssessmentOrganisationSummary> { new AssessmentOrganisationSummary { Id = "EPA0001" } });

            _mockApiClient.Setup(r => r.GetDeliveryAreas())
               .ReturnsAsync(new List<DeliveryArea> { new DeliveryArea { Id = 1, Area = "East Midlands" } });

            _mockApplyClient = new Mock<IApplicationApiClient>();

            _validationService = new ValidationService();
            _assessorValidationService = new AssessorValidationService(_mockApiClient.Object);
            _mockLogger = new Mock<ILogger<AnswerService>>();

            _mockSpecialCharacterCleanserService = new Mock<ISpecialCharacterCleanserService>();
            _mockSpecialCharacterCleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);

            _answerInjectionService = new AnswerInjectionService(
                _mockApiClient.Object,
                _mockApplyClient.Object,
                _validationService,
                _assessorValidationService,
                _mockSpecialCharacterCleanserService.Object,
                _mockLogger.Object
            );            
        }

        [Test, TestCaseSource(nameof(InjectionTestCases))]
        public void WhenInjectingOrganisationAndContactHappyPathForAnApplication(InjectionTestCase testCase)
        {
            List<ValidationErrorDetail> validationErrors = new List<ValidationErrorDetail>();

            if(testCase.IsOrganisationStandardTaken)
            {
                validationErrors.Add(new ValidationErrorDetail { ErrorMessage = "standard taken" });
            }
            else if(testCase.Command.StandardCode < 1)
            {
                validationErrors.Add(new ValidationErrorDetail { ErrorMessage = "standard invalid" });
            }

            _mockApiClient.Setup(r => r.CreateOrganisationStandardValidate(It.IsAny<CreateEpaOrganisationStandardValidationRequest>()))
                .Returns(Task.FromResult(new ValidationResponse { Errors = validationErrors }));

            _mockApiClient.Setup(r => r.CreateEpaOrganisationStandard(It.IsAny<CreateEpaOrganisationStandardRequest>()))
                .Returns(Task.FromResult(testCase.ExpectedResponse.EpaoStandardId));

            var actualResponse = _answerInjectionService.InjectApplyOrganisationStandardDetailsIntoRegister(testCase.Command).Result;

            if (actualResponse.WarningMessages.Count > 0)
            {
                actualResponse.WarningMessages = testCase.ExpectedResponse.WarningMessages;
            }

            Assert.AreEqual(JsonConvert.SerializeObject(testCase.ExpectedResponse), JsonConvert.SerializeObject(actualResponse));

            if (actualResponse.WarningMessages.Count > 0)
            {
                _mockApiClient.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<CreateEpaOrganisationStandardRequest>()), Times.Never);
            }
            else
            {
                _mockApiClient.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<CreateEpaOrganisationStandardRequest>()), Times.Once);
            }
        }

        protected static IEnumerable<InjectionTestCase> InjectionTestCases
        {
            get
            {
                yield return new InjectionTestCase(Guid.NewGuid(), Guid.NewGuid(), "EPA0001",
                    1, DateTime.UtcNow.Date, "East Midlands", false, "EPA Standard Id", null);
                yield return new InjectionTestCase(Guid.NewGuid(), Guid.NewGuid(), null,
                    0, DateTime.UtcNow.Date, "East Midlands", false, null, "organisation id missing");
                yield return new InjectionTestCase(Guid.NewGuid(), Guid.NewGuid(), "INVALID",
                    0, DateTime.UtcNow.Date, "East Midlands", false, null, "organisation id invalid");
                yield return new InjectionTestCase(Guid.NewGuid(), Guid.NewGuid(), "EPA0001",
                    0, DateTime.UtcNow.Date, "East Midlands", false, null, "standard invalid");
                yield return new InjectionTestCase(Guid.NewGuid(), Guid.NewGuid(), "EPA0001",
                    99, DateTime.UtcNow.Date, "East Midlands", true, null, "standard taken");
            }
        }

        public class InjectionTestCase
        {
            public CreateOrganisationStandardCommand Command { get; set; }
            public CreateOrganisationStandardFromApplyResponse ExpectedResponse { get; set; }

            public bool IsOrganisationStandardTaken { get; set; }


            public InjectionTestCase(Guid applyingContactId,
                Guid organisationId, string endPointAssessorOrganisationId, int standardCode, DateTime effectiveFrom, string deliveryAreas,
                bool isOrganisationStandardTaken, string epaoStandardId, string warningMessage)
            {
                var warningMessages = new List<string>();
                if (!string.IsNullOrEmpty(warningMessage))
                {
                    warningMessages.Add(warningMessage);
                }

                IsOrganisationStandardTaken = isOrganisationStandardTaken;

                var response = new CreateOrganisationStandardFromApplyResponse
                {
                    WarningMessages = warningMessages,
                    EpaoStandardId = epaoStandardId
                };

                Command = new CreateOrganisationStandardCommand
                {
                    OrganisationId = organisationId,
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                    StandardCode = standardCode,
                    EffectiveFrom = DateTime.Parse(effectiveFrom.ToString()),
                    DeliveryAreas = deliveryAreas?.Split(",").ToList(),
                    ApplyingContactId = applyingContactId,
                };
                ExpectedResponse = response;
            }
        }
    }
}



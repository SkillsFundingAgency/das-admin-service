using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using FHADetails = SFA.DAS.AssessorService.Api.Types.Models.AO.FHADetails;
using SFA.DAS.AdminService.Application.Interfaces;
using SFA.DAS.AdminService.Application.Interfaces.Validation;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AdminService.Web.Tests.Services
{
    [TestFixture]
    public class AnswerInjectionServiceApplyOrganisationAndContactintoRegisterTests
    {
        private AnswerInjectionService _answerInjectionService;
        private IValidationService _validationService;
        private Mock<IApiClient> _mockApiClient;
        private IAssessorValidationService _assessorValidationService;
        private Mock<ILogger<AnswerService>> _mockLogger;
        private Mock<ISpecialCharacterCleanserService> _mockSpecialCharacterCleanserService;

        [SetUp]
        public void Setup()
        {
            var applicationId = Guid.NewGuid();

            _mockApiClient = new Mock<IApiClient>();
            _mockApiClient.Setup(x => x.GetEpaContact("00000000-0000-0000-0000-000000000000"))
                .Returns(Task.FromResult<AssessmentOrganisationContact>(null));

            _mockApiClient.Setup(x => x.CreateEpaContact(It.IsAny<CreateEpaOrganisationContactRequest>()))
                .Returns(Task.FromResult(It.IsAny<string>()));

            _mockApiClient.Setup(x => x.CreateEpaContactValidate(It.IsAny<CreateEpaContactValidationRequest>()))
                .Returns(Task.FromResult(new ValidationResponse()));

            _mockApiClient.Setup(x => x.AssociateOrganisationWithEpaContact(It.IsAny<AssociateEpaOrganisationWithEpaContactRequest>()))
                .Returns(Task.FromResult(true));

            _validationService = new ValidationService();
            _assessorValidationService = new AssessorValidationService(_mockApiClient.Object);
            _mockLogger = new Mock<ILogger<AnswerService>>();

            _mockSpecialCharacterCleanserService = new Mock<ISpecialCharacterCleanserService>();
            _mockSpecialCharacterCleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);

            _answerInjectionService = new AnswerInjectionService(
                _mockApiClient.Object,
                _validationService,
                _assessorValidationService,
                _mockSpecialCharacterCleanserService.Object,
                _mockLogger.Object
            );

            var expectedOrganisationTypes = new List<OrganisationType>
            {
                new OrganisationType {Id = 1, Type = "Type 1"},
                new OrganisationType {Id = 2, Type = "Training Provider"}
            };

            _mockApiClient.Setup(x => x.GetOrganisationTypes())
                .Returns(Task.FromResult(expectedOrganisationTypes));

            _mockSpecialCharacterCleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);
        }

        [Test, TestCaseSource(nameof(InjectionTestCasesHappyPath))]
        public void WhenInjectingOrganisationAndContactHappyPathForAnApplication(InjectionTestCase testCase)
        {
            List<ValidationErrorDetail> validationErrors = new List<ValidationErrorDetail>();

            if (testCase.IsOrganisationNameTaken)
            {
                validationErrors.Add(new ValidationErrorDetail { ErrorMessage = "standard taken" });
            }
            if (testCase.IsUkprnTaken)
            {
                validationErrors.Add(new ValidationErrorDetail { ErrorMessage = "ukprn invalid" });
            }
            if (testCase.IsCompanyNumberTaken)
            {
                validationErrors.Add(new ValidationErrorDetail { ErrorMessage = "company number taken" });
            }
            if (testCase.IsCharityNumberTaken)
            {
                validationErrors.Add(new ValidationErrorDetail { ErrorMessage = "charity number taken" });
            }
            if (testCase.IsEmailTaken)
            {
                validationErrors.Add(new ValidationErrorDetail { ErrorMessage = "email taken" });
            }

            _mockApiClient.Setup(r => r.CreateOrganisationValidate(It.IsAny<CreateEpaOrganisationValidationRequest>()))
                .Returns(Task.FromResult(new ValidationResponse { Errors = validationErrors }));

            _mockApiClient.Setup(r => r.CreateEpaOrganisation(It.IsAny<CreateEpaOrganisationRequest>()))
                .Returns(Task.FromResult(testCase.ExpectedResponse.OrganisationId));

            _mockApiClient.Setup(r => r.SearchOrganisations(It.IsAny<string>()))
                .ReturnsAsync(new List<AssessmentOrganisationSummary> { new AssessmentOrganisationSummary { Id = testCase.ExpectedResponse.EpaOrganisationId } });
            _mockApiClient.Setup(r => r.GetEpaOrganisation(It.IsAny<string>()))
                .ReturnsAsync(new EpaOrganisation {Id = testCase.ExpectedResponse.ContactId, OrganisationData = new OrganisationData { FHADetails = new FHADetails() } });
            testCase.Command.CreatedBy = "00000000-0000-0000-0000-000000000000";
            var actualResponse = _answerInjectionService
                .InjectApplyOrganisationAndContactDetailsIntoRegister(testCase.Command).Result;
            if (actualResponse.WarningMessages.Count > 0)
            {

                actualResponse.WarningMessages = testCase.ExpectedResponse.WarningMessages;
            }

            Assert.AreEqual(JsonConvert.SerializeObject(testCase.ExpectedResponse),
               JsonConvert.SerializeObject(actualResponse));

            if (actualResponse.WarningMessages.Count > 0)
            {
                _mockApiClient.Verify(r => r.CreateEpaOrganisation(It.IsAny<CreateEpaOrganisationRequest>()), Times.Never);
                _mockApiClient.Verify(r => r.CreateEpaContact(It.IsAny<CreateEpaOrganisationContactRequest>()), Times.Never);
                _mockApiClient.Verify(r => r.UpdateFinancials(It.IsAny<UpdateFinancialsRequest>()), Times.Never);
            }
            else if(testCase.Command.IsEpaoApproved.Value || testCase.ExpectedResponse.ApplySourceIsEpao)
            {
                _mockApiClient.Verify(r => r.CreateEpaOrganisation(It.IsAny<CreateEpaOrganisationRequest>()), Times.Never);
                _mockApiClient.Verify(r => r.CreateEpaContact(It.IsAny<CreateEpaOrganisationContactRequest>()), Times.Never);
                _mockApiClient.Verify(r => r.UpdateFinancials(It.IsAny<UpdateFinancialsRequest>()), Times.Once);
            }
            else
            {
                _mockApiClient.Verify(r => r.CreateEpaOrganisation(It.IsAny<CreateEpaOrganisationRequest>()), Times.Once);
                _mockApiClient.Verify(r => r.CreateEpaContact(It.IsAny<CreateEpaOrganisationContactRequest>()), Times.Once);
                _mockApiClient.Verify(r => r.UpdateFinancials(It.IsAny<UpdateFinancialsRequest>()), Times.Never);
            }
        }

        protected static IEnumerable<InjectionTestCase> InjectionTestCasesHappyPath
        {
            get
            {
                yield return new InjectionTestCase("RoEPAO", true, false, null, false, null, null, null, false, null,
                    false, null, false, null, null, null, false, null, "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, true, null, false, null, null, null, false, null,
                    false, null, false, null, null, null, false, null, "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "12345678", false, "12345678", false, "1234", false, "EPA9999", "joe@cool.com", "Joe Cool", false,
                    null, "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, null, false, "trading name 1",
                    "TrainingProvider", "12345678", false, "12345678", false, "1234", false, "EPA9999", "joe@cool.com",
                    "Joe Cool", false, null, "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, null, false, null, "TrainingProvider",
                    "12345678", false, "12345678", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "organisation name not present", "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "a", false, null, "TrainingProvider",
                    "12345678", false, "12345678", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "organisation name too short", "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "aaa", true, null, "TrainingProvider",
                    "12345678", false, "12345678", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "organisation name already taken", "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProviderX",
                    "12345678", false, "12345678", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "organisation type not identified", "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", true, "12345678", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "ukprn invalid", "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "ABC", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "company number invalid", "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", true, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "company number taken", "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", false, "ABC", false, null, "joe@cool.com", "Joe Cool", false,
                    "charity number invalid", "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", false, "1234", true, null, "joe@cool.com", "Joe Cool", false,
                    "charity number taken", "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", false, "1234", false, null, "joecool.com", "Joe Cool", false,
                    "email invalid", "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", false, "1234", false, null, "joe@cool.com", "Joe Cool", true,
                    "email taken", "00000000-0000-0000-0000-000000000000");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", false, "1234", false, null, "joe@cool.com", "Jo", false,
                    "contact name bad", "00000000-0000-0000-0000-000000000000");

            }
        }

        public class InjectionTestCase
        {
            public CreateOrganisationContactCommand Command { get; set; }
            public CreateOrganisationAndContactFromApplyResponse ExpectedResponse { get; set; }

            public string WArningMessage1 { get; set; }
            public bool IsOrganisationNameTaken { get; set; }
            public bool IsUkprnTaken { get; set; }
            public bool IsCompanyNumberTaken { get; set; }
            public bool IsCharityNumberTaken { get; set; }
            public bool IsEmailTaken { get; set; }
            public string CreatedBy { get;set; } 
            public InjectionTestCase(string organisationReferenceType, bool isEpaoSource, bool isEpaoApproved,
                string organisationName, bool isOrganisationNameTaken, string tradingName, string organisationType,
                string ukprn, bool isUkprnTaken, string companyNumber, bool isCompanyNumberTaken, string charityNumber,
                bool isCharityNumberTaken, string organisationId, string email, string contactName, bool isEmailTaken,
                string warningMessage1, string contactId)
            {
                var warningMessages = new List<string>();
                if (!string.IsNullOrEmpty(warningMessage1))
                {
                    warningMessages.Add(warningMessage1);
                }

                IsOrganisationNameTaken = isOrganisationNameTaken;
                IsUkprnTaken = isUkprnTaken;
                IsCompanyNumberTaken = isCompanyNumberTaken;
                IsCharityNumberTaken = isCharityNumberTaken;
                IsEmailTaken = isEmailTaken;
                CreatedBy = contactId;

                var response = new CreateOrganisationAndContactFromApplyResponse
                {
                    IsEpaoApproved = isEpaoApproved,
                    ApplySourceIsEpao = isEpaoSource,
                    WarningMessages = warningMessages,
                    OrganisationId = organisationId,
                    ContactId = Guid.Parse(contactId)
                };
                //Command = new CreateOrganisationContactCommand();
                //{OrganisationReferenceType = organisationReferenceType};
                Command = new CreateOrganisationContactCommand
                {
                    OrganisationReferenceType = organisationReferenceType,
                    IsEpaoApproved = isEpaoApproved,
                    OrganisationName = organisationName,
                    TradingName = tradingName,
                    UseTradingName = true,
                    OrganisationUkprn = ukprn,
                    CompanyUkprn = "87654321",
                    CompanyNumber = companyNumber,
                    CharityNumber = charityNumber,
                    OrganisationType = organisationType,
                    ContactEmail = email,
                    ContactName = contactName,
                    FamilyName = "",
                    GivenNames = "",
                    ContactPhoneNumber = "11111111",
                    FinancialDueDate = DateTime.MaxValue,
                    IsFinancialExempt = false
                };
                ExpectedResponse = response;
            }
        }
    }
}



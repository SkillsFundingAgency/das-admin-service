using System;
using System.Linq;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using Newtonsoft.Json.Linq;
using SFA.DAS.AssessorService.Domain.Entities;
using FHADetails = SFA.DAS.AssessorService.Domain.Entities.FHADetails;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Tests.Services
{
    [TestFixture]
    public class AnswerServiceGatherAnswersTests
    {
        private AnswerService _answerService;
        private Mock<IApiClient> _mockApplyApiClient;
        private Mock<IQnaApiClient> _mockQnaApiClient;

        private Guid _applicationId;

        [SetUp]
        public void Arrange()
        {
            _applicationId = Guid.NewGuid();
            _mockApplyApiClient = new Mock<IApiClient>();
            _mockQnaApiClient = new Mock<IQnaApiClient>();
            _answerService = new AnswerService(
                _mockApplyApiClient.Object,
                _mockQnaApiClient.Object
            );
        }

        [Test,TestCaseSource(nameof(CommandTestCases))]
        public void WhenGatheringAnswersForAnApplication(CommandTest commandTestSetup)
        {
            var signinId = Guid.NewGuid();
            var contactId = Guid.NewGuid();

            var expectedCommand = new CreateOrganisationContactCommand
            {
                UseTradingName = commandTestSetup.UseTradingName,
                TradingName = commandTestSetup.TradingName,
                OrganisationName = commandTestSetup.OrganisationName,
                IsEpaoApproved = commandTestSetup.IsEpaoApproved,
                OrganisationType = commandTestSetup.OrganisationType,
                OrganisationUkprn = commandTestSetup.OrganisationUkprn,
                OrganisationReferenceType = commandTestSetup.OrganisationReferenceType,
                ContactName = commandTestSetup.ContactName,

                ContactAddress1 = commandTestSetup.ContactAddress != null
                    ? commandTestSetup.GetJsonValue(commandTestSetup.ContactAddress, "AddressLine1")
                    : commandTestSetup.ContactAddress1,

                ContactAddress2 = commandTestSetup.ContactAddress != null
                    ? commandTestSetup.GetJsonValue(commandTestSetup.ContactAddress, "AddressLine2")
                    : commandTestSetup.ContactAddress2,

                ContactAddress3 = commandTestSetup.ContactAddress != null
                    ? commandTestSetup.GetJsonValue(commandTestSetup.ContactAddress, "AddressLine3")
                    : commandTestSetup.ContactAddress3,

                ContactAddress4 = commandTestSetup.ContactAddress != null
                    ? commandTestSetup.GetJsonValue(commandTestSetup.ContactAddress, "AddressLine4")
                    : commandTestSetup.ContactAddress4,

                ContactPostcode = commandTestSetup.ContactAddress != null
                    ? commandTestSetup.GetJsonValue(commandTestSetup.ContactAddress, "Postcode")
                    : commandTestSetup.ContactPostcode,

                ContactEmail = commandTestSetup.ContactEmail,
                ContactPhoneNumber = commandTestSetup.ContactPhoneNumber,
                CompanyUkprn = commandTestSetup.CompanyUkprn,
                CompanyNumber = commandTestSetup.CompanyNumber,
                CharityNumber = commandTestSetup.CharityNumber,
                StandardWebsite = commandTestSetup.StandardWebsite,
                FamilyName = "",
                GivenNames = "",
                UserEmail ="",
                CreatedBy = contactId.ToString(),
                FinancialDueDate = commandTestSetup.FinancialDueDate,
                IsFinancialExempt = commandTestSetup.IsFinancialExempt,
                OtherApplyingUserEmails = new List<string>()
            };

            var applicationData = new Dictionary<string, object>
            {
                ["trading-name"] = commandTestSetup.TradingName,
                ["use-trading-name"] = commandTestSetup.UseTradingName,
                ["contact-name"] = commandTestSetup.ContactName,
                ["contact-address"] = commandTestSetup.ContactAddress,
                ["contact-address1"] = commandTestSetup.ContactAddress1,
                ["contact-address2"] = commandTestSetup.ContactAddress2,
                ["contact-address3"] = commandTestSetup.ContactAddress3,
                ["contact-address4"] = commandTestSetup.ContactAddress4,
                ["contact-postcode"] = commandTestSetup.ContactPostcode,
                ["contact-email"] = commandTestSetup.ContactEmail,
                ["contact-phone-number"] = commandTestSetup.ContactPhoneNumber,  
                ["company-ukprn"] = commandTestSetup.CompanyUkprn,
                ["company-number"] = commandTestSetup.CompanyNumber,
                ["charity-number"] = commandTestSetup.CharityNumber,
                ["standard-website"] = commandTestSetup.StandardWebsite
            };

            int? organisationUkprn = null;

            if (int.TryParse(commandTestSetup.OrganisationUkprn, out int ukprnOut))
                organisationUkprn = ukprnOut;

            var applicationOrganisation = new Organisation
            {
                Id = Guid.NewGuid(),
                EndPointAssessorName = commandTestSetup.OrganisationName,
                OrganisationType = new AssessorService.Domain.Entities.OrganisationType { Type = commandTestSetup.OrganisationType },
                EndPointAssessorUkprn = organisationUkprn,
                EndPointAssessorOrganisationId = commandTestSetup.OrganisationReferenceType,
                //CreatedBy = contactId.ToString(),
                OrganisationData =new AssessorService.Domain.Entities.OrganisationData
                {
                    RoEPAOApproved = commandTestSetup.IsEpaoApproved != null && commandTestSetup.IsEpaoApproved.Value,
                    FHADetails = new FHADetails
                    {
                        FinancialDueDate = commandTestSetup.FinancialDueDate,
                        FinancialExempt = commandTestSetup.IsFinancialExempt
                    }
                }
            };

            var applicationContact = new Contact { Id = contactId, SignInId = signinId, FamilyName = "", GivenNames = "",  Email = "" };

            var application = new ApplicationResponse
            {
                Id = _applicationId,
                ApplicationId = _applicationId,
                OrganisationId = applicationOrganisation.Id,
                CreatedBy = applicationContact.Id.ToString()
            };

            _mockApplyApiClient.Setup(x => x.GetApplicationFromAssessor(application.Id.ToString())).ReturnsAsync(application);
            _mockQnaApiClient.Setup(x => x.GetApplicationData(application.ApplicationId)).ReturnsAsync(applicationData);
            _mockApplyApiClient.Setup(x => x.GetOrganisation(application.OrganisationId)).ReturnsAsync(applicationOrganisation);
            _mockApplyApiClient.Setup(x => x.GetOrganisationContacts(applicationOrganisation.Id)).ReturnsAsync(new List<Contact> { applicationContact });

            var actualCommand = _answerService.GatherAnswersForOrganisationAndContactForApplication(_applicationId).Result;
   
            Assert.AreEqual(JsonConvert.SerializeObject(expectedCommand), JsonConvert.SerializeObject(actualCommand));
        }

        protected static IEnumerable<CommandTest> CommandTestCases
        {
            get
            {
                yield return new CommandTest("organisation name", "trading name 1", true, true, "true", "TrainingProvider","12343211", "RoEPAO", "Joe Contact", null, "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234","11112222","RC333333","1221121","www.test.com", DateTime.MaxValue, false);
                yield return new CommandTest("organisation name", "trading name 1", true, true, "true", "TrainingProvider", "12343211", "RoEPAO", "Joe Contact", null, "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234", "11112222", "RC333333", "1221121", "www.test.com", null, true);
                yield return new CommandTest("organisation name", "trading name 1", true, true, "yes", "TrainingProvider", "12343211", "RoEPAO", "Joe Contact", null, "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234", "11112222", "RC333333", "1221121", "www.test.com", DateTime.MaxValue, false);
                yield return new CommandTest("organisation name", "trading name 1", true, true, "1", "TrainingProvider", "12343211", "RoEPAO", "Joe Contact", null, "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234", "11112222", "RC333333", "1221121", "www.test.com", DateTime.MaxValue, false);
                yield return new CommandTest("organisation name", "trading name 1", true, false, "false", "TrainingProvider", "12343211", "RoEPAO", "Joe Contact", null, "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234", "11112222", "RC333333", "1221121", "www.test.com", DateTime.MaxValue, false);
                yield return new CommandTest("organisation name", "trading name 1", true, false, "0", "TrainingProvider", "12343211", "RoEPAO", "Joe Contact", "{ 'AddressLine1': 'address 1', 'AddressLine2': 'address 2', 'AddressLine3': 'address 3', 'AddressLine4': 'address 4', 'Postcode': 'CV1' }", "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234", "11112222", "RC333333", "1221121", "www.test.com", DateTime.MaxValue, false);
            }
        }

        public class CommandTest
        {
            public string OrganisationName { get; set; }
            public string OrganisationType { get; set; }
            public string OrganisationUkprn { get; set; }
            public bool? IsEpaoApproved { get; set; }
            public string TradingName { get; set; }
            public bool UseTradingName { get; set; }
            public string UseTradingNameString { get; set; }
            public string OrganisationReferenceType { get; set; }
            public string ContactName { get; set; }
            public string ContactAddress { get; set; }
            public string ContactAddress1 { get; set; }
            public string ContactAddress2 { get; set; }
            public string ContactAddress3 { get; set; }
            public string ContactAddress4 { get; set; }

            public string ContactPostcode { get; set; }
            public string ContactEmail { get; set; }
            public string ContactPhoneNumber { get; set; }
            public string CompanyUkprn { get; set; }
            public string CompanyNumber { get; set; }
            public string CharityNumber { get; set; }
            public string StandardWebsite { get; set; }

            public DateTime? FinancialDueDate { get; set; }
            public bool? IsFinancialExempt { get; set; }

            public CommandTest(string organisationName, string tradingName, bool isEpaoApproved, bool useTradingName, string useTradingNameString, string organisationType, string organisationUkprn 
               , string organisationReferenceType, string contactName, string contactAddress, string contactAddress1, string contactAddress2, string contactAddress3, string contactAddress4, string contactPostcode
               , string contactEmail, string contactPhoneNumber, string companyUkprn, string companyNumber, string charityNumber, string standardWebsite, DateTime? financialDueDate, bool? isFinancialExempt)
            {
                OrganisationName = organisationName;
                OrganisationType = organisationType;
                OrganisationUkprn = organisationUkprn;
                IsEpaoApproved = isEpaoApproved;
                TradingName = tradingName;
                UseTradingName = useTradingName;
                UseTradingNameString = useTradingNameString;
                OrganisationReferenceType = organisationReferenceType;
                ContactName = contactName;
                ContactAddress = contactAddress;
                ContactAddress1 = contactAddress1;
                ContactAddress2 = contactAddress2;
                ContactAddress3 = contactAddress3;
                ContactAddress4 = contactAddress4;
                ContactPostcode = contactPostcode;
                ContactEmail = contactEmail;
                ContactPhoneNumber = contactPhoneNumber;
                CompanyUkprn = companyUkprn;
                CompanyNumber = companyNumber;
                CharityNumber = charityNumber;
                StandardWebsite = standardWebsite;
                FinancialDueDate = financialDueDate;
                IsFinancialExempt = isFinancialExempt;
            }

            public string GetJsonValue(string json, string jsonKey)
            {
                try
                {
                    var contactAddress = JObject.Parse(json);
                    return contactAddress[jsonKey].ToString();
                }
                catch
                {
                    return null;
                }                
            }
        }

        [Test, TestCaseSource(nameof(StandardCommandTestCases))]
        public void WhenGatheringAnswersForAnOrganisationStandard(StandardCommandTest commandTestSetup)
        {
            var expectedCommand = new CreateOrganisationStandardCommand
            { 
                CreatedBy = commandTestSetup.CreatedBy,
                OrganisationId = commandTestSetup.OrganisationId.ToString(),
                StandardCode = commandTestSetup.StandardCode,
                EffectiveFrom = commandTestSetup.EffectiveFrom,
                DeliveryAreas = commandTestSetup.DeliveryAreas
            };

            var applicationData = new Dictionary<string, object>
            {
                ["effective-from"] = commandTestSetup.EffectiveFrom,
                ["delivery-areas"] = commandTestSetup.DeliveryAreasString
            };

            var applicationOrganisation = new Organisation
            {
                Id = commandTestSetup.OrganisationId,
                EndPointAssessorName = commandTestSetup.OrganisationName
            };

            var application = new ApplicationResponse
            {
                Id = _applicationId,
                ApplicationId = _applicationId,
                CreatedBy = commandTestSetup.CreatedBy,
                OrganisationId = applicationOrganisation.Id,
                StandardCode = commandTestSetup.StandardCode
            };

            _mockApplyApiClient.Setup(x => x.GetApplicationFromAssessor(application.Id.ToString())).ReturnsAsync(application);
            _mockQnaApiClient.Setup(x => x.GetApplicationData(application.ApplicationId)).ReturnsAsync(applicationData);
            _mockApplyApiClient.Setup(x => x.GetOrganisation(application.OrganisationId)).ReturnsAsync(applicationOrganisation);

            var actualCommand = _answerService.GatherAnswersForOrganisationStandardForApplication(_applicationId).Result;

            Assert.AreEqual(JsonConvert.SerializeObject(expectedCommand), JsonConvert.SerializeObject(actualCommand));
        }

        public class StandardCommandTest
        {
            public string OrganisationName { get; set; }
            public string CreatedBy { get; set; }
            public Guid OrganisationId { get; set; }
            public int StandardCode { get; set; }
            public DateTime EffectiveFrom { get; set; }
            public string DeliveryAreasString { get; set; }
            public List<string> DeliveryAreas => DeliveryAreasString?.Split(",").ToList();

            public StandardCommandTest(string organisationName, string createdBy
               , Guid organisationId, int standardCode, DateTime effectiveFrom, string deliveryAreasString)
            {
                OrganisationName = organisationName;
                CreatedBy = createdBy;
                OrganisationId = organisationId;
                StandardCode = standardCode;
                EffectiveFrom = DateTime.Parse(effectiveFrom.ToString());
                DeliveryAreasString = deliveryAreasString;
            }
        }

        protected static IEnumerable<StandardCommandTest> StandardCommandTestCases
        {
            get
            {
                yield return new StandardCommandTest("organisation name", Guid.NewGuid().ToString(), Guid.NewGuid(), 1, DateTime.UtcNow.Date, "East Midlands");
            }
        }
    }
    
}

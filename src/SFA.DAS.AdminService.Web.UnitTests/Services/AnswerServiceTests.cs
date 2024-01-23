using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Commands;
using SFA.DAS.AdminService.Web.Domain.Apply;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.UnitTests.Constraints;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contact = SFA.DAS.AssessorService.Domain.Entities.Contact;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AdminService.Web.UnitTests.Services
{
    [TestFixture]
    public class AnswerServiceTests
    {
        private readonly Mock<IApplicationService> _mockApplicationService;
        private readonly AnswerService _sut;
        private WithdrawalApplicationDetails _withdrawalApplicationDetails;
        private ApplicationDetails _applicationDetails;

        public AnswerServiceTests()
        {
            _mockApplicationService = new Mock<IApplicationService>();

            _withdrawalApplicationDetails = new WithdrawalApplicationDetails
            {
                ApplicationId = Guid.NewGuid(),
                EndPointAssessorOrganisationId = "TestOrganisationId",
                StandardCode = 1234,
                ConfirmedWithdrawalDate = DateTime.UtcNow
            };

            _applicationDetails = new ApplicationDetails
            {
                ApplicationData = new Dictionary<string, object>
                {
                    {"trading_name", "TestTradingName"},
                    {"use_trading_name", "yes"},
                    {"contact_given_name", "John"},
                    {"contact_family_name", "Doe"},
                    {"contact_address", null},
                    {"contact_address1", "Line1"},
                    {"contact_address2", "Line2"},
                    {"contact_address3", "Line3"},
                    {"contact_address4", "Line4"},
                    {"contact_postcode", "Postcode"},
                    {"contact_email", "john.doe@example.com"},
                    {"contact_phone_number", "123456789"},
                    {"company_ukprn", "123456"},
                    {"company_number", "12345"},
                    {"charity_number", "54321"},
                    {"standard_website", "https://example.com"}
                },
                Organisation = new Organisation()
                {
                    Id = Guid.NewGuid(),
                    EndPointAssessorName = "OrganisationName",
                    EndPointAssessorUkprn = 12345678,
                    EndPointAssessorOrganisationId = "EPA0001",
                    OrganisationType = new OrganisationType
                    {
                        Type = string.Empty
                    },
                    OrganisationData = new OrganisationData
                    {
                        RoEPAOApproved = true,
                        FHADetails = new AssessorService.Domain.Entities.FHADetails
                        {
                            FinancialDueDate = DateTime.UtcNow,
                            FinancialExempt = true
                        }
                    }
                },
                ApplyingContact = new Contact()
                {
                    Id = Guid.NewGuid(),
                    GivenNames = "Test",
                    FamilyName = "Tester",
                    Email = "test@test.com"
                },
                OrganisationContacts = new List<Contact>()
                {
                    new Contact()
                    {
                        Email = "test@test.com"
                    }
                }
            };

            _sut = new AnswerService(_mockApplicationService.Object);
        }

        [Test]
        public async Task GatherAnswersForOrganisationAndContactForApplication_ReturnsEmptyCreateOrganisationContactCommand_WhenApplicationDetailsAreNull()
        {
            // Arrange
            _mockApplicationService.Setup(m => m.GetApplicationsDetails(It.IsAny<Guid>()))
                .ReturnsAsync((ApplicationDetails)null);

            // Act
            var result = await _sut.GatherAnswersForOrganisationAndContactForApplication(Guid.NewGuid());

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, JsonIs.EquivalentTo(new CreateOrganisationContactCommand()));
            });
        }

        [Test]
        public async Task GatherAnswersForOrganisationAndContactForApplication_ReturnsCorrectlyMappedCommand()
        {
            // Arrange
            _mockApplicationService.Setup(x => x.GetApplicationsDetails(It.IsAny<Guid>()))
                .ReturnsAsync(_applicationDetails);

            // Act
            var result = await _sut.GatherAnswersForOrganisationAndContactForApplication(Guid.NewGuid());

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.TradingName, Is.EqualTo(_applicationDetails.ApplicationData["trading_name"]));
                Assert.That(result.UseTradingName, Is.True);
                Assert.That(result.ContactGivenNames, Is.EqualTo(_applicationDetails.ApplicationData["contact_given_name"]));
                Assert.That(result.ContactFamilyName, Is.EqualTo(_applicationDetails.ApplicationData["contact_family_name"]));
                Assert.That(result.ContactAddress1, Is.EqualTo(_applicationDetails.ApplicationData["contact_address1"]));
                Assert.That(result.ContactAddress2, Is.EqualTo(_applicationDetails.ApplicationData["contact_address2"]));
                Assert.That(result.ContactAddress3, Is.EqualTo(_applicationDetails.ApplicationData["contact_address3"]));
                Assert.That(result.ContactAddress4, Is.EqualTo(_applicationDetails.ApplicationData["contact_address4"]));
                Assert.That(result.ContactPostcode, Is.EqualTo(_applicationDetails.ApplicationData["contact_postcode"]));
                Assert.That(result.ContactEmail, Is.EqualTo(_applicationDetails.ApplicationData["contact_email"]));
                Assert.That(result.ContactPhoneNumber, Is.EqualTo(_applicationDetails.ApplicationData["contact_phone_number"]));
                Assert.That(result.CompanyUkprn, Is.EqualTo(_applicationDetails.ApplicationData["company_ukprn"]));
                Assert.That(result.CompanyNumber, Is.EqualTo(_applicationDetails.ApplicationData["company_number"]));
                Assert.That(result.CharityNumber, Is.EqualTo(_applicationDetails.ApplicationData["charity_number"]));
                Assert.That(result.StandardWebsite, Is.EqualTo(_applicationDetails.ApplicationData["standard_website"]));
                Assert.That(result.OrganisationName, Is.EqualTo(_applicationDetails.Organisation.EndPointAssessorName));
                Assert.That(result.OrganisationUkprn, Is.EqualTo(_applicationDetails.Organisation.EndPointAssessorUkprn));
                Assert.That(result.OrganisationId, Is.EqualTo(_applicationDetails.Organisation.Id));
                Assert.That(result.EndPointAssessorOrganisationId, Is.EqualTo(_applicationDetails.Organisation.EndPointAssessorOrganisationId));
                Assert.That(result.IsRoEpaoApproved, Is.EqualTo(_applicationDetails.Organisation.OrganisationData.RoEPAOApproved));
                Assert.That(result.ApplyingContactFamilyName, Is.EqualTo(_applicationDetails.ApplyingContact.FamilyName));
                Assert.That(result.ApplyingContactGivenNames, Is.EqualTo(_applicationDetails.ApplyingContact.GivenNames));
                Assert.That(result.ApplyingContactEmail, Is.EqualTo(_applicationDetails.ApplyingContact.Email));
            });
        }


        [Test]
        public async Task GatherAnswersForWithdrawOrganisationForApplication_ReturnsNull_WhenGetWithdrawalApplicationDetailsReturnsNull()
        {
            // Arrange
            _mockApplicationService.Setup(m => m.GetWithdrawalApplicationDetails(It.IsAny<Guid>())).ReturnsAsync((WithdrawalApplicationDetails)null);

            // Act
            var result = await _sut.GatherAnswersForWithdrawOrganisationForApplication(Guid.NewGuid(), "TestUser");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GatherAnswersForWithdrawOrganisationForApplication_ReturnsCorrectData()
        {
            // Arrange
            _mockApplicationService.Setup(x => x.GetWithdrawalApplicationDetails(It.IsAny<Guid>()))
                .ReturnsAsync(_withdrawalApplicationDetails);

            // Act
            var result = await _sut.GatherAnswersForWithdrawOrganisationForApplication(Guid.NewGuid(), "TestUser");

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.ApplicationId, Is.EqualTo(_withdrawalApplicationDetails.ApplicationId));
                Assert.That(result.EndPointAssessorOrganisationId, Is.EqualTo(_withdrawalApplicationDetails.EndPointAssessorOrganisationId));
                Assert.That(result.WithdrawalDate, Is.EqualTo(_withdrawalApplicationDetails.ConfirmedWithdrawalDate));
                Assert.That(result.UpdatedBy, Is.EqualTo("TestUser"));
            });
        }

        [Test]
        public async Task GatherAnswersForWithdrawStandardForApplication_ReturnsNull_WhenWithdrawalApplicationDetailsAreNull()
        {
            // Arrange
            _mockApplicationService.Setup(m => m.GetWithdrawalApplicationDetails(It.IsAny<Guid>()))
                .ReturnsAsync((WithdrawalApplicationDetails)null);

            // Act
            var result = await _sut.GatherAnswersForWithdrawStandardForApplication(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GatherAnswersForWithdrawStandardForApplication_ReturnsCorrectData()
        {
            // Arrange
            _mockApplicationService.Setup(x => x.GetWithdrawalApplicationDetails(It.IsAny<Guid>()))
                .ReturnsAsync(_withdrawalApplicationDetails);

            // Act
            var result = await _sut.GatherAnswersForWithdrawStandardForApplication(Guid.NewGuid());

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.EndPointAssessorOrganisationId, Is.EqualTo(_withdrawalApplicationDetails.EndPointAssessorOrganisationId));
                Assert.That(result.StandardCode, Is.EqualTo(_withdrawalApplicationDetails.StandardCode));
                Assert.That(result.WithdrawalDate, Is.EqualTo(_withdrawalApplicationDetails.ConfirmedWithdrawalDate));
            });
        }
    }
}

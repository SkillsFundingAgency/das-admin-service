using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.UnitTests.Services
{
    [TestFixture]
    public class ApplicationServiceTests
    {
        private ApplicationService _sut;
        private Mock<IOrganisationsApiClient> _organisationsApiClient;
        private Mock<IApplicationApiClient> _applicationApiClient;
        private Mock<IQnaApiClient> _qnaApiClient;

        [SetUp]
        public void Setup()
        {
            _organisationsApiClient = new Mock<IOrganisationsApiClient>();
            _applicationApiClient = new Mock<IApplicationApiClient>();
            _qnaApiClient = new Mock<IQnaApiClient>();

            _sut = new ApplicationService(_applicationApiClient.Object, _organisationsApiClient.Object, _qnaApiClient.Object);
        }

        [Test]
        public async Task GetWithdrawalApplicationDetails_ReturnsNull_WhenApplicationIsNull()
        {
            // Arrange
            _applicationApiClient.Setup(m => m.GetApplication(It.IsAny<Guid>())).ReturnsAsync((ApplicationResponse)null);

            // Act
            var result = await _sut.GetWithdrawalApplicationDetails(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetWithdrawalApplicationDetails_ReturnsNull_WhenApplicationDataIsNull()
        {
            // Arrange
            var application = new ApplicationResponse { ApplicationId = Guid.NewGuid() };
            _applicationApiClient.Setup(m => m.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);
            _qnaApiClient.Setup(m => m.GetApplicationDataDictionary(It.IsAny<Guid>())).ReturnsAsync((Dictionary<string, object>)null);

            // Act
            var result = await _sut.GetWithdrawalApplicationDetails(application.ApplicationId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetWithdrawalApplicationDetails_ReturnsNull_WhenOrganisationIsNull()
        {
            // Arrange
            var application = new ApplicationResponse { ApplicationId = Guid.NewGuid() };
            var applicationData = new Dictionary<string, object>();
            _applicationApiClient.Setup(m => m.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);
            _qnaApiClient.Setup(m => m.GetApplicationDataDictionary(It.IsAny<Guid>())).ReturnsAsync(applicationData);
            _organisationsApiClient.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync((Organisation)null);

            // Act
            var result = await _sut.GetWithdrawalApplicationDetails(application.ApplicationId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetWithdrawalApplicationDetails_ReturnsValidResult_WhenAllDataIsAvailable()
        {
            // Arrange
            var application = new ApplicationResponse { ApplicationId = Guid.NewGuid(), StandardCode = 123 };
            var applicationData = new Dictionary<string, object> { { "ConfirmedWithdrawalDate", DateTime.Now } };
            var organisation = new Organisation { EndPointAssessorOrganisationId = "dummyId" };

            _applicationApiClient.Setup(m => m.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);
            _qnaApiClient.Setup(m => m.GetApplicationDataDictionary(It.IsAny<Guid>())).ReturnsAsync(applicationData);
            _organisationsApiClient.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync(organisation);

            // Act
            var result = await _sut.GetWithdrawalApplicationDetails(application.ApplicationId);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.ApplicationId, Is.EqualTo(application.Id));
                Assert.That(result.EndPointAssessorOrganisationId, Is.EqualTo(organisation.EndPointAssessorOrganisationId));
                Assert.That(result.StandardCode, Is.EqualTo(application.StandardCode));
                Assert.That(result.ConfirmedWithdrawalDate, Is.EqualTo(applicationData["ConfirmedWithdrawalDate"]));
            });
        }

        [Test]
        public async Task GetApplicationsDetails_ReturnsNull_WhenApplicationIsNull()
        {
            // Arrange
            _applicationApiClient.Setup(m => m.GetApplication(It.IsAny<Guid>())).ReturnsAsync((ApplicationResponse)null);

            // Act
            var result = await _sut.GetApplicationsDetails(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetApplicationsDetails_ReturnsNull_WhenOrganisationContactsIsEmpty()
        {
            // Arrange
            var application = new ApplicationResponse { ApplicationId = Guid.NewGuid(), CreatedBy = "test" };
            var applicationData = new Dictionary<string, object>();
            var organisation = new Organisation { Id = Guid.NewGuid() };
            var contacts = new List<Contact> { };

            _applicationApiClient.Setup(m => m.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);
            _qnaApiClient.Setup(m => m.GetApplicationDataDictionary(It.IsAny<Guid>())).ReturnsAsync(applicationData);
            _organisationsApiClient.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync(organisation);
            _organisationsApiClient.Setup(m => m.GetOrganisationContacts(It.IsAny<Guid>())).ReturnsAsync(contacts);

            // Act
            var result = await _sut.GetApplicationsDetails(application.ApplicationId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetApplicationsDetails_ReturnsNull_WhenApplyingContactIsNull()
        {
            // Arrange
            var application = new ApplicationResponse { ApplicationId = Guid.NewGuid(), CreatedBy = Guid.NewGuid().ToString() };
            var applicationData = new Dictionary<string, Object>();
            var organisation = new Organisation { Id = Guid.NewGuid() };
            var contacts = new List<Contact> { new Contact { Id = Guid.NewGuid() } }; // Not the same Guid so no applying contact will be found

            _applicationApiClient.Setup(m => m.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);
            _qnaApiClient.Setup(m => m.GetApplicationDataDictionary(It.IsAny<Guid>())).ReturnsAsync(applicationData);
            _organisationsApiClient.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync(organisation);
            _organisationsApiClient.Setup(m => m.GetOrganisationContacts(It.IsAny<Guid>())).ReturnsAsync(contacts);

            // Act
            var result = await _sut.GetApplicationsDetails(application.ApplicationId);

            Assert.That(result, Is.Null);
        }


        [Test]
        public async Task GetApplicationsDetails_ReturnsValidResult_WhenAllDataIsAvailable()
        {
            // Arrange
            var application = new ApplicationResponse { ApplicationId = Guid.NewGuid(), CreatedBy = Guid.NewGuid().ToString() };
            var applicationData = new Dictionary<string, object>();
            var organisation = new Organisation { Id = Guid.NewGuid() };
            var contacts = new List<Contact> { new Contact { Id = Guid.Parse(application.CreatedBy) } };

            _applicationApiClient.Setup(m => m.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);
            _qnaApiClient.Setup(m => m.GetApplicationDataDictionary(It.IsAny<Guid>())).ReturnsAsync(applicationData);
            _organisationsApiClient.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync(organisation);
            _organisationsApiClient.Setup(m => m.GetOrganisationContacts(It.IsAny<Guid>())).ReturnsAsync(contacts);

            // Act
            var result = await _sut.GetApplicationsDetails(application.ApplicationId);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.ApplicationResponse, Is.EqualTo(application));
                Assert.That(result.ApplicationData, Is.EqualTo(applicationData));
                Assert.That(result.Organisation, Is.EqualTo(organisation));
                Assert.That(result.ApplyingContact, Is.EqualTo(contacts[0]));
            });
        }
    }
}

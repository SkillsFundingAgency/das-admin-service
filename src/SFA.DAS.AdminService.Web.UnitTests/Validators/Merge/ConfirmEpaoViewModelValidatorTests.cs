using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;
using SFA.DAS.AdminService.Web.Models.Merge;
using SFA.DAS.AdminService.Web.Validators.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Tests.Validators.Merge
{
    public class ConfirmEpaoViewModelValidatorTests
    {
        private Mock<IMergeOrganisationSessionService> _mockSessionService;
        private Mock<IMergeOrganisationsApiClient> _mergeOrganisationsApiClient;

        private ConfirmEpaoViewModel _viewModel;

        private ConfirmEpaoViewModelValidator Validator;

        [SetUp]
        public void Arrange()
        {
            _mockSessionService = new Mock<IMergeOrganisationSessionService>();
            _mergeOrganisationsApiClient = new Mock<IMergeOrganisationsApiClient>();

            _viewModel = new ConfirmEpaoViewModel
            {
                EpaoId = "EPA0001"
            };

            Validator = new ConfirmEpaoViewModelValidator(_mockSessionService.Object, _mergeOrganisationsApiClient.Object);
        }

        [Test]
        public void When_ThePrimaryEpaoSelectedIsAlreadyTheSelectedSecondaryEpao_Then_ReturnInvalid()
        {
            var mergeRequest = new MergeRequest
            {
                SecondaryEpao = new Epao("EPA0001", "Test Epao name", null)
            };

            _mockSessionService.Setup(ms => ms.GetMergeRequest())
                .Returns(mergeRequest);

            _viewModel.MergeOrganisationType = "primary";

            var result = Validator.Validate(_viewModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Single().ErrorMessage.Should().Be("The primary EPAO cannot be the same as the secondary EPAO.");
        }

        [Test]
        public void When_TheSecondaryEpaoSelectedIsAlreadyTheSelectedPrimaryEpao_Then_ReturnInvalid()
        {
            var mergeRequest = new MergeRequest
            {
                PrimaryEpao = new Epao("EPA0001", "Test Epao name", null)
            };

            _mockSessionService.Setup(ms => ms.GetMergeRequest())
                .Returns(mergeRequest);

            _viewModel.MergeOrganisationType = "secondary";

            var result = Validator.Validate(_viewModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Single().ErrorMessage.Should().Be("The secondary EPAO cannot be the same as the primary EPAO.");
        }

        [Test]
        public void When_TheSelectedSecondaryEpaoHasPreviouslyBeenMerge_Then_ReturnInvalid()
        {
            var mergeRequest = new MergeRequest { };

            _mockSessionService.Setup(ms => ms.GetMergeRequest())
                .Returns(mergeRequest);

            var mergeLogEntries = new List<MergeLogEntry>
            {
                new MergeLogEntry()
            };

            var mockResponse = new PaginatedList<MergeLogEntry>(mergeLogEntries, 1, 1, 1);

            _mergeOrganisationsApiClient.Setup(c => c.GetMergeLog(It.IsAny<GetMergeLogRequest>()))
                .ReturnsAsync(mockResponse);

            _viewModel.MergeOrganisationType = "secondary";

            var result = Validator.Validate(_viewModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Single().ErrorMessage.Should().Be("Secondary EPAO has previously been merged.");
        }
    }
}

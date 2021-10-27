using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Commands.ApproveStandardApplication;
using SFA.DAS.AdminService.Web.Commands.ReturnApplicationSequence;
using SFA.DAS.AdminService.Web.Queries.GetApplication;
using SFA.DAS.AdminService.Web.Queries.GetOrganisation;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApplicationResponse = SFA.DAS.AdminService.Web.Models.Apply.Application;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Apply
{
    public class WhenPostingReturnRefactored : ApplicationControllerTestsBase
    {
        private Guid _requestApplicationId;
        private Guid _organisationId;

        private ApplicationResponse _applicationResponse;
        private Organisation _organisation;
        private ApproveStandardApplicationResponse _approveStandardResponse;

        private const string WarningMessage = "Warning message";

        [SetUp]
        public void Setup()
        {
            _requestApplicationId = Guid.NewGuid();
            _organisationId = Guid.NewGuid();

            _applicationResponse = _fixture.Build<ApplicationResponse>()
                .With(x => x.ApplicationId, _requestApplicationId)
                .With(x => x.OrganisationId, _organisationId)
                .Create();

            _organisation = _fixture.Build<Organisation>()
                .With(x => x.Id, _organisationId)
                .Without(x => x.Certificates)
                .Without(x => x.Contacts)
                .Create();

            _approveStandardResponse = new ApproveStandardApplicationResponse
            {
                WarningMessages = new List<string>()
            };

            _mediator.Setup(m => m.Send(It.Is<GetApplicationQuery>(q => q.Id == _requestApplicationId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_applicationResponse);

            _mediator.Setup(m => m.Send(It.Is<GetOrganisationQuery>(q => q.Id == _organisationId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_organisation);

            _mediator.Setup(m => m.Send(It.IsAny<ApproveStandardApplicationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_approveStandardResponse);
        }

        [Test]
        public async Task Then_GetApplicationQueryIsSent()
        {
            await _controller.ReturnRefactored(_requestApplicationId, 1, "1", new BackViewModel());

            _mediator.Verify(m => m.Send(It.Is<GetApplicationQuery>(q => q.Id == _requestApplicationId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task Then_GetOrganisationQueryIsSent()
        {
            await _controller.ReturnRefactored(_requestApplicationId, 1, "1", new BackViewModel());

            _mediator.Verify(m => m.Send(It.Is<GetOrganisationQuery>(q => q.Id == _organisationId), It.IsAny<CancellationToken>()), Times.Once());

        }

        [Test]
        public async Task And_ActiveSequenceIsNull_And_SequenceNoIsStandardSequence_Then_ReturnRedirectToStandardApplication()
        {
            SetupActiveSequenceIsNull();

            var result  = await _controller.ReturnRefactored(_requestApplicationId, ApplyConst.STANDARD_SEQUENCE_NO, "1", new BackViewModel()) as RedirectToActionResult;

            result.ActionName.Should().Be("StandardApplications");
            result.ControllerName.Should().Be("StandardApplication");
        }

        [Test]
        public async Task Then_ReturnApplicationSequenceCommandIsSent()
        {
            SetupValidActiveSequence();

            await _controller.ReturnRefactored(_requestApplicationId, ApplyConst.STANDARD_SEQUENCE_NO, "1", new BackViewModel());

            _mediator.Verify(m => m.Send(It.IsAny<ReturnApplicationSequenceCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task And_ApproveReturnsWarnings_Then_ReturnApplicationIsNotSent()
        {
            SetupApprveApplicationWarning();

            await _controller.ReturnRefactored(_requestApplicationId, ApplyConst.STANDARD_SEQUENCE_NO, "1", new BackViewModel());

            _mediator.Verify(m => m.Send(It.IsAny<ReturnApplicationSequenceCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task And_NoWarnings_Then_ReturnApplicationIsSent()
        {
            await _controller.ReturnRefactored(_requestApplicationId, ApplyConst.STANDARD_SEQUENCE_NO, "1", new BackViewModel());

            _mediator.Verify(m => m.Send(It.IsAny<ReturnApplicationSequenceCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        }



        private void SetupValidActiveSequence()
        {
            _applicationResponse.ApplyData.Sequences = new List<ApplySequence>
            {
                _fixture.Build<ApplySequence>()
                    .With(s => s.SequenceNo, 2)
                    .With(s => s.IsActive, true)
                    .With(s => s.NotRequired, false)
                    .With(s => s.Sections, new List<ApplySection>())
                .Create()
            };
        }

        private void SetupActiveSequenceIsNull()
        {
            _applicationResponse.ApplyData.Sequences = new List<ApplySequence>();
        }

        private void SetupApprveApplicationWarning()
        {
            _approveStandardResponse.WarningMessages.Add(WarningMessage);
        }

    }
}

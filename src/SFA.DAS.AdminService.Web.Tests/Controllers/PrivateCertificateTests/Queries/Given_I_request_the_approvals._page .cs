﻿using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_approvals_page : CertificateQueryBase
    {
        private IActionResult _result;
        private CertificateApprovalViewModel _viewModelResponse;
        private CertificateApprovalsController certificateApprovalsController;

        [OneTimeSetUp]
        public void Arrange()
        {
            certificateApprovalsController =
                new CertificateApprovalsController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockApiClient                  
                    );

            MappingStartup.AddMappings();
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            AutoMapper.Mapper.Reset();
        }

        [Test]
        public void ShouldReturnApprovals()
        {
            var certificatePostApprovalViewModel = new CertificatePostApprovalViewModel
            {
                ActionHint = "Approved",
                ApprovalResults = new[]
                {
                    new ApprovalResult
                    {
                        CertificateReference = "SomeRef",
                        IsApproved = CertificateStatus.Submitted,
                        PrivatelyFundedStatus = CertificateStatus.Approved,
                        ReasonForChange = "SomeReason",
                        
                    }
                }
            };

            var returnResult = certificateApprovalsController.Approved(0).GetAwaiter().GetResult();
            returnResult.As<ViewResult>().Model.As<CertificateApprovalViewModel>().ApprovedCertificates.Items.Count()
                .Should().Be(3);
            
        }

        [Test]
        public void ShouldReturnRejections()
        {
            var certificatePostApprovalViewModel = new CertificatePostApprovalViewModel
            {
                ActionHint = "Approved",
                ApprovalResults = new[]
                {
                    new ApprovalResult
                    {
                        CertificateReference = "SomeRef",
                        IsApproved = CertificateStatus.Draft,
                        PrivatelyFundedStatus = CertificateStatus.Rejected,
                        ReasonForChange = "SomeReason"
                    }
                }
            };

            var returnResult = certificateApprovalsController.Rejected(0).GetAwaiter().GetResult();
            returnResult.As<ViewResult>().Model.As<CertificateApprovalViewModel>().RejectedCertificates.Items.Count()
                .Should().Be(3);
        }

        [Test]
        public void ShouldReturnToBeApproveds()
        {
            var certificatePostApprovalViewModel = new CertificatePostApprovalViewModel
            {
                ActionHint = "Approved",
                ApprovalResults = new[]
                {
                    new ApprovalResult
                    {
                        CertificateReference = "SomeRef",
                        IsApproved = CertificateStatus.ToBeApproved,
                        PrivatelyFundedStatus = CertificateStatus.SentForApproval,
                        ReasonForChange = "SomeReason"
                    }
                },
                PageIndex = 0,
            };

            var returnResult = certificateApprovalsController.SentForApproval(0).GetAwaiter().GetResult();
            returnResult.As<ViewResult>().Model.As<CertificateApprovalViewModel>().SentForApprovalCertificates.Items.Count()
                .Should().Be(4);
        }
    }
}


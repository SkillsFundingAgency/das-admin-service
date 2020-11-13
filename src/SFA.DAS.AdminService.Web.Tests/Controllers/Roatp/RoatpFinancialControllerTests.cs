using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Validators.Roatp.Applications;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Financial;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Roatp
{

    [TestFixture]
    public class RoatpFinancialControllerTests
    {
        private Mock<IRoatpOrganisationApiClient> _roatpOrganisationApiClient;
        private Mock<IRoatpApplicationApiClient> _applicationApplyApiClient;
        private Mock<IQnaApiClient> _qnaApiClient;
        private Mock<IRoatpFinancialClarificationViewModelValidator> _clarificationValidator;
        private RoatpFinancialController _controller;
        private readonly Guid _applicationId = Guid.NewGuid();
        private string _emailAddress = "Test@test.com";
        protected Mock<IHttpContextAccessor> MockHttpContextAccessor;
        private FinancialReviewDetails _financialReviewDetails;
        private string _buttonPressToAdd;
       
        [SetUp]
        public void Before_each_test()
        {
            _roatpOrganisationApiClient = new Mock<IRoatpOrganisationApiClient>();
            _applicationApplyApiClient = new Mock<IRoatpApplicationApiClient>();
            _clarificationValidator = new Mock<IRoatpFinancialClarificationViewModelValidator>();
            _qnaApiClient = new Mock<IQnaApiClient>();

           

            _financialReviewDetails = new FinancialReviewDetails();
            MockHttpContextAccessor = SetupMockedHttpContextAccessor();

            _controller = new RoatpFinancialController(_roatpOrganisationApiClient.Object,
                _applicationApplyApiClient.Object,
                _qnaApiClient.Object,
                MockHttpContextAccessor.Object, _clarificationValidator.Object)
            {
                ControllerContext = MockedControllerContext.Setup() 
            };
        }

        private static Mock<IHttpContextAccessor> SetupMockedHttpContextAccessor()
        {
            
            var user = MockedUser.Setup();

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext { User = user };

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            return mockHttpContextAccessor;
        }

        [Test]
        public void ViewApplication_creates_correct_view_model_with_email()
        {
            _applicationApplyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(
                new RoatpApplicationResponse {ApplicationId = _applicationId,
                    ApplyData = new RoatpApplyData {ApplyDetails =  new RoatpApplyDetails
                {
                    OrganisationName = "org name",
                    UKPRN = "12344321",
                    ReferenceNumber = "3443",
                    ProviderRouteName = "main",
                    ApplicationSubmittedOn = DateTime.Today
                },Sequences = new List<RoatpApplySequence>
                    {
                        new RoatpApplySequence
                        {
                            SequenceNo = 5,
                            NotRequired = true
                        }
                    }}});

            _applicationApplyApiClient.Setup(x => x.GetRoatpSequences()).ReturnsAsync(new List<RoatpSequence>());
            _qnaApiClient
                .Setup(x => x.GetQuestionTag(_applicationId, RoatpQnaConstants.QnaQuestionTags.HasParentCompany))
                .ReturnsAsync("No");
            _applicationApplyApiClient.Setup(x => x.GetContactForApplication(_applicationId))
                .ReturnsAsync(new AssessorService.Domain.Entities.Contact {Email = _emailAddress});

            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails))
                .ReturnsAsync(new Section {ApplicationId = _applicationId, QnAData = new QnAData()});
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.DescribeYourOrganisation))
                .ReturnsAsync(new Section {ApplicationId = _applicationId, QnAData = new QnAData()});

            var result = _controller.ViewApplication(_applicationId).GetAwaiter().GetResult();
            result.Should().BeAssignableTo<ViewResult>();

            var viewResult = result as ViewResult;
            var viewModel = viewResult.Model as RoatpFinancialApplicationViewModel;

            viewModel.ApplicantEmailAddress.Should().Be(_emailAddress);
        }


        [Test]
        public void SubmitClarification_redirects_when_no_application()
        {
            _applicationApplyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync((RoatpApplicationResponse)null);

            var result = _controller.SubmitClarification(_applicationId, new RoatpFinancialClarificationViewModel()).Result as RedirectToActionResult;
            Assert.AreEqual("OpenApplications",result.ActionName);
        }


        [TestCase(FinancialApplicationSelectedGrade.Outstanding)]
        [TestCase(FinancialApplicationSelectedGrade.Satisfactory)]
        [TestCase(FinancialApplicationSelectedGrade.Good)]
        [TestCase(FinancialApplicationSelectedGrade.Inadequate)]
        [TestCase(FinancialApplicationSelectedGrade.Exempt)]
        public void SubmitClarification_valid_submission(string grade)
        {
            _clarificationValidator.Setup(x =>
                    x.Validate(It.IsAny<RoatpFinancialClarificationViewModel>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new ValidationResponse {}));

            _applicationApplyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(
                new RoatpApplicationResponse
                {
                    ApplicationId = _applicationId,
                    ApplyData = new RoatpApplyData
                    {
                        ApplyDetails = new RoatpApplyDetails
                        {
                            OrganisationName = "org name",
                            UKPRN = "12344321",
                            ReferenceNumber = "3443",
                            ProviderRouteName = "main",
                            ApplicationSubmittedOn = DateTime.Today
                        },
                        Sequences = new List<RoatpApplySequence>
                        {
                            new RoatpApplySequence
                            {
                                SequenceNo = 5,
                                NotRequired = true
                            }
                        }
                    },
                    FinancialGrade = new FinancialReviewDetails()
                });
            _financialReviewDetails = new FinancialReviewDetails
            {
                GradedBy = MockHttpContextAccessor.Name,
                GradedDateTime = DateTime.UtcNow,
                SelectedGrade = grade,
                FinancialDueDate = DateTime.Today.AddDays(5),
                Comments = "comments",
                ClarificationResponse = "clarification response",
                ClarificationRequestedOn = DateTime.UtcNow
            };

            var vm = new RoatpFinancialClarificationViewModel
            {
                ApplicationId = _applicationId,
                FinancialReviewDetails = _financialReviewDetails,
                OutstandingFinancialDueDate = new FinancialDueDate
                {
                    Day = "1",
                    Month = "1",
                    Year = (DateTime.Now.Year + 1).ToString()
                },
                ClarificationResponse = "clarification response",
                ClarificationComments = "clarification comments",
                FilesToUpload = null
            };
            var result = _controller.SubmitClarification(_applicationId, vm).Result as RedirectToActionResult;
            _applicationApplyApiClient.Verify(x => x.ReturnFinancialReview(_applicationId, It.IsAny<FinancialReviewDetails>()), Times.Once);
            Assert.AreEqual("Graded", result.ActionName);
        }

        [Test]
        public void When_clarification_file_is_uploaded_and_page_is_refreshed_with_filename_included_in_model()
        {
            var buttonPressed = "submitClarificationFiles";
            _applicationApplyApiClient.Setup(x => x.GetRoatpSequences()).ReturnsAsync(new List<RoatpSequence>());
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.DescribeYourOrganisation))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });
            _controller = new RoatpFinancialController(_roatpOrganisationApiClient.Object,
                _applicationApplyApiClient.Object,
                _qnaApiClient.Object,
                MockHttpContextAccessor.Object, _clarificationValidator.Object)
            {
                ControllerContext = MockedControllerContext.Setup(buttonPressed)
            };

            _clarificationValidator.Setup(x =>
                    x.Validate(It.IsAny<RoatpFinancialClarificationViewModel>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new ValidationResponse { }));
           
            _applicationApplyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(
                new RoatpApplicationResponse
                {
                    ApplicationId = _applicationId,
                    ApplyData = new RoatpApplyData
                    {
                        ApplyDetails = new RoatpApplyDetails
                        {
                            OrganisationName = "org name",
                            UKPRN = "12344321",
                            ReferenceNumber = "3443",
                            ProviderRouteName = "main",
                            ApplicationSubmittedOn = DateTime.Today
                        },
                        Sequences = new List<RoatpApplySequence>
                        {
                            new RoatpApplySequence
                            {
                                SequenceNo = 5,
                                NotRequired = true
                            }
                        }
                    },
                    FinancialGrade = new FinancialReviewDetails()
                });

            _applicationApplyApiClient.Setup(x =>
                    x.UploadClarificationFile(_applicationId, It.IsAny<string>(), It.IsAny<IFormFileCollection>()))
                .ReturnsAsync(true);

                
            _financialReviewDetails = new FinancialReviewDetails
            {
                GradedBy = MockHttpContextAccessor.Name,
                GradedDateTime = DateTime.UtcNow,
                SelectedGrade = FinancialApplicationSelectedGrade.Good,
                FinancialDueDate = DateTime.Today.AddDays(5),
                Comments = "comments",
                ClarificationResponse = "clarification response",
                ClarificationRequestedOn = DateTime.UtcNow
            };

            var vm = new RoatpFinancialClarificationViewModel
            {
                ApplicationId = _applicationId,
                FinancialReviewDetails = _financialReviewDetails,
                OutstandingFinancialDueDate = new FinancialDueDate
                {
                    Day = "1",
                    Month = "1",
                    Year = (DateTime.Now.Year + 1).ToString()
                },
                ClarificationResponse = "clarification response",
                ClarificationComments = "clarification comments",
                FilesToUpload = null
            };
            var result = _controller.SubmitClarification(_applicationId, vm).Result as ViewResult;

            Assert.IsTrue(result.ViewName.Contains("Application_Clarification.cshtml"));
            var resultModel = result.Model as RoatpFinancialClarificationViewModel;

            Assert.IsTrue(resultModel.FinancialReviewDetails.ClarificationFiles[0].Filename == "file.pdf");
        }


        [Test]
        public void when_validation_errors_occur_page_refreshes_with_validation_messages()
        {
            var buttonPressed = "submitClarificationFiles";
            _applicationApplyApiClient.Setup(x => x.GetRoatpSequences()).ReturnsAsync(new List<RoatpSequence>());
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.DescribeYourOrganisation))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });
            _controller = new RoatpFinancialController(_roatpOrganisationApiClient.Object,
                _applicationApplyApiClient.Object,
                _qnaApiClient.Object,
                MockHttpContextAccessor.Object, _clarificationValidator.Object)
            {
                ControllerContext = MockedControllerContext.Setup(buttonPressed)
            };

            _clarificationValidator.Setup(x =>
                    x.Validate(It.IsAny<RoatpFinancialClarificationViewModel>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new ValidationResponse {Errors = new List<ValidationErrorDetail> {new ValidationErrorDetail {ErrorMessage = "error message", Field = "errorField"}}}));

            _applicationApplyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(
                new RoatpApplicationResponse
                {
                    ApplicationId = _applicationId,
                    ApplyData = new RoatpApplyData
                    {
                        ApplyDetails = new RoatpApplyDetails
                        {
                            OrganisationName = "org name",
                            UKPRN = "12344321",
                            ReferenceNumber = "3443",
                            ProviderRouteName = "main",
                            ApplicationSubmittedOn = DateTime.Today
                        },
                        Sequences = new List<RoatpApplySequence>
                        {
                            new RoatpApplySequence
                            {
                                SequenceNo = 5,
                                NotRequired = true
                            }
                        }
                    },
                    FinancialGrade = new FinancialReviewDetails()
                });

            _applicationApplyApiClient.Setup(x =>
                    x.UploadClarificationFile(_applicationId, It.IsAny<string>(), It.IsAny<IFormFileCollection>()))
                .ReturnsAsync(true);


            _financialReviewDetails = new FinancialReviewDetails
            {
                GradedBy = MockHttpContextAccessor.Name,
                GradedDateTime = DateTime.UtcNow,
                SelectedGrade = FinancialApplicationSelectedGrade.Good,
                FinancialDueDate = DateTime.Today.AddDays(5),
                Comments = "comments",
                ClarificationResponse = "clarification response",
                ClarificationRequestedOn = DateTime.UtcNow
            };

            var vm = new RoatpFinancialClarificationViewModel
            {
                ApplicationId = _applicationId,
                FinancialReviewDetails = _financialReviewDetails,
                OutstandingFinancialDueDate = new FinancialDueDate
                {
                    Day = "1",
                    Month = "1",
                    Year = (DateTime.Now.Year + 1).ToString()
                },
                ClarificationResponse = "clarification response",
                ClarificationComments = "clarification comments",
                FilesToUpload = null
            };
            var result = _controller.SubmitClarification(_applicationId, vm).Result as ViewResult;

            Assert.IsTrue(result.ViewName.Contains("Application_Clarification.cshtml"));
            var resultModel = result.Model as RoatpFinancialClarificationViewModel;
            Assert.AreEqual(1,resultModel.ErrorMessages.Count);
        }
    }
}
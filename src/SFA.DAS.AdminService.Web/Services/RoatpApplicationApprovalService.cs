using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Applications;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace SFA.DAS.AdminService.Web.Services
{
    public class RoatpApplicationApprovalService : IRoatpApplicationApprovalService
    {
        private const int FinancialHealthSequenceNo = 2;
        private const int EmployerProviderTypeId = 2;
        private const string PublicBodyOrganisationType = "A public body";
        private const string EducationalInstituteOrganisationType = "An educational institute";

        private readonly IQnaApiClient _qnaApiClient;
        private readonly IRoatpApplicationApiClient _roatpApplicationApiClient;
        private readonly IRoatpApiClient _roatpApiClient;

        public RoatpApplicationApprovalService(IQnaApiClient qnaApiClient, IRoatpApplicationApiClient roatpApplicationApiClient, IRoatpApiClient roatpApiClient)
        {
            _qnaApiClient = qnaApiClient;
            _roatpApplicationApiClient = roatpApplicationApiClient;
            _roatpApiClient = roatpApiClient;
        }
 
        public bool IsEligibleForRegister(string gatewayAssessmentStatus, string financialReviewStatus, IEnumerable<RoatpApplySequence> applySequences)
        {
            if (gatewayAssessmentStatus != ApplicationStatus.Approved)
            {
                return false;
            }

            var financialHealthChecksRequired = false;
            var financialHealthSequence = applySequences.FirstOrDefault(x => x.SequenceNo == FinancialHealthSequenceNo);
            if (financialHealthSequence != null)
            {
                financialHealthChecksRequired = !financialHealthSequence.NotRequired;
            }

            var validFinancialHealthStatuses = new [] { FinancialReviewStatus.Approved, FinancialReviewStatus.Exempt };
            if (financialHealthChecksRequired && !validFinancialHealthStatuses.Contains(financialReviewStatus))
            {
                return false;
            }

            var validApplicationSequenceStatuses = new[] { SequenceReviewStatus.Evaluated, SequenceReviewStatus.NotRequired };
            foreach(var sequence in applySequences)
            {
                if (!validApplicationSequenceStatuses.Contains(sequence.Status))
                {
                    return false;
                }
                foreach (var section in sequence.Sections.Where(s => !s.NotRequired))
                {
                    if (section.Status != AssessorReviewStatus.Approved) 
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        public async Task<RoatpApplicationApprovalViewModel> BuildApplicationApprovalViewModel(Guid applicationId)
        {
            var model = new RoatpApplicationApprovalViewModel
            {
                ApplicationId = applicationId
            };

            var application = await _roatpApplicationApiClient.GetApplication(applicationId);

            var providerRoutes = await _roatpApiClient.GetProviderTypes();
            var applicationProviderRoute = providerRoutes.FirstOrDefault(x => x.Id == application.ApplyData.ApplyDetails.ProviderRoute);
            var organisationTypes = await _roatpApiClient.GetOrganisationTypes(application.ApplyData.ApplyDetails.ProviderRoute);

            model.ProviderTypeId = applicationProviderRoute.Id;
            model.ApplicationRoute = applicationProviderRoute.Type;
            model.OrganisationTypes = organisationTypes;

            var preamblePage = await _qnaApiClient.GetPageBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.Preamble,
                                                                      RoatpQnaConstants.RoatpSections.Preamble.SectionId, RoatpQnaConstants.RoatpSections.Preamble.PageId);

            var preambleAnswers = preamblePage.PageOfAnswers.FirstOrDefault();

            var legalNameAnswer = preambleAnswers.Answers.FirstOrDefault(x => x.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.LegalName);
            var tradingNameAnswer = preambleAnswers.Answers.FirstOrDefault(x => x.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.TradingName);
            var ukprnAnswer = preambleAnswers.Answers.FirstOrDefault(x => x.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKPRN);

            model.LegalName = legalNameAnswer.Value;
            model.TradingName = tradingNameAnswer?.Value;
            model.UKPRN = ukprnAnswer.Value;
            model.OrganisationTypeId = await MapOrganisationDetailsQuestionsToRoatpOrganisationType(applicationId, model.ProviderTypeId);

            var verifiedCompanyAnswer = preambleAnswers.Answers.FirstOrDefault(x => x.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UkrlpVerificationCompany);
            if (verifiedCompanyAnswer != null && verifiedCompanyAnswer.Value == "TRUE")
            {
                var companyNumberAnswer = preambleAnswers.Answers.FirstOrDefault(x => x.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.CompanyNumber);
                model.CompanyNumber = companyNumberAnswer.Value;
            }

            var verifiedCharityAnswer = preambleAnswers.Answers.FirstOrDefault(x => x.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UkrlpVerificationCharity);
            if (verifiedCharityAnswer != null && verifiedCharityAnswer.Value == "TRUE")
            {
                var charityNumberAnswer = preambleAnswers.Answers.FirstOrDefault(x => x.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.CharityNumber);
                model.CharityNumber = charityNumberAnswer.Value;
            }
            
            return await Task.FromResult(model);
        }

        public async Task<bool> SubmitOrganisationToRoatpRegister(RoatpApplicationApprovalViewModel roatpApplicationModel)
        {
            var request = Mapper.Map<CreateRoatpOrganisationRequest>(roatpApplicationModel);
            request.StatusDate = DateTime.Now;

            var result = await _roatpApiClient.CreateOrganisation(request);

            if (result)
            {
                await _roatpApplicationApiClient.CompleteAssessorReview(roatpApplicationModel.ApplicationId, roatpApplicationModel.Username);
                
                return await _roatpApplicationApiClient.UpdateApplicationStatus(roatpApplicationModel.ApplicationId, ApplicationStatus.Approved);               
            }

            return await Task.FromResult(false);
        }

        private async Task<int> MapOrganisationDetailsQuestionsToRoatpOrganisationType(Guid applicationId, int providerTypeId)
        {
            int organisationTypeId = 0; // default to 'Unassigned'

            var organisationTypes = await _roatpApiClient.GetOrganisationTypes(providerTypeId);

            string organisationTypePageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting;
            string organisationTypeQuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeMainSupporting;
            if (providerTypeId == EmployerProviderTypeId)
            {
                organisationTypePageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeEmployer;
                organisationTypeQuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeEmployer;
            }
            var organisationTypePage = await _qnaApiClient.GetPageBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.YourOrganisation, 
                                                                        RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, organisationTypePageId);
            
            var organisationDetailsAnswers = organisationTypePage.PageOfAnswers.FirstOrDefault();
            var organisationTypeAnswer = organisationDetailsAnswers.Answers.FirstOrDefault(x => x.QuestionId == organisationTypeQuestionId);

            var matchingOrganisationType = organisationTypes.FirstOrDefault(x => x.Type.Equals(organisationTypeAnswer.Value.ToString()));

            if (matchingOrganisationType != null)
            {
                return matchingOrganisationType.Id;
            }

            switch (organisationTypeAnswer.Value)
            {
                case PublicBodyOrganisationType:
                    organisationTypeId = await MapPublicBodyOrganisationType(applicationId, organisationTypeAnswer.Value, providerTypeId, organisationTypes);
                    break;
                case EducationalInstituteOrganisationType:
                    organisationTypeId = await MapEducationalInstituteOrganisationType(applicationId, organisationTypeAnswer.Value, providerTypeId, organisationTypes);
                    break;
            }

            return organisationTypeId;
        }

        private async Task<int> MapPublicBodyOrganisationType(Guid applicationId, string organisationTypeText, int providerTypeId, IEnumerable<OrganisationType> organisationTypes)
        {
            var publicBodyOrganisationTypePageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.PublicBodyTypeMainSupporting;
            if (providerTypeId == EmployerProviderTypeId)
            {
                publicBodyOrganisationTypePageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.PublicBodyTypeEmployer;
            }
            var publicBodyOrganisationTypeQuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.PublicBodyTypeMainSupporting;
            if (providerTypeId == EmployerProviderTypeId)
            {
                publicBodyOrganisationTypeQuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.PublicBodyTypeEmployer;
            }

            var publicBodyOrganisationTypePage = await _qnaApiClient.GetPageBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                 RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, publicBodyOrganisationTypePageId);

            var organisationDetailsAnswers = publicBodyOrganisationTypePage.PageOfAnswers.FirstOrDefault();
            var organisationTypeAnswer = organisationDetailsAnswers.Answers.FirstOrDefault(x => x.QuestionId == publicBodyOrganisationTypeQuestionId);

            var matchingOrganisationType = organisationTypes.FirstOrDefault(x => x.Type.Equals(organisationTypeAnswer.Value.ToString()));

            if (matchingOrganisationType != null)
            {
                return matchingOrganisationType.Id;
            }

            return 0;
        }

        private async Task<int> MapEducationalInstituteOrganisationType(Guid applicationId, string organisationTypeText, int providerTypeId, IEnumerable<OrganisationType> organisationTypes)
        {
            var educationOrganisationTypePageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.EducationalInstituteTypeMainSupporting;
            if (providerTypeId == EmployerProviderTypeId)
            {
                educationOrganisationTypePageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.EducationalInstituteTypeEmployer;
            }
            var educationOrganisationTypeQuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.EducationalInstituteTypeMainSupporting;
            if (providerTypeId == EmployerProviderTypeId)
            {
                educationOrganisationTypeQuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.EducationalInstituteTypeEmployer;
            }

            var educationOrganisationTypePage = await _qnaApiClient.GetPageBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                 RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, educationOrganisationTypePageId);

            var organisationDetailsAnswers = educationOrganisationTypePage.PageOfAnswers.FirstOrDefault();
            var organisationTypeAnswer = organisationDetailsAnswers.Answers.FirstOrDefault(x => x.QuestionId == educationOrganisationTypeQuestionId);

            var matchingOrganisationType = organisationTypes.FirstOrDefault(x => x.Type.Equals(organisationTypeAnswer.Value.ToString()));

            if (matchingOrganisationType != null)
            {
                return matchingOrganisationType.Id;
            }

            return 0;
        }
    }
}

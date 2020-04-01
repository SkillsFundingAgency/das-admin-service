using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayExperienceAndAccreditationOrchestrator : IGatewayExperienceAndAccreditationOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IRoatpExperienceAndAccreditationApiClient _experienceAndAccreditationApiClient;
        private readonly ILogger<GatewayExperienceAndAccreditationOrchestrator> _logger;

        public GatewayExperienceAndAccreditationOrchestrator(IRoatpApplicationApiClient applyApiClient, IRoatpExperienceAndAccreditationApiClient experienceAndAccreditationApiClient, ILogger<GatewayExperienceAndAccreditationOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _experienceAndAccreditationApiClient = experienceAndAccreditationApiClient;
            _logger = logger;
        }

        public async Task<SubcontractorDeclarationViewModel> GetSubcontractorDeclarationViewModel(GetSubcontractorDeclarationRequest request)
        {
            _logger.LogInformation($"Retrieving subcontractor declaration details for application {request.ApplicationId}");

            var model = new SubcontractorDeclarationViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.SubcontractorDeclaration, request.UserName);

            var subcontractorDeclaration = await _experienceAndAccreditationApiClient.GetSubcontractorDeclaration(request.ApplicationId);

            model.HasDeliveredTrainingAsSubcontractor = subcontractorDeclaration.HasDeliveredTrainingAsSubcontractor;
            model.ContractFileName = subcontractorDeclaration.ContractFileName;

            return model;
        }

        public async Task<FileStreamResult> GetSubcontractorDeclarationContractFile(GetSubcontractorDeclarationContractFileRequest request)
        {
            return await _experienceAndAccreditationApiClient.GetSubcontractorDeclarationContractFile(request.ApplicationId);
        }

        public async Task<OfficeForStudentsViewModel> GetOfficeForStudentsViewModel(GetOfficeForStudentsRequest request)
        {
            _logger.LogInformation($"Retrieving office for students details for application {request.ApplicationId}");

            var model = new OfficeForStudentsViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.OfficeForStudents, request.UserName);

            model.IsOrganisationFundedByOfficeForStudents = await _experienceAndAccreditationApiClient.GetOfficeForStudents(request.ApplicationId) == "Yes";

            return model;
        }

        public async Task<InitialTeacherTrainingViewModel> GetInitialTeacherTrainingViewModel(GetInitialTeacherTrainingRequest request)
        {
            _logger.LogInformation($"Retrieving initial teacher training details for application {request.ApplicationId}");

            var model = new InitialTeacherTrainingViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.InitialTeacherTraining, request.UserName);

            var initialTeacherTraining = await _experienceAndAccreditationApiClient.GetInitialTeacherTraining(request.ApplicationId);

            model.DoesOrganisationOfferInitialTeacherTraining = initialTeacherTraining.DoesOrganisationOfferInitialTeacherTraining;
            model.IsPostGradOnlyApprenticeship = initialTeacherTraining.IsPostGradOnlyApprenticeship;

            return model;
        }

        public async Task<OfstedDetailsViewModel> GetOfstedDetailsViewModel(GetOfstedDetailsRequest request)
        {
            _logger.LogInformation($"Retrieving ofsted details for application {request.ApplicationId}");

            var model = new OfstedDetailsViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.Ofsted, request.UserName);

            var ofstedDetails = await _experienceAndAccreditationApiClient.GetOfstedDetails(request.ApplicationId);

            model.FullInspectionApprenticeshipGrade = ofstedDetails.FullInspectionApprenticeshipGrade;
            model.FullInspectionOverallEffectivenessGrade = ofstedDetails.FullInspectionOverallEffectivenessGrade;
            model.GradeWithinTheLast3Years = ofstedDetails.GradeWithinTheLast3Years;
            model.HasHadFullInspection = ofstedDetails.HasHadFullInspection;
            model.HasHadMonitoringVisit = ofstedDetails.HasHadMonitoringVisit;
            model.HasHadShortInspectionWithinLast3Years = ofstedDetails.HasHadShortInspectionWithinLast3Years;
            model.HasMaintainedFullGradeInShortInspection = ofstedDetails.HasMaintainedFullGradeInShortInspection;
            model.HasMaintainedFundingSinceInspection = ofstedDetails.HasMaintainedFundingSinceInspection;
            model.ReceivedFullInspectionGradeForApprenticeships = ofstedDetails.ReceivedFullInspectionGradeForApprenticeships;

            return model;
        }
    }
}

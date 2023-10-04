using SFA.DAS.AdminService.Web.Domain.Apply;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApiClient _apiApiClient;
        private readonly IApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public ApplicationService(IApiClient apiClient, IApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient)
        {
            _apiApiClient = apiClient;
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
        }

        public async Task<WithdrawalApplicationDetails> GetWithdrawalApplicationDetails(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var applicationData = await _qnaApiClient.GetApplicationDataDictionary(application?.ApplicationId ?? Guid.Empty);
            var organisation = await _apiApiClient.GetOrganisation(application?.OrganisationId ?? Guid.Empty);

            if (application is null || applicationData is null || organisation is null) return null;

            return new WithdrawalApplicationDetails
            {
                ApplicationId = application.Id,
                EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                StandardCode = application.StandardCode,
                ConfirmedWithdrawalDate = (DateTime)applicationData[nameof(ApplicationData.ConfirmedWithdrawalDate)]
            };
        }

        public async Task<ApplicationDetails> GetApplicationsDetails(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var applicationData = await _qnaApiClient.GetApplicationDataDictionary(application?.ApplicationId ?? Guid.Empty);

            var organisation = await _apiApiClient.GetOrganisation(application?.OrganisationId ?? Guid.Empty);

            var organisationContacts = await _apiApiClient.GetOrganisationContacts(organisation?.Id ?? Guid.Empty);
            var applyingContact = organisationContacts?.FirstOrDefault(c => c.Id.ToString().Equals(application?.CreatedBy, StringComparison.InvariantCultureIgnoreCase));

            if (application is null || applicationData is null || organisation is null || applyingContact is null) return null;

            return new ApplicationDetails
            {
                ApplicationResponse = application,
                ApplicationData = applicationData,
                Organisation = organisation,
                OrganisationContacts = organisationContacts,
                ApplyingContact = applyingContact
            };
        }
    }
}

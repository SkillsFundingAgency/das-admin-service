using AutoMapper;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApiClient _apiClient;
        private readonly IApplicationApiClient _applyApiClient;
        private IAnswerService _answerService;
        private IAnswerInjectionService _answerInjectionService;

        public ApplicationService(IApiClient apiClient, IApplicationApiClient applyApiClient, IAnswerService answerService, IAnswerInjectionService answerInjectionService)
        {
            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _answerService = answerService;
            _answerInjectionService = answerInjectionService;
        }

        public async Task<Models.Apply.Application> GetApplication(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);

            return Mapper.Map<Models.Apply.Application>(application);
        }

        public async Task<Organisation> GetOrganisation(Guid organisationId)
        {
            return await _apiClient.GetOrganisation(organisationId);
        }

        public async Task<List<string>> ApproveApplication(Guid applicationId)
        {
            var application = await GetApplication(applicationId);

            var warningMessages = new List<string>();

            if (application.RequiresFinancialApproval)
            {
                //_logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {application.Id} - Sequence One IS REQUIRED.");
                var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

                if (!organisation.OrganisationData.RoEPAOApproved)
                {
                    // _logger.LogInformation($"Attempting to inject organisation into register for application {applicationId}");
                    var command = await _answerService.GatherAnswersForOrganisationAndContactForApplication(application.Id);

                    var response = await _answerInjectionService.InjectApplyOrganisationAndContactDetailsIntoRegister(command);

                    warningMessages.AddWarningMessages(response.WarningMessages);
                }
            }

            if (!warningMessages.Any())
            {
                // _logger.LogInformation($"Attempting to inject standard into register for application {applicationId}");
                var command = await _answerService.GatherAnswersForOrganisationStandardForApplication(application.Id);
                var response = await _answerInjectionService.InjectApplyOrganisationStandardDetailsIntoRegister(command);

                warningMessages.AddWarningMessages(response.WarningMessages);
            }

            return warningMessages;
        }

        public async Task ReturnApplicationSequence(Guid applicationId, int sequenceNo, string returnType, string username)
        {
            await _applyApiClient.ReturnApplicationSequence(applicationId, sequenceNo, returnType, username);
        }
    }
}

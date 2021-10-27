using MediatR;
using SFA.DAS.AdminService.Web.Domain.Apply;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Commands.ApproveStandardApplication
{
    public class ApproveStandardApplicationHandler : IRequestHandler<ApproveStandardApplicationCommand, ApproveStandardApplicationResponse>
    {
        private IApplicationService _applicationService;

        public ApproveStandardApplicationHandler(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public async Task<ApproveStandardApplicationResponse> Handle(ApproveStandardApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationService.GetApplication(request.ApplicationId);

            if (application.ActiveSequence is null || application.ActiveSequence.SequenceNo != request.SequenceNo || application.ActiveSequence.Sections.Any(s => s.Status != ApplicationSectionStatus.Evaluated && !s.NotRequired))
            {
                throw new InvalidOperationException();
            }

            var errorMessages = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(returnType))
            {
                errorMessages["ReturnType"] = "Please state what you would like to do next";
            }

            if (errorMessages.Any())
            {
                
            }

            var organisation = await _applicationService.GetOrganisation(application.OrganisationId);
 
            var versions = application.ApplyData?.Apply?.Versions;
            var standardDescription = application.GetStandardDescriptionWithVersions(versions, request.SequenceNo);

            var approveResponse = new ApproveStandardApplicationResponse
            {
                Versions = versions,
                EndPointAssessorName = organisation.EndPointAssessorName,
                StandardDescription = standardDescription,
                WarningMessages = new List<string>()
            };

            if (request.SequenceNo == ApplyConst.STANDARD_SEQUENCE_NO && request.ReturnType == ReturnTypes.Approve)
            {
                var response = await _applicationService.ApproveApplication(request.ApplicationId);

                approveResponse.WarningMessages.AddWarningMessages(response);
            }

            return approveResponse;
        }

    }
}

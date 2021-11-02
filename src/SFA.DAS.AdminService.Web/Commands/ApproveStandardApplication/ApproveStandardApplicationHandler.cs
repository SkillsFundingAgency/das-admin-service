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

            var organisation = await _applicationService.GetOrganisation(application.OrganisationId);

            var response = new ApproveStandardApplicationResponse
            {
                Application = application,
                Versions = application.Versions,
                EndPointAssessorName = organisation.EndPointAssessorName,
                StandardDescription = application.GetStandardDescriptionWithVersions(application.Versions, request.SequenceNo),
                WarningMessages = new List<string>(),
                ErrorMessages = new Dictionary<string, string>()
            };

            if (string.IsNullOrWhiteSpace(request.ReturnType))
            {
                response.ErrorMessages["ReturnType"] = "Please state what you would like to do next";
            }

            if (!response.ErrorMessages.Any())
            {
                if (request.SequenceNo == ApplyConst.STANDARD_SEQUENCE_NO && request.ReturnType == ReturnTypes.Approve)
                {
                    var approveResponse = await _applicationService.ApproveApplication(application);

                    response.WarningMessages.AddWarningMessages(approveResponse);
                }
            }

            return response;
        }

    }
}

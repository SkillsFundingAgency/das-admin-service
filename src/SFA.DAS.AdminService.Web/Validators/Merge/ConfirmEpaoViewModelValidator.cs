using FluentValidation;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Validators.Merge
{
    public class ConfirmEpaoViewModelValidator : AbstractValidator<ConfirmEpaoViewModel>
    {
        private readonly IMergeOrganisationSessionService _mergeSessionService;
        private readonly IMergeOrganisationsApiClient _mergeOrganisationsApiClient;

        public ConfirmEpaoViewModelValidator(IMergeOrganisationSessionService mergeSessionService, IMergeOrganisationsApiClient mergeOrganisationsApiClient)
        {
            _mergeSessionService = mergeSessionService;
            _mergeOrganisationsApiClient = mergeOrganisationsApiClient;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var mergeRequest = _mergeSessionService.GetMergeRequest();

                if (vm.MergeOrganisationType == "primary")
                {
                    var secondaryEpao = mergeRequest.SecondaryEpao;

                    if (secondaryEpao?.Id == vm.EpaoId)
                    {
                        context.AddFailure("Epao", "The primary EPAO cannot be the same as the secondary EPAO.");
                    }
                }
                else if (vm.MergeOrganisationType == "secondary")
                {
                    var primaryEpao = mergeRequest.PrimaryEpao;

                    if (primaryEpao?.Id == vm.EpaoId)
                    {
                        context.AddFailure("Epao", "The secondary EPAO cannot be the same as the primary EPAO.");
                    }

                    var secondaryEpaoPreviousMerges = _mergeOrganisationsApiClient.GetMergeLog(new GetMergeLogRequest { SecondaryEPAOId = vm.EpaoId }).Result;

                    if (secondaryEpaoPreviousMerges != null && secondaryEpaoPreviousMerges.Items.Count > 0)
                    {
                        context.AddFailure("Epao", "Secondary EPAO has previously been merged.");
                    }
                }
            });
        }
    }
}

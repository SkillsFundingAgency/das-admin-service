using FluentValidation;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;

namespace SFA.DAS.AdminService.Web.Validators.Merge
{
    public class ConfirmEpaoViewModelValidator : AbstractValidator<ConfirmEpaoViewModel>
    {
        private IMergeOrganisationSessionService _mergeSessionService;

        public ConfirmEpaoViewModelValidator(IMergeOrganisationSessionService mergeSessionService)
        {
            _mergeSessionService = mergeSessionService;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var mergeRequest = _mergeSessionService.GetMergeRequest();

                if (vm.OrganisationType == "primary")
                {
                    var secondaryEpao = mergeRequest.SecondaryEpao;

                    if (secondaryEpao?.Id == vm.EpaoId)
                    {
                        context.AddFailure("Epao", "The primary EPAO cannot be the same as the secondary EPAO.");
                    }
                }
                else if (vm.OrganisationType == "secondary")
                {
                    var primaryEpao = mergeRequest.PrimaryEpao;

                    if (primaryEpao?.Id == vm.EpaoId)
                    {
                        context.AddFailure("Epao", "The secondary EPAO cannot be the same as the primary EPAO.");
                    }
                }
            });
        }
    }
}

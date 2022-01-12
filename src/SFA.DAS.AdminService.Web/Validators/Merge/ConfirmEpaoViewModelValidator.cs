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
                if (vm.OrganisationType == "primary")
                {
                    var secondaryEpao = _mergeSessionService.GetSecondaryEpao();

                    if (secondaryEpao?.Id == vm.EpaoId)
                    {
                        context.AddFailure("Epao", "The primary EPAO cannot be the same as the secondary EPAO.");
                    }
                }
                else if (vm.OrganisationType == "secondary")
                {
                    var primaryEpao = _mergeSessionService.GetPrimaryEpao();

                    if (primaryEpao?.Id == vm.EpaoId)
                    {
                        context.AddFailure("Epao", "The secondary EPAO cannot be the same as the primary EPAO.");
                    }
                }
            });
        }
    }
}

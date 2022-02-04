using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Merge;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class MergeOverviewViewModelValidator : AbstractValidator<MergeOverviewViewModel>
    {
        public MergeOverviewViewModelValidator()
        {
            RuleFor(vm => vm.PrimaryEpaoId).NotNull().WithMessage("Confirm a primary EPAO");

            RuleFor(vm => vm.SecondaryEpaoId).NotNull().WithMessage("Confirm a secondary EPAO");

            RuleFor(vm => vm.SecondaryEpaoEffectiveTo).NotNull().WithMessage("Confirm secondary EPAO effective to");
        }
    }
}

using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AdminService.Web.ViewModels;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateOptionViewModelValidator : AbstractValidator<CertificateOptionViewModel>
    {
        public CertificateOptionViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (vm.HasAdditionalLearningOption == null)
                {
                    context.AddFailure("HasAdditionalLearningOption", "Select yes or no");
                }

                if (vm.HasAdditionalLearningOption.HasValue && vm.HasAdditionalLearningOption.Value == true && string.IsNullOrWhiteSpace(vm.Option))
                {
                    context.AddFailure("Option", "Enter the learning option");
                }
            });
        }
    }
}
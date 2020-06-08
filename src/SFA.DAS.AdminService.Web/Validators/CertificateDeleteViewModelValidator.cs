using FluentValidation;
using System;
using SFA.DAS.AdminService.Web.ViewModels.CertificateDelete;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.Applications
{
    public class CertificateDeleteViewModelValidator : AbstractValidator<CertificateAuditDetailsViewModel>
    {
        public CertificateDeleteViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (HasExceededWordCount(vm.ReasonForChange))
                {
                    context.AddFailure("ReasonForChange", "Reason for deleting this certificate must be 500 words or less");
                }
            });
        }

        private static bool HasExceededWordCount(string input, int maxWordcount = 500)
        {
            bool hasExceeded = false;

            var text = input?.Trim();

            if (!string.IsNullOrEmpty(text))
            {
                var wordCount = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;

                hasExceeded = (wordCount > maxWordcount);
            }

            return hasExceeded;
        }
    }
}

using FluentValidation;
using System;
using System.Text.RegularExpressions;
using SFA.DAS.AdminService.Web.ViewModels.CertificateDelete;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.Applications
{
    public class CertificateDeleteViewModelValidator : AbstractValidator<CertificateAuditDetailsViewModel>
    {
        public CertificateDeleteViewModelValidator()
        {
            RuleFor(vm => vm.IncidentNumber).NotEmpty().WithMessage("Enter a ticket or incident number");
            RuleFor(vm => vm.ReasonForChange).NotEmpty().WithMessage("Enter a reason for deleting this certificate");
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (HasExceededWordCount(vm?.ReasonForChange))
                {
                    context.AddFailure("ReasonForChange", "Reason for deleting this certificate must be 500 words or less");
                }
            });
        }

        private static bool HasExceededWordCount(string input, int maxWordcount = 500)
        {
            bool hasExceeded = false;

            if (!string.IsNullOrEmpty(input))
            {
                var wordCount = CountWords(input);
                hasExceeded = (wordCount > maxWordcount);
            }
            return hasExceeded;
        }

        public static int CountWords(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }
    }
}

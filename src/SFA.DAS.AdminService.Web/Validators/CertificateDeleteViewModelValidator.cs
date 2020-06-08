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
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (vm?.ReasonForChange is null || string.IsNullOrWhiteSpace(vm.ReasonForChange))
                {
                    context.AddFailure("ReasonForChange", "Enter a reason for deleting this certificate");
                }
                if (vm?.IncidentNumber is null || string.IsNullOrWhiteSpace(vm.IncidentNumber))
                {
                    context.AddFailure("IncidentNumber", "Enter a ticket or incident number");
                }
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

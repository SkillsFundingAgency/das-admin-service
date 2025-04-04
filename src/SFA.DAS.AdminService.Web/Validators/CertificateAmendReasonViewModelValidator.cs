using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateAmendReasonViewModelValidator : AbstractValidator<CertificateAmendReasonViewModel>
    {
        public CertificateAmendReasonViewModelValidator()
        {
            RuleFor(vm => vm.IncidentNumber).NotEmpty().WithMessage("Enter the ticket reference");

            RuleFor(vm => vm.Reasons).NotEmpty().WithMessage("Select reason(s) for amending certificate information");

            When(vm => vm.Reasons != null && vm.Reasons.Contains("Other"), () =>
            {
                RuleFor(vm => vm.OtherReason).
                    NotEmpty().
                    WithMessage("Give details");

                RuleFor(vm => vm.OtherReason)
                    .MaximumLength(200)
                    .WithMessage("Details must be 200 characters or fewer");
            });
        } 
    }
}
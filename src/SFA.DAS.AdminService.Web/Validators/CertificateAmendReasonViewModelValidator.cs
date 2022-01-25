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

            RuleFor(vm => vm.OtherReason).NotEmpty().When(vm => vm.Reasons != null && vm.Reasons.Contains("Other")).WithMessage("Give details");
        } 
    }
}
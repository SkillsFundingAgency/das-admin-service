using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateReprintReasonViewModelValidator : AbstractValidator<CertificateReprintReasonViewModel>
    {
        public CertificateReprintReasonViewModelValidator()
        {
            RuleFor(vm => vm.IncidentNumber).NotEmpty().WithMessage("Enter the ticket reference");

            RuleFor(vm => vm.Reasons).NotEmpty().WithMessage("Select reason(s) for requesting a certificate reprint");

            RuleFor(vm => vm.OtherReason).NotEmpty().When(vm => vm.Reasons != null && vm.Reasons.Contains("Other")).WithMessage("Give details");
        } 
    }
}
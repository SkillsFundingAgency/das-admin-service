using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Search;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class AmendFrameworkReprintReasonViewModelValidator : AbstractValidator<FrameworkLearnerAmendReprintReasonViewModel>
    {
        public AmendFrameworkReprintReasonViewModelValidator()
        {
             RuleFor(vm => vm.TicketNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Enter the ticket reference number")
                .MaximumLength(20)
                    .WithMessage("Ticket reference number must be 20 characters or fewer")
                .Matches("^[a-zA-Z0-9]*$")
                    .WithMessage("Enter only alphanumeric characters");

            RuleFor(vm => vm.SelectedReprintReasons)
                .NotEmpty()
                .WithMessage("Select reasons for requesting a reprint");

            When(vm => vm.SelectedReprintReasons != null && vm.SelectedReprintReasons.Contains("Other"), () =>
            {
                RuleFor(vm => vm.OtherReason)
                    .NotEmpty()
                    .WithMessage("Give details")
                    .MaximumLength(200)
                    .WithMessage("Details must be 200 characters or fewer");
            });
        }
    }
}

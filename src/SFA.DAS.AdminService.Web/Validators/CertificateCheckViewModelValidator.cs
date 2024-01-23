using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateCheckViewModelValidator : AbstractValidator<CertificateCheckViewModel>
    {
        public CertificateCheckViewModelValidator(IStandardVersionApiClient standardVersionApiClient)
        {
            When(vm => vm.Status != CertificateStatus.Draft, () => 
            {
                RuleFor(vm => vm).Custom((vm, context) =>
                {
                    var options = standardVersionApiClient.GetStandardOptions(vm.GetStandardId()).Result;
                    if (options != null && options.HasOptions())
                    {
                        if (string.IsNullOrWhiteSpace(vm.Option))
                        {
                            context.AddFailure(nameof(vm.Option), "Add an option");
                        }
                    }
                });

                When(vm => vm.SelectedGrade != null && vm.SelectedGrade != CertificateGrade.Fail, () =>
                {
                    var addressCannotBeEmpty = "Enter an address";

                    RuleFor(vm => vm.SendTo).NotEqual(CertificateSendTo.None)
                        .WithMessage("Enter the certificate receiver");

                    RuleFor(vm => vm.Name).NotEmpty()
                        .WithMessage("You need to give a name of who will receive the certificate");

                    When(vm => vm.SendTo == CertificateSendTo.Employer, () =>
                    {
                        RuleFor(vm => vm.Employer).NotEmpty()
                            .OverridePropertyName(vm => vm.AddressLine1)
                            .WithMessage(addressCannotBeEmpty);
                    });

                    RuleFor(vm => vm.AddressLine1).NotEmpty()
                        .WithMessage(addressCannotBeEmpty);

                    RuleFor(vm => vm.City).NotEmpty()
                        .OverridePropertyName(vm => vm.AddressLine1)
                        .WithMessage(addressCannotBeEmpty);

                    RuleFor(vm => vm.Postcode).NotEmpty()
                        .OverridePropertyName(vm => vm.AddressLine1)
                        .WithMessage(addressCannotBeEmpty);
                });
            });
        }
    }
}
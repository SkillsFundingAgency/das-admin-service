using System;
using System.Linq;
using FluentValidation;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AssessorService.ExternalApis;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateUkprnViewModelValidator : AbstractValidator<CertificateUkprnViewModel>
    {
        private readonly IRoatpApiClient _apiClient;

        public CertificateUkprnViewModelValidator(
            IRoatpApiClient apiClient)
        {
            _apiClient = apiClient;
            RuleFor(vm => vm.Ukprn).NotEmpty()
                .WithMessage("Enter the training provider's UKPRN").DependentRules(() =>
            {
                RuleFor(vm => vm.Ukprn).Must(BeANumber).WithMessage("The UKPRN should contain 8 numbers").DependentRules(() =>
                {
                    RuleFor(vm => vm.Ukprn).Length(8).WithMessage("The UKPRN should contain 8 numbers").DependentRules(() =>
                    {
                        RuleFor(vm => vm.Ukprn).Must(UkprnMustExist).WithMessage("This UKPRN cannot be found");
                    });
                });
            });
        }

        private bool BeANumber(string number)
        {
            return int.TryParse(number, out int _);
        }

        private bool UkprnMustExist(string ukprn)
        {
            try
            {
                var providerUkprn = Convert.ToInt32(ukprn);
                var result = _apiClient.Search(providerUkprn.ToString()).GetAwaiter().GetResult().SearchResults.FirstOrDefault();
                if (result == null)
                {
                    return false;
                }
            }
            catch (EntityNotFoundException e)
            {
                return false;
            }            

            return true;
        }
    }
}

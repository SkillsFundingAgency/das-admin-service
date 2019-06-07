
namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    using SFA.DAS.AssessorService.Api.Types.Models.Validation;
    using System.Collections.Generic;

    public interface IRoatpOrganisationValidator
    {
        List<ValidationErrorDetail> IsValidLegalName(string legalName);
        List<ValidationErrorDetail> IsValidUKPRN(string ukprn);
        List<ValidationErrorDetail> IsValidTradingName(string tradingName);
        List<ValidationErrorDetail> IsValidCompanyNumber(string companyNumber);
        List<ValidationErrorDetail> IsValidCharityNumber(string charityNumber);
    }
}

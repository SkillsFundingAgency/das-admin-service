using System.Collections.Generic;
using SFA.DAS.AdminService.Common.Validation;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.Applications
{
    public class RoatpSearchTermValidator : IRoatpSearchTermValidator
    {
        private const string SearchTerm = "SearchTerm";
        private const int MinimumSearchTermLength = 3;
        private readonly string SearchTermMandatory = "Enter an organisation name or UKPRN";
        private readonly string SearchTermLength = $"Enter a UKPRN or an organisation name using {MinimumSearchTermLength} or more characters";

        public ValidationResponse Validate(string searchTerm)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(SearchTerm, SearchTermMandatory));
            }
            else if (searchTerm.Length < MinimumSearchTermLength)
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(SearchTerm, SearchTermLength));
            }

            return validationResponse;
        }
    }

    public interface IRoatpSearchTermValidator
    {
        ValidationResponse Validate(string searchTerm);
    }
}

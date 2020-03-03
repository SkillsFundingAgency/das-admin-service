using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Applications
{
    public class RoatpApplicationApprovalViewModel
    {
        public Guid ApplicationId { get; set; }
        public int OrganisationTypeId { get; set; }
        public int ProviderTypeId { get; set; }
        public string ApplicationRoute { get; set; }
        public string LegalName { get; set; }
        public string UKPRN { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public string TradingName { get; set; }
        public DateTime? ApplicationDeterminedDate
        {
            get
            {
                var yearWithCentury = DeterminedDateYear;
                if (yearWithCentury != null && yearWithCentury <= 99)
                    yearWithCentury += 2000;

                var formatStrings = new string[] { "d/M/yyyy" };
                if (!DateTime.TryParseExact($"{DeterminedDateDay}/{DeterminedDateMonth}/{yearWithCentury}", formatStrings, null, DateTimeStyles.None,
                    out DateTime formattedDate))
                {
                    return null;
                }
                return formattedDate;
            }
        }

        public IEnumerable<OrganisationType> OrganisationTypes { get; set; }

        public string OrganisationTypeDescription
        {
            get
            {
                if (OrganisationTypes == null || !OrganisationTypes.Any())
                {
                    return string.Empty;
                }

                var organisationType = OrganisationTypes.FirstOrDefault(x => x.Id == OrganisationTypeId);

                return organisationType?.Type;
            }
        }

        public int? DeterminedDateDay { get; set; }
        public int? DeterminedDateMonth { get; set; }
        public int? DeterminedDateYear { get; set; }

        public string Username { get; set; }

        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        public bool IsError => ErrorMessages != null && ErrorMessages.Count > 0;

        public bool IsErrorDay => IsError && (ErrorMessages.Any(x => x.Field == "DeterminedDateDay"));
        public bool IsErrorMonth => IsError && (ErrorMessages.Any(x => x.Field == "DeterminedDateMonth"));
        public bool IsErrorYear => IsError && (ErrorMessages.Any(x => x.Field == "DeterminedDateYear"));
    }
}

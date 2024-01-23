using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Application.Commands
{
    public class CreateOrganisationContactCommand
    {
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationType { get; set; }
        public int? OrganisationUkprn { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public bool? IsRoEpaoApproved { get; set; }
        public string TradingName { get; set; }
        public bool UseTradingName { get; set; }
        public string ContactGivenNames { get; set; }
        public string ContactFamilyName { get; set; }
        public string ContactAddress1 { get; set; }
        public string ContactAddress2 { get; set; }
        public string ContactAddress3 { get; set; }
        public string ContactAddress4 { get; set; }

        public string ContactPostcode { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string CompanyUkprn { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public string StandardWebsite { get; set; }

        public DateTime? FinancialDueDate { get; set; }
        public bool? IsFinancialExempt { get; set; }

        public Guid ApplyingContactId { get; set; }
        public string ApplyingContactGivenNames { get; set; }
        public string ApplyingContactFamilyName { get; set; }
        public string ApplyingContactEmail { get; set; }

        public List<string> OtherApplyingUserEmails { get; set; }

        public CreateOrganisationContactCommand(Guid organisationId, string organisationName, string organisationType,
            int? organisationUkprn, string endPointAssessorOrganisationId, bool? isRoEpaoApproved, string tradingName,
            bool useTradingName, string contactGivenNames, string contactFamilyName, string contactAddress1, string contactAddress2,
            string contactAddress3, string contactAddress4, string contactPostcode, string contactEmail,
            string contactPhoneNumber, string companyUkprn, string companyNumber, string charityNumber,
            string standardWebsite, Guid applyingContactId, string applyingContactGivenNames, string applyingContactFamilyName, string applyingContactEmail,
            List<string> otherApplyingUserEmails, DateTime? financialDueDate, bool? isFinancialExempt)
        {
            OrganisationId = organisationId;
            OrganisationName = organisationName;
            OrganisationType = organisationType;
            OrganisationUkprn = organisationUkprn;
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
            IsRoEpaoApproved = isRoEpaoApproved;
            TradingName = tradingName;
            UseTradingName = useTradingName;
            ContactGivenNames = contactGivenNames;
            ContactFamilyName = contactFamilyName;
            ContactAddress1 = contactAddress1;
            ContactAddress2 = contactAddress2;
            ContactAddress3 = contactAddress3;
            ContactAddress4 = contactAddress4;
            ContactPostcode = contactPostcode;
            ContactEmail = contactEmail;
            ContactPhoneNumber = contactPhoneNumber;
            CompanyUkprn = companyUkprn;
            CompanyNumber = companyNumber;
            CharityNumber = charityNumber;
            StandardWebsite = standardWebsite;
            ApplyingContactId = applyingContactId;
            ApplyingContactFamilyName = applyingContactFamilyName;
            ApplyingContactGivenNames = applyingContactGivenNames;
            ApplyingContactEmail = applyingContactEmail;
            OtherApplyingUserEmails = otherApplyingUserEmails;
            FinancialDueDate = financialDueDate;
            IsFinancialExempt = isFinancialExempt;
        }

        public CreateOrganisationContactCommand()
        {
        }
    }
}

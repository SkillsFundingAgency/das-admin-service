﻿namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SFA.DAS.AssessorService.Api.Types.Models.Validation;
    using SFA.DAS.AdminService.Web.Resources;
    using SFA.DAS.AdminService.Web.Services;

    public class RoatpOrganisationValidator : IRoatpOrganisationValidator
    {
        private const string CharityNumberInvalidCharactersRegex = "[^a-zA-Z0-9\\-]";
        private const string CompaniesHouseNumberRegexNumeric = "[0-9]{8}";
        private const string CompaniesHouseNumberRegexWithPrefix = "[A-Z]{2}[0-9]{5}[A-Z0-9]{1}";
        public List<ValidationErrorDetail> IsValidLegalName(string legalName)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            legalName = TextSanitiser.SanitiseText(legalName);

            if (String.IsNullOrWhiteSpace(legalName))
            {
                errorMessages.Add(new ValidationErrorDetail("LegalName", RoatpOrganisationValidation.LegalNameMandatory));
                return errorMessages;
            }

              if (legalName.Length > 200)
            {
                errorMessages.Add(new ValidationErrorDetail("LegalName", RoatpOrganisationValidation.LegalNameMaxLength));
            }
         

            if (legalName.Length < 2)
            {
                errorMessages.Add(new ValidationErrorDetail("LegalName", RoatpOrganisationValidation.LegalNameMinLength));
            }

            return errorMessages;
        }
        
        public List<ValidationErrorDetail> IsValidUKPRN(string ukprn)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            if (String.IsNullOrWhiteSpace(ukprn))
            {
                errorMessages.Add(new ValidationErrorDetail("UKPRN", RoatpOrganisationValidation.UKPRNMandatory));
                return errorMessages;
            }

            long ukprnValue = 0;
            bool isParsed = long.TryParse(ukprn, out ukprnValue);

            if (!isParsed)
            {
                errorMessages.Add(new ValidationErrorDetail("UKPRN", RoatpOrganisationValidation.UKPRNFormat));
            }

            // TODO MFCMFC Remove this before branch UKRLP_etc gets remerged
            if (ukprnValue == 111111111111)
                return errorMessages;

            if (ukprnValue < 10000000 || ukprnValue > 99999999)
            {
                errorMessages.Add(new ValidationErrorDetail("UKPRN", RoatpOrganisationValidation.UKPRNLength));
            }

            return errorMessages;
        }

        public List<ValidationErrorDetail> IsValidTradingName(string tradingName)
        {
            tradingName = TextSanitiser.SanitiseText(tradingName);

            var errorMessages = new List<ValidationErrorDetail>();

            if (String.IsNullOrWhiteSpace(tradingName))
            {
                return errorMessages;
            }

            if (tradingName.Length > 200)
            {
                errorMessages.Add(new ValidationErrorDetail("TradingName", RoatpOrganisationValidation.TradingNameMaxLength));
            }

            return errorMessages;
        }

        public List<ValidationErrorDetail> IsValidCompanyNumber(string companyNumber)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            if (String.IsNullOrWhiteSpace(companyNumber))
            {
                return errorMessages;
            }

            if (companyNumber.Length != 8)
            {
                errorMessages.Add(new ValidationErrorDetail("CompanyNumber", RoatpOrganisationValidation.CompanyNumberLength));
            }

            if (!Regex.IsMatch(companyNumber, CompaniesHouseNumberRegexWithPrefix)
                && (!Regex.IsMatch(companyNumber, CompaniesHouseNumberRegexNumeric)))
            {
                errorMessages.Add(new ValidationErrorDetail("CompanyNumber", RoatpOrganisationValidation.CompanyNumberFormat));
            }

            return errorMessages;
        }

        public List<ValidationErrorDetail> IsValidCharityNumber(string charityNumber)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            if (String.IsNullOrWhiteSpace(charityNumber))
            {
                return errorMessages;
            }

            if (charityNumber.Length < 6 || charityNumber.Length > 14)
            {
                errorMessages.Add(new ValidationErrorDetail("CharityNumber", RoatpOrganisationValidation.CharityNumberLength));
            }

            if (Regex.IsMatch(charityNumber, CharityNumberInvalidCharactersRegex))
            {
                errorMessages.Add(new ValidationErrorDetail("CharityNumber", RoatpOrganisationValidation.CharityNumberFormat));
            }

            return errorMessages;
        }
    }
}

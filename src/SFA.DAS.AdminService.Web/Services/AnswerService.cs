using Newtonsoft.Json.Linq;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IApplicationService _applicationService;

        public AnswerService(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public async Task<CreateOrganisationContactCommand> GatherAnswersForOrganisationAndContactForApplication(Guid applicationId)
        {
            var applicationDetails = await _applicationService.GetApplicationsDetails(applicationId);
            if(applicationDetails == null) return new CreateOrganisationContactCommand();

            var applicationData = applicationDetails.ApplicationData;

            var tradingName = GetAnswer(applicationData, "trading_name");
            var useTradingNameString = GetAnswer(applicationData, "use_trading_name");
            var useTradingName = "yes".Equals(useTradingNameString, StringComparison.InvariantCultureIgnoreCase) || "true".Equals(useTradingNameString, StringComparison.InvariantCultureIgnoreCase) || "1".Equals(useTradingNameString, StringComparison.InvariantCultureIgnoreCase);

            // get a contact by their contact name parts (application started May 2019 to date)
            var contactGivenNames = GetAnswer(applicationData, "contact_given_name");
            var contactFamilyName = GetAnswer(applicationData, "contact_family_name");

            if (string.IsNullOrEmpty(contactGivenNames) && string.IsNullOrEmpty(contactFamilyName))
            {
                // get a contact by their contact name (application started before May 2019)
                var contactName = GetAnswer(applicationData, "contact_name");
                if (!string.IsNullOrEmpty(contactName))
                {
                    var matches = Regex.Matches(contactName, "^*([,'.-a-zA-z]{1,})");
                    if (matches.Count() > 0)
                    {
                        var contactNameParts = matches.Cast<Group>()
                            .Where(o => o.Value != string.Empty)
                            .Select(o => o.Value);

                        contactGivenNames = contactNameParts.Count() == 1
                            ? contactNameParts.First()
                            : string.Join(" ", contactNameParts.Take(contactNameParts.Count() - 1));

                        // using the (Unknown) placeholder for the family name in cases where only a single word was entered
                        contactFamilyName = contactNameParts.Count() == 1
                            ? "(Unknown)"
                            : contactNameParts.Last();
                    }
                }
            }

            // get a contact address which is a single question with multiple answers
            var contactAddress = GetJsonAnswer(applicationData, "contact_address");

            // handle both a contact address which is a single question with multiple answers or multiple questions with a single answer 
            var contactAddress1 = contactAddress?["AddressLine1"].ToString() ?? GetAnswer(applicationData, "contact_address1");
            var contactAddress2 = contactAddress?["AddressLine2"].ToString() ?? GetAnswer(applicationData, "contact_address2");
            var contactAddress3 = contactAddress?["AddressLine3"].ToString() ?? GetAnswer(applicationData, "contact_address3");
            var contactAddress4 = contactAddress?["AddressLine4"].ToString() ?? GetAnswer(applicationData, "contact_address4");
            var contactPostcode = contactAddress?["Postcode"].ToString() ?? GetAnswer(applicationData, "contact_postcode");

            var contactEmail = GetAnswer(applicationData, "contact_email");
            var contactPhoneNumber = GetAnswer(applicationData, "contact_phone_number");
            var companyUkprn = GetAnswer(applicationData, "company_ukprn");

            var companyNumber = GetAnswer(applicationData, "company_number");
            if ("no".Equals(companyNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                companyNumber = null;
            }

            var charityNumber = GetAnswer(applicationData, "charity_number");
            if ("no".Equals(charityNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                charityNumber = null;
            }

            var standardWebsite = GetAnswer(applicationData, "standard_website");

            var command = new CreateOrganisationContactCommand
            (applicationDetails.Organisation.Id,
                applicationDetails.Organisation.EndPointAssessorName,
                applicationDetails.Organisation.OrganisationType.Type,
                applicationDetails.Organisation.EndPointAssessorUkprn,
                applicationDetails.Organisation.EndPointAssessorOrganisationId,
                applicationDetails.Organisation.OrganisationData.RoEPAOApproved,
                tradingName,
                useTradingName,
                contactGivenNames,
                contactFamilyName,
                contactAddress1,
                contactAddress2,
                contactAddress3,
                contactAddress4,
                contactPostcode,
                contactEmail,
                contactPhoneNumber,
                companyUkprn,
                companyNumber,
                charityNumber,
                standardWebsite,
                applicationDetails.ApplyingContact.Id,
                applicationDetails.ApplyingContact.GivenNames,
                applicationDetails.ApplyingContact.FamilyName,
                applicationDetails.ApplyingContact.Email,
                applicationDetails.OrganisationContacts.Where(c => c.Email != applicationDetails.ApplyingContact.Email).Select(c => c.Email).ToList(),
                applicationDetails.Organisation.OrganisationData?.FHADetails?.FinancialDueDate,
                applicationDetails.Organisation.OrganisationData?.FHADetails?.FinancialExempt);

            return command;
        }

        public async Task<CreateOrganisationStandardCommand> GatherAnswersForOrganisationStandardForApplication(Guid applicationId)
        {
            var applicationDetails = await _applicationService.GetApplicationsDetails(applicationId);
            if (applicationDetails == null) return new CreateOrganisationStandardCommand();

            var applicationData = applicationDetails.ApplicationData;

            var effectiveFrom = DateTime.UtcNow.Date;
            if (DateTime.TryParse(GetAnswer(applicationData, "effective_from"), out var effectiveFromDate))
            {
                effectiveFrom = effectiveFromDate.Date;
            }

            var deliveryAreas = GetAnswer(applicationData, "delivery_areas");

            var command = new CreateOrganisationStandardCommand
            (
                applicationDetails.Organisation.Id,
                applicationDetails.Organisation.EndPointAssessorOrganisationId,
                applicationDetails.ApplicationResponse.StandardCode ?? 0,
                applicationDetails.ApplicationResponse.ApplyData?.Apply?.StandardReference,
                applicationDetails.ApplicationResponse.ApplyData?.Apply?.Versions,
                effectiveFrom,
                deliveryAreas?.Split(',').ToList(),
                applicationDetails.ApplyingContact.Id,
                applicationDetails.ApplicationResponse.StandardApplicationType);

            return command;
        }

        public async Task<WithdrawOrganisationRequest> GatherAnswersForWithdrawOrganisationForApplication(Guid applicationId, string updatedBy)
        {
            var withdrawalApplicationDetails = await _applicationService.GetWithdrawalApplicationDetails(applicationId);
            if (withdrawalApplicationDetails is null) return null;

            return new WithdrawOrganisationRequest
            {
                ApplicationId = withdrawalApplicationDetails.ApplicationId,
                EndPointAssessorOrganisationId = withdrawalApplicationDetails.EndPointAssessorOrganisationId,
                WithdrawalDate = withdrawalApplicationDetails.ConfirmedWithdrawalDate,
                UpdatedBy = updatedBy
            };
        }

        public async Task<WithdrawStandardRequest> GatherAnswersForWithdrawStandardForApplication(Guid applicationId)
        {
            var withdrawalApplicationDetails = await _applicationService.GetWithdrawalApplicationDetails(applicationId);
            if(withdrawalApplicationDetails is null) return null;

            return new WithdrawStandardRequest
            {
                EndPointAssessorOrganisationId = withdrawalApplicationDetails.EndPointAssessorOrganisationId,
                StandardCode = withdrawalApplicationDetails.StandardCode.GetValueOrDefault(),
                WithdrawalDate = withdrawalApplicationDetails.ConfirmedWithdrawalDate
            };
        }

        private string GetAnswer(Dictionary<string, object> applicationData, string questionTag)
        {
            return applicationData.ContainsKey(questionTag) ? applicationData[questionTag]?.ToString() : null;
        }

        private JObject GetJsonAnswer(Dictionary<string, object> applicationData, string questionTag)
        {
            try
            {
                var json = GetAnswer(applicationData, questionTag);
                return JObject.Parse(json);
            }
            catch
            {
                return default(JObject);
            }
        }
    }
}

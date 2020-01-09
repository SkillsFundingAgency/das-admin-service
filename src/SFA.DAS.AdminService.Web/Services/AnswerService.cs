using Newtonsoft.Json.Linq;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IApiClient _apiApiClient;
        private readonly IApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;


        public AnswerService(IApiClient apiClient, IApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient)
        {
            _apiApiClient = apiClient;
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
        }

        public async Task<CreateOrganisationContactCommand> GatherAnswersForOrganisationAndContactForApplication(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var applicationData = await _qnaApiClient.GetApplicationData(application?.ApplicationId ?? Guid.Empty);

            var organisation = await _apiApiClient.GetOrganisation(application?.OrganisationId ?? Guid.Empty);

            var organisationContacts = await _apiApiClient.GetOrganisationContacts(organisation?.Id ?? Guid.Empty);
            var applyingContact = organisationContacts?.FirstOrDefault(c => c.Id.ToString().Equals(application?.CreatedBy, StringComparison.InvariantCultureIgnoreCase));

            if (application is null || applicationData is null || organisation is null || applyingContact is null) return new CreateOrganisationContactCommand();

            var tradingName = GetAnswer(applicationData, "trading_name");
            var useTradingNameString = GetAnswer(applicationData, "use_trading_name");
            var useTradingName = "yes".Equals(useTradingNameString, StringComparison.InvariantCultureIgnoreCase) || "true".Equals(useTradingNameString, StringComparison.InvariantCultureIgnoreCase) || "1".Equals(useTradingNameString, StringComparison.InvariantCultureIgnoreCase);

            var contactGivenNames = GetAnswer(applicationData, "contact_given_name");
            var contactFamilyName = GetAnswer(applicationData, "contact_family_name");

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
            (   organisation.Id,
                organisation.EndPointAssessorName,
                organisation.OrganisationType.Type,
                organisation.EndPointAssessorUkprn,
                organisation.EndPointAssessorOrganisationId,
                organisation.OrganisationData.RoEPAOApproved,
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
                applyingContact.Id,
                applyingContact.GivenNames,
                applyingContact.FamilyName,
                applyingContact.Email,
                organisationContacts.Where(c => c.Email != applyingContact.Email).Select(c => c.Email).ToList(),
                organisation.OrganisationData?.FHADetails?.FinancialDueDate,
                organisation.OrganisationData?.FHADetails?.FinancialExempt);


            return command;
        }

        public async Task<CreateOrganisationStandardCommand> GatherAnswersForOrganisationStandardForApplication(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var applicationData = await _qnaApiClient.GetApplicationData(application?.ApplicationId ?? Guid.Empty);

            var organisation = await _apiApiClient.GetOrganisation(application?.OrganisationId ?? Guid.Empty);
            var organisationContacts = await _apiApiClient.GetOrganisationContacts(organisation?.Id ?? Guid.Empty);
            var applyingContact = organisationContacts?.FirstOrDefault(c => c.Id.ToString().Equals(application?.CreatedBy, StringComparison.InvariantCultureIgnoreCase));

            if (application is null || applicationData is null || organisation is null || applyingContact is null) return new CreateOrganisationStandardCommand();

            var effectiveFrom = DateTime.UtcNow.Date;
            if(DateTime.TryParse(GetAnswer(applicationData, "effective_from"), out var effectiveFromDate))
            {
                effectiveFrom = effectiveFromDate.Date;
            }
                
            var deliveryAreas = GetAnswer(applicationData, "delivery_areas");
                
            var command = new CreateOrganisationStandardCommand
            (   
                organisation.Id,
                organisation.EndPointAssessorOrganisationId,
                application.StandardCode ?? 0,
                effectiveFrom,
                deliveryAreas?.Split(',').ToList(),
                applyingContact.Id);

            return command;
        }

        private string GetAnswer(Dictionary<string, object> applicationData, string questionTag)
        {
            return applicationData.ContainsKey(questionTag) ? applicationData[questionTag].ToString() : null;
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


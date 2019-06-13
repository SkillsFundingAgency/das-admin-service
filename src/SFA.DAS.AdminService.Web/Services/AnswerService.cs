using Newtonsoft.Json.Linq;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AdminService.Web.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IApiClient _assessorApiClient;


        public AnswerService(IApplyApiClient applyApiClient, IApiClient assessorApiClient)
        {
            _applyApiClient = applyApiClient;
            _assessorApiClient = assessorApiClient;
        }

        public async Task<CreateOrganisationContactCommand> GatherAnswersForOrganisationAndContactForApplication(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = application?.ApplyingOrganisation ?? await _applyApiClient.GetOrganisationForApplication(applicationId);
            var organisationContacts = await _applyApiClient.GetOrganisationContacts(organisation?.Id ?? Guid.Empty);
            var applyingContact = organisationContacts?.FirstOrDefault(c => c.Id.ToString().Equals(application?.CreatedBy, StringComparison.InvariantCultureIgnoreCase));

            if (application is null || organisation is null || applyingContact is null) return new CreateOrganisationContactCommand();

            var tradingName = await GetAnswer(application.Id, "trading-name");
            var useTradingNameString = await GetAnswer(application.Id, "use-trading-name");
            var useTradingName = "yes".Equals(useTradingNameString, StringComparison.InvariantCultureIgnoreCase) || "true".Equals(useTradingNameString, StringComparison.InvariantCultureIgnoreCase) || "1".Equals(useTradingNameString, StringComparison.InvariantCultureIgnoreCase);

            var contactName = await GetAnswer(application.Id, "contact-name");
            var contactGivenName = await GetAnswer(application.Id, "contact-given-name");
            var contactFamilyName = await GetAnswer(application.Id, "contact-family-name");

            // get a contact address which is a single question with multiple answers
            JProperty contactAddress = null;

            var contactAddressJsonAnswer = await GetJsonAnswer(application.Id, "contact-address");
            if (contactAddressJsonAnswer != null)
            {
                contactAddress = JObject.Parse(contactAddressJsonAnswer).First is JProperty
                    ? JObject.Parse(contactAddressJsonAnswer).First as JProperty
                    : null;
            }

            // handle both a contact address which is a single question with multiple answers or multiple questions with a single answer 
            var contactAddress1 = contactAddress?.Value["AddressLine1"].ToString() ?? await GetAnswer(application.Id, "contact-address1");
            var contactAddress2 = contactAddress?.Value["AddressLine2"].ToString() ?? await GetAnswer(application.Id, "contact-address2");
            var contactAddress3 = contactAddress?.Value["TownOrCity"].ToString() ?? await GetAnswer(application.Id, "contact-address3");
            var contactAddress4 = contactAddress?.Value["County"].ToString() ?? await GetAnswer(application.Id, "contact-address4");
            var contactPostcode = contactAddress?.Value["Postcode"].ToString() ?? await GetAnswer(application.Id, "contact-postcode");

            var contactEmail = await GetAnswer(application.Id, "contact-email");
            var contactPhoneNumber = await GetAnswer(application.Id, "contact-phone-number");
            var companyUkprn = await GetAnswer(application.Id, "company-ukprn");

            var companyNumber = await GetAnswer(application.Id, "company-number");
            if ("no".Equals(companyNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                companyNumber = null;
            }

            var charityNumber = await GetAnswer(application.Id, "charity-number");
            if ("no".Equals(charityNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                charityNumber = null;
            }

            var standardWebsite = await GetAnswer(application.Id, "standard-website");
          
            var command = new CreateOrganisationContactCommand
            (   organisation.Name,
                organisation.OrganisationType,
                organisation.OrganisationUkprn?.ToString(),
                organisation.OrganisationDetails?.OrganisationReferenceType,
                organisation.RoEPAOApproved,
                tradingName,
                useTradingName,
                contactName,
                contactGivenName,
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
                applyingContact.Id.ToString(),
                applyingContact.FamilyName,
                applyingContact.GivenNames,
                applyingContact.SigninId,
                applyingContact.SigninType,
                applyingContact.Email,
                organisationContacts.Where(c => c.Email != applyingContact.Email).Select(c => c.Email).ToList(),
                organisation.OrganisationDetails?.FHADetails?.FinancialDueDate,
                organisation.OrganisationDetails?.FHADetails?.FinancialExempt);


            return command;
        }

        public async Task<CreateOrganisationStandardCommand> GatherAnswersForOrganisationStandardForApplication(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _applyApiClient.GetOrganisationForApplication(applicationId);

            if (application is null || organisation is null) return new CreateOrganisationStandardCommand();

            var assessorOrganisation = (await _assessorApiClient.SearchOrganisations(organisation.Name)).FirstOrDefault();

            var organisationId = assessorOrganisation?.Id;
            var createdBy = application.CreatedBy ?? organisation.CreatedBy;
            var standardCode = application.ApplicationData?.StandardCode;

            var effectiveFrom = DateTime.UtcNow.Date;
            if(DateTime.TryParse(await GetAnswer(applicationId, "effective-from"), out var effectiveFromDate))
            {
                effectiveFrom = effectiveFromDate.Date;
            }
                
            var deliveryAreas = await GetAnswer(applicationId, "delivery-areas");
            

            var command = new CreateOrganisationStandardCommand
            (createdBy,
                organisationId,
                standardCode ?? 0,
                effectiveFrom,
                deliveryAreas?.Split(',').ToList());

            return command;
        }

        public async Task<string> GetAnswer(Guid applicationId, string questionTag)
        {
           var response= await _applyApiClient.GetAnswer(applicationId, questionTag);
           return response.Answer;
        }

        public async Task<string> GetJsonAnswer(Guid applicationId, string questionTag)
        {
            var response = await _applyApiClient.GetJsonAnswer(applicationId, questionTag);
            return response.Answer;
        }
    }
}


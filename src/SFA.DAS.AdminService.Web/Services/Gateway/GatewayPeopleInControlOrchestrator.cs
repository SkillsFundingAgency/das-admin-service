using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Validators;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.Apply;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayPeopleInControlOrchestrator: IGatewayPeopleInControlOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IRoatpOrganisationSummaryApiClient _organisationSummaryApiClient;
        private readonly ILogger<GatewayPeopleInControlOrchestrator> _logger;

        public GatewayPeopleInControlOrchestrator(IRoatpApplicationApiClient applyApiClient, IRoatpOrganisationSummaryApiClient organisationSummaryApiClient, ILogger<GatewayPeopleInControlOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
            _organisationSummaryApiClient = organisationSummaryApiClient;
        }

        public async Task<PeopleInControlPageViewModel> GetPeopleInControlViewModel(GetPeopleInControlRequest request)
        {
             _logger.LogInformation($"Retrieving legal name details for application {request.ApplicationId}");

            var pageId = GatewayPageIds.LegalName;

            var commonDetails =
                await _applyApiClient.GetPageCommonDetails(request.ApplicationId, pageId, request.UserName);

            var model = new PeopleInControlPageViewModel
            {
                ApplicationId = request.ApplicationId,
                PageId = pageId,
                ApplyLegalName = commonDetails.LegalName,
                Ukprn = commonDetails.Ukprn,
                Status = commonDetails.Status,
                OptionPassText = commonDetails.OptionPassText,
                OptionFailText = commonDetails.OptionFailText,
                OptionInProgressText = commonDetails.OptionInProgressText,
                SourcesCheckedOn = commonDetails.CheckedOn,
                ApplicationSubmittedOn = commonDetails.ApplicationSubmittedOn,
                GatewayReviewStatus = commonDetails.GatewayReviewStatus
            };


            model.TypeOfOrganisation = await _organisationSummaryApiClient.GetTypeOfOrganisation(request.ApplicationId);
            //  Company
            //  Company and charity
            //  Charity
            //  Sole trader or partnership
            //  Statutory institute?


            // Stubbing while I work it out


            model.CompanyDirectors = new TabularData();
            model.PeopleWithSignificantControl = new TabularData();
            model.Trustees = new TabularData();
            model.WhosInControl = new TabularData();

            // CompanyDirectors
            // Only show if company
            // tag CompaniesHouseDirectors

            // PeopleWithSignificantControl
            // Only show if company and has PSC's
            // tag CompaniesHousePSCs

            // Trustees
            // Only show if charity
            // tag CharityTrustees


            // whosInControl
            //Only show is sole trader, partnership, statutory institute or any other manual entry
            // partnership, tag is AddPartners
            // sole trader  ?? nothing?? there is a tag for AddSoleTradeDOB  1,1980
            // when sole trader (SoleTradeOrPartnership": "Sole trader") use UKRLPLegalName, AddsoleTradDob....
            // when partnership ("SoleTradeOrPartnership": "Partnership") and individual ("PartnershipType": "Individual") use AddPartners
            // when partnership and organisation - name of org
 
            // gov statute tag was AddPeopleInControl
            
            // urn school was AddPeopleInControl


            var whosInControl = new TabularData
            {
                Caption = "Company directors",   // this could be anything....
                HeadingTitles = new List<string> {"Name", "Date of birth"},
                DataRows = new List<TabularDataRow>
                {
                    new TabularDataRow {Columns = new List<string> {"BARTON, Catherine-Jane", "Jan 1969"}}
                }
            };


            // Note as per comment by Cherky on 15:55 26/03/20, make all names in tables uppercase

            model.WhosInControl = whosInControl;





            return model;
        }
    }
}

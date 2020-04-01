using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            var pageId = GatewayPageIds.PeopleInControl;

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

            model.CompanyDirectorsData = new PeopleInControlData
            {
                Caption = "Company directors",
                SubmittedApplicationHeading = "Submitted application data",
                ExternalSourceHeading = "Companies House data",
                FromExternalSource = await _organisationSummaryApiClient.GetDirectorsFromCompaniesHouse(request.ApplicationId),
                FromSubmittedApplication = await _organisationSummaryApiClient.GetDirectorsFromSubmitted(request.ApplicationId)
            };

            model.PscData = new PeopleInControlData
            {
                Caption = "People with significant control (PSC’s)",
                SubmittedApplicationHeading = "Submitted application data",
                ExternalSourceHeading = "Companies House data",
                FromExternalSource = await _organisationSummaryApiClient.GetPscsFromCompaniesHouse(request.ApplicationId),
                FromSubmittedApplication = await _organisationSummaryApiClient.GetPscsFromSubmitted(request.ApplicationId)
            };


            model.TrusteeData = new PeopleInControlData
            {
                Caption = "Trustees",
                SubmittedApplicationHeading = "Submitted application data",
                ExternalSourceHeading = "Charity commission data",
                FromExternalSource = await _organisationSummaryApiClient.GetTrusteesFromCharityCommission(request.ApplicationId),
                FromSubmittedApplication = await _organisationSummaryApiClient.GetTrusteesFromSubmitted(request.ApplicationId)
            };


            model.WhosInControlData = new PeopleInControlData
            {
                Caption = "Who's in control",
                SubmittedApplicationHeading = "Submitted application data",
                ExternalSourceHeading = null,
                FromExternalSource = null,
                FromSubmittedApplication =
                    await _organisationSummaryApiClient.GetWhosInControlFromSubmitted(request.ApplicationId)
            };
            // whosInControl
            //Only show is sole trader, partnership, statutory institute or any other manual entry
            // partnership, tag is AddPartners
            // sole trader  ?? nothing?? there is a tag for AddSoleTradeDOB  1,1980
            // when sole trader (SoleTradeOrPartnership": "Sole trader") use UKRLPLegalName, AddsoleTradDob....
            // when partnership ("SoleTradeOrPartnership": "Partnership") and individual ("PartnershipType": "Individual") use AddPartners
            // when partnership and organisation - name of org

            // gov statute tag was AddPeopleInControl

            // urn school was AddPeopleInControl


            //var whosInControl = new TabularData
            // this might use GetPeopleInControl, or alternatives for sole trader....

            //model.WhosInControl = new TabularData();


            //var whosInControl = new TabularData
            //{
            //    Caption = "Who's in control",
            //    HeadingTitles = new List<string> {"Submitted application data"},
            //    DataRows = new List<TabularDataRow>()
            //};

            // var peopleInControl = await _organisationSummaryApiClient.GetPeopleInControl(request.ApplicationId);
            // if (peopleInControl == null)
            // {
            //     //MFCMFC need to debug/check this
            //     peopleInControl = await _organisationSummaryApiClient.GetPartners(request.ApplicationId);
            // }

            // if (peopleInControl != null)
            // {
            //     if (peopleInControl?.DataRows != null && peopleInControl.DataRows.Any())
            //     {
            //         foreach (var row in peopleInControl.DataRows)
            //         {
            //             var person = string.Empty;
            //             if (row.Columns.Count == 1)
            //                 person = row.Columns[0];

            //             if (row.Columns.Count > 1)
            //                 person = $"{row.Columns[0]} ({row.Columns[1]})";

            //             if (person != string.Empty)
            //             {
            //                 whosInControl.DataRows.Add(new TabularDataRow
            //                     {Columns = new List<string> {person.ToUpper()}});
            //             }
            //         }
            //     }
            //}
            // else
            // {
            //     var soleTraderDob= await _organisationSummaryApiClient.GetSoleTraderDob(request.ApplicationId);

            //      if (!string.IsNullOrEmpty(soleTraderDob))
            //     {
            //         var person = commonDetails.LegalName.ToUpper();

            //        person = $"{person} ({soleTraderDob})";
            //         whosInControl.DataRows.Add(new TabularDataRow
            //             { Columns = new List<string> { person } });

            //    }
            // }

            //model.WhosInControl = whosInControl;


            return model;
        }
    }
}
